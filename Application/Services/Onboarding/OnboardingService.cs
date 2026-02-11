using Application.Dtos;
using Data.Context;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Onboarding;

public class OnboardingService : IOnboardingService
{
    private readonly EmployeeAppDbContext _context;

    private static readonly List<OnboardingModuleDefinition> ModuleDefinitions =
    [
        new("personal_details", "Personal Details", 1, true, 1),
        new("qualifications", "Qualifications", 2, true, 1),
        new("next_of_kin", "Next of Kin", 3, true, 1),
        new("hr_info", "HR Information", 4, true, 1),
        new("documents", "Documents", 5, true, 1)
    ];

    public OnboardingService(EmployeeAppDbContext context)
    {
        _context = context;
    }

    public async Task<OnboardingSnapshotDto?> GetSnapshotAsync(string userId)
    {
        var employee = await _context.Employees
            .Include(e => e.Address)
            .Include(e => e.Qualifications)
            .Include(e => e.NextOfKin)
            .Include(e => e.HrInfo)
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee == null)
        {
            return null;
        }

        await EnsureModuleDefinitionsAsync();

        var progress = await UpdateDocumentsProgressAsync(userId);

        var personalDetails = new PersonalDetailsDto
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Gender = employee.Gender ?? string.Empty,
            PhoneNumber = employee.PhoneNumber ?? string.Empty,
            Street = employee.Address?.Street ?? string.Empty,
            City = employee.Address?.City ?? string.Empty,
            State = employee.Address?.State ?? string.Empty,
            Country = employee.Address?.Country ?? string.Empty
        };

        var qualifications = employee.Qualifications
            .OrderByDescending(q => q.Year)
            .Select(q => new QualificationDto
            {
                Title = q.Title,
                Institution = q.Institution,
                Year = q.Year,
                Grade = q.Grade
            })
            .ToList();

        var nextOfKin = employee.NextOfKin == null
            ? null
            : new NextOfKinDto
            {
                FullName = employee.NextOfKin.FullName,
                Relationship = employee.NextOfKin.Relationship,
                PhoneNumber = employee.NextOfKin.PhoneNumber,
                Address = employee.NextOfKin.Address
            };

        var hrInfo = employee.HrInfo == null
            ? null
            : new HrInfoDto
            {
                DateOfBirth = employee.HrInfo.DateOfBirth,
                MaritalStatus = employee.HrInfo.MaritalStatus,
                NationalId = employee.HrInfo.NationalId,
                BloodGroup = employee.HrInfo.BloodGroup,
                Genotype = employee.HrInfo.Genotype
            };

        return new OnboardingSnapshotDto
        {
            Progress = progress,
            PersonalDetails = personalDetails,
            Qualifications = qualifications,
            NextOfKin = nextOfKin,
            HrInfo = hrInfo
        };
    }

    public async Task<OnboardingProgressDto> UpdatePersonalDetailsAsync(string userId, PersonalDetailsDto dto)
    {
        var employee = await GetEmployeeAsync(userId);

        employee.FirstName = dto.FirstName.Trim();
        employee.LastName = dto.LastName.Trim();
        employee.Gender = dto.Gender.Trim();
        employee.PhoneNumber = dto.PhoneNumber.Trim();

        if (employee.Address == null)
        {
            employee.Address = new EmployeeAddress
            {
                EmployeeId = employee.Id
            };
        }

        employee.Address.Street = dto.Street.Trim();
        employee.Address.City = dto.City.Trim();
        employee.Address.State = dto.State.Trim();
        employee.Address.Country = dto.Country.Trim();

        await _context.SaveChangesAsync();

        await UpsertProgressAsync(employee.Id, "personal_details", true);
        await UpdateCompletionAsync(employee);

        return await GetProgressAsync(employee.Id);
    }

    public async Task<OnboardingProgressDto> AddQualificationAsync(string userId, QualificationDto dto)
    {
        var employee = await GetEmployeeAsync(userId);

        var qualification = new EmployeeQualification
        {
            Id = Guid.NewGuid(),
            EmployeeId = employee.Id,
            Title = dto.Title.Trim(),
            Institution = dto.Institution.Trim(),
            Year = dto.Year,
            Grade = dto.Grade?.Trim()
        };

        _context.EmployeeQualifications.Add(qualification);
        await _context.SaveChangesAsync();

        var hasQualification = await _context.EmployeeQualifications
            .AnyAsync(q => q.EmployeeId == employee.Id);

        await UpsertProgressAsync(employee.Id, "qualifications", hasQualification);
        await UpdateCompletionAsync(employee);

        return await GetProgressAsync(employee.Id);
    }

    public async Task<OnboardingProgressDto> SaveNextOfKinAsync(string userId, NextOfKinDto dto)
    {
        var employee = await GetEmployeeAsync(userId);

        if (employee.NextOfKin == null)
        {
            var nextOfKin = new EmployeeNextOfKin
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                FullName = dto.FullName.Trim(),
                Relationship = dto.Relationship.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                Address = dto.Address?.Trim()
            };

            _context.EmployeeNextOfKins.Add(nextOfKin);
            employee.NextOfKin = nextOfKin;
        }
        else
        {
            employee.NextOfKin.FullName = dto.FullName.Trim();
            employee.NextOfKin.Relationship = dto.Relationship.Trim();
            employee.NextOfKin.PhoneNumber = dto.PhoneNumber.Trim();
            employee.NextOfKin.Address = dto.Address?.Trim();

            _context.EmployeeNextOfKins.Update(employee.NextOfKin);
        }

        await _context.SaveChangesAsync();

        await UpsertProgressAsync(employee.Id, "next_of_kin", true);
        await UpdateCompletionAsync(employee);

        return await GetProgressAsync(employee.Id);
    }

    public async Task<OnboardingProgressDto> SaveHrInfoAsync(string userId, HrInfoDto dto)
    {
        var employee = await GetEmployeeAsync(userId);

        if (employee.HrInfo == null)
        {
            var hrInfo = new EmployeeHrInfo
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                DateOfBirth = dto.DateOfBirth.Date,
                MaritalStatus = dto.MaritalStatus.Trim(),
                NationalId = dto.NationalId.Trim(),
                BloodGroup = dto.BloodGroup.Trim(),
                Genotype = dto.Genotype.Trim()
            };

            _context.EmployeeHrInfos.Add(hrInfo);
            employee.HrInfo = hrInfo;
        }
        else
        {
            employee.HrInfo.DateOfBirth = dto.DateOfBirth.Date;
            employee.HrInfo.MaritalStatus = dto.MaritalStatus.Trim();
            employee.HrInfo.NationalId = dto.NationalId.Trim();
            employee.HrInfo.BloodGroup = dto.BloodGroup.Trim();
            employee.HrInfo.Genotype = dto.Genotype.Trim();
            _context.EmployeeHrInfos.Update(employee.HrInfo);
        }

        await _context.SaveChangesAsync();

        await UpsertProgressAsync(employee.Id, "hr_info", true);
        await UpdateCompletionAsync(employee);

        return await GetProgressAsync(employee.Id);
    }

    public async Task<OnboardingProgressDto> UpdateDocumentsProgressAsync(string userId)
    {
        var employee = await GetEmployeeAsync(userId);
        var hasDocuments = await _context.EmployeeDocuments
            .AnyAsync(d => d.EmployeeId == employee.Id);

        await UpsertProgressAsync(employee.Id, "documents", hasDocuments);
        await UpdateCompletionAsync(employee);

        return await GetProgressAsync(employee.Id);
    }

    public async Task<bool> IsOnboardingCompleteAsync(string userId)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee == null)
        {
            return true;
        }

        if (employee.IsOnboardingComplete)
        {
            return true;
        }

        await EnsureModuleDefinitionsAsync();
        var progress = await GetProgressAsync(employee.Id);
        return progress.IsComplete;
    }

    private async Task<Data.Model.Employee> GetEmployeeAsync(string userId)
    {
        var employee = await _context.Employees
            .Include(e => e.Address)
            .Include(e => e.NextOfKin)
            .Include(e => e.HrInfo)
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee == null)
        {
            throw new InvalidOperationException("Employee record not found.");
        }

        return employee;
    }

    private async Task EnsureModuleDefinitionsAsync()
    {
        var existing = await _context.OnboardingModules
            .Select(m => m.Key)
            .ToListAsync();

        var missing = ModuleDefinitions
            .Where(def => !existing.Contains(def.Key))
            .Select(def => new OnboardingModule
            {
                Id = Guid.NewGuid(),
                Key = def.Key,
                Name = def.Name,
                SortOrder = def.SortOrder,
                IsRequired = def.IsRequired,
                Weight = def.Weight
            })
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        _context.OnboardingModules.AddRange(missing);
        await _context.SaveChangesAsync();
    }

    private async Task<OnboardingProgressDto> GetProgressAsync(Guid employeeId)
    {
        var modules = await _context.OnboardingModules
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        var progressEntries = await _context.EmployeeOnboardingProgress
            .Where(p => p.EmployeeId == employeeId)
            .ToListAsync();

        var statuses = modules.Select(module =>
        {
            var progress = progressEntries.FirstOrDefault(p => p.OnboardingModuleId == module.Id);
            return new OnboardingModuleStatusDto
            {
                Key = module.Key,
                Name = module.Name,
                IsRequired = module.IsRequired,
                IsCompleted = progress?.IsCompleted ?? false
            };
        }).ToList();

        var requiredCount = statuses.Count(s => s.IsRequired);
        var completedCount = statuses.Count(s => s.IsRequired && s.IsCompleted);
        var percent = requiredCount == 0
            ? 100
            : (int)Math.Round(completedCount * 100.0 / requiredCount, MidpointRounding.AwayFromZero);

        return new OnboardingProgressDto
        {
            ProgressPercent = percent,
            IsComplete = requiredCount > 0 && completedCount == requiredCount,
            Modules = statuses
        };
    }

    private async Task UpsertProgressAsync(Guid employeeId, string moduleKey, bool isCompleted)
    {
        await EnsureModuleDefinitionsAsync();

        var module = await _context.OnboardingModules
            .FirstOrDefaultAsync(m => m.Key == moduleKey);

        if (module == null)
        {
            throw new InvalidOperationException($"Onboarding module '{moduleKey}' not configured.");
        }

        var entry = await _context.EmployeeOnboardingProgress
            .FirstOrDefaultAsync(p => p.EmployeeId == employeeId && p.OnboardingModuleId == module.Id);

        if (entry == null)
        {
            entry = new EmployeeOnboardingProgress
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                OnboardingModuleId = module.Id,
                IsCompleted = isCompleted,
                CompletedAt = isCompleted ? DateTime.UtcNow : null
            };

            _context.EmployeeOnboardingProgress.Add(entry);
        }
        else
        {
            entry.IsCompleted = isCompleted;
            entry.CompletedAt = isCompleted ? DateTime.UtcNow : null;
        }

        await _context.SaveChangesAsync();
    }

    private async Task UpdateCompletionAsync(Data.Model.Employee employee)
    {
        var progress = await GetProgressAsync(employee.Id);
        if (!progress.IsComplete || employee.IsOnboardingComplete)
        {
            return;
        }

        employee.IsOnboardingComplete = true;
        employee.OnboardingCompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private sealed record OnboardingModuleDefinition(
        string Key,
        string Name,
        int SortOrder,
        bool IsRequired,
        int Weight);
}
