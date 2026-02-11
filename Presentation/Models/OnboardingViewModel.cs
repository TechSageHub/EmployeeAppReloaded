using System.ComponentModel.DataAnnotations;
using Application.Dtos;

namespace Presentation.Models;

public class OnboardingViewModel
{
    public OnboardingProgressDto Progress { get; set; } = new();
    public PersonalDetailsViewModel PersonalDetails { get; set; } = new();
    public QualificationViewModel Qualification { get; set; } = new();
    public NextOfKinViewModel NextOfKin { get; set; } = new();
    public HrInfoViewModel HrInfo { get; set; } = new();
    public List<QualificationItemViewModel> ExistingQualifications { get; set; } = new();
    public List<EmployeeDocumentDto> Documents { get; set; } = new();
    public bool IsComplete => Progress.IsComplete;

    public static OnboardingViewModel FromSnapshot(OnboardingSnapshotDto snapshot)
    {
        return new OnboardingViewModel
        {
            Progress = snapshot.Progress,
            PersonalDetails = new PersonalDetailsViewModel
            {
                FirstName = snapshot.PersonalDetails.FirstName,
                LastName = snapshot.PersonalDetails.LastName,
                Email = snapshot.PersonalDetails.Email,
                Gender = snapshot.PersonalDetails.Gender,
                PhoneNumber = snapshot.PersonalDetails.PhoneNumber,
                Street = snapshot.PersonalDetails.Street,
                City = snapshot.PersonalDetails.City,
                State = snapshot.PersonalDetails.State,
                Country = snapshot.PersonalDetails.Country
            },
            NextOfKin = new NextOfKinViewModel
            {
                FullName = snapshot.NextOfKin?.FullName,
                Relationship = snapshot.NextOfKin?.Relationship,
                PhoneNumber = snapshot.NextOfKin?.PhoneNumber,
                Address = snapshot.NextOfKin?.Address
            },
            HrInfo = new HrInfoViewModel
            {
                DateOfBirth = snapshot.HrInfo?.DateOfBirth ?? DateTime.Today.AddYears(-18),
                MaritalStatus = snapshot.HrInfo?.MaritalStatus,
                NationalId = snapshot.HrInfo?.NationalId,
                BloodGroup = snapshot.HrInfo?.BloodGroup,
                Genotype = snapshot.HrInfo?.Genotype
            },
            ExistingQualifications = snapshot.Qualifications.Select(q => new QualificationItemViewModel
            {
                Title = q.Title,
                Institution = q.Institution,
                Year = q.Year,
                Grade = q.Grade
            }).ToList()
        };
    }
}

public class PersonalDetailsViewModel
{
    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Gender { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Street { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string State { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Country { get; set; } = string.Empty;

    public PersonalDetailsDto ToDto() => new()
    {
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        Gender = Gender,
        PhoneNumber = PhoneNumber,
        Street = Street,
        City = City,
        State = State,
        Country = Country
    };
}

public class QualificationViewModel
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Institution { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; } = DateTime.Now.Year;

    [StringLength(100)]
    public string? Grade { get; set; }

    public QualificationDto ToDto() => new()
    {
        Title = Title,
        Institution = Institution,
        Year = Year,
        Grade = Grade
    };
}

public class QualificationItemViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Grade { get; set; }
}

public class NextOfKinViewModel
{
    [Required, StringLength(200)]
    public string? FullName { get; set; }

    [Required, StringLength(100)]
    public string? Relationship { get; set; }

    [Required, StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    public NextOfKinDto ToDto() => new()
    {
        FullName = FullName ?? string.Empty,
        Relationship = Relationship ?? string.Empty,
        PhoneNumber = PhoneNumber ?? string.Empty,
        Address = Address
    };
}

public class HrInfoViewModel
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-18);

    [Required, StringLength(50)]
    public string? MaritalStatus { get; set; }

    [Required, StringLength(50)]
    public string? NationalId { get; set; }

    [Required, StringLength(5)]
    public string? BloodGroup { get; set; }

    [Required, StringLength(5)]
    public string? Genotype { get; set; }

    public HrInfoDto ToDto() => new()
    {
        DateOfBirth = DateOfBirth,
        MaritalStatus = MaritalStatus ?? string.Empty,
        NationalId = NationalId ?? string.Empty,
        BloodGroup = BloodGroup ?? string.Empty,
        Genotype = Genotype ?? string.Empty
    };
}
