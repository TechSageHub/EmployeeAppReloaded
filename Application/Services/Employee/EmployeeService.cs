using Application.ContractMapping;
using Application.Dtos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Employee;

public class EmployeeService : IEmployeeService
{
    private readonly EmployeeAppDbContext _context;
    private readonly Cloudinary _cloudinary;

    public EmployeeService(EmployeeAppDbContext context, Cloudinary cloudinary)
    {
        _context = context;
        _cloudinary = cloudinary;
    }

    public async Task<EmployeeDto?> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        try
        {
            var emailExists = _context.Employees.Any(e => e.Email == dto.Email);
            if (emailExists)
            {
                return null;
            }

            var employee = dto.ToModel();

            if (dto.Photo != null && dto.Photo.Length > 0)
            {
                var imageUrl = await UploadImageAsync(dto.Photo);
                employee.ImageUrl = imageUrl;
            }

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return employee.ToDto();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while creating the employee: " + ex.Message);
            return null;
        }
    }

    public async Task DeleteEmployeeAsync(Guid employeeId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
        {
            throw new KeyNotFoundException("Employee not found.");
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }

    public async Task<EmployeesDto> GetAllEmployeesAsync()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .ToListAsync();

        return employees.EmployeesDto();
    }

    public async Task<EmployeeDto> GetEmployeeByIdAsync(Guid employeeId)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(d => d.Id == employeeId);

        if (employee == null)
            return null;

        return new EmployeeDto
        {
            Id = employee.Id,
            Salary = employee.Salary,
            HireDate = employee.HireDate,
            DepartmentId = employee.DepartmentId,
            LastName = employee.LastName,
            FirstName = employee.FirstName,
            Email = employee.Email,
            ImageUrl = employee.ImageUrl
        };
    }

    public async Task<EmployeeDto> UpdateEmployeeAsync(UpdateEmployeeDto employeeDto)
    {
        var employee = await _context.Employees.FindAsync(employeeDto.Id);
        if (employee == null)
        {
            return null;
        }

        employee.FirstName = employeeDto.FirstName;
        employee.LastName = employeeDto.LastName;
        employee.Salary = employeeDto.Salary;
        employee.Email = employeeDto.Email;
        employee.HireDate = employeeDto.HireDate;
        employee.DepartmentId = employeeDto.DepartmentId;

       
        if (employeeDto.Photo != null && employeeDto.Photo.Length > 0)
        {
            var imageUrl = await UploadImageAsync(employeeDto.Photo);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                employee.ImageUrl = imageUrl;
            }
        }

        await _context.SaveChangesAsync();

        return new EmployeeDto
        {
            Id = employee.Id,
            Email = employee.Email,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            DepartmentId = employee.DepartmentId,
            HireDate = employee.HireDate,
            Salary = employee.Salary,
            ImageUrl = employee.ImageUrl
        };
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
            Folder = "employees"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        return result.SecureUrl.AbsoluteUri;
    }
}
