namespace Application.Dtos;

public class OnboardingProgressDto
{
    public int ProgressPercent { get; set; }
    public bool IsComplete { get; set; }
    public List<OnboardingModuleStatusDto> Modules { get; set; } = new();
}

public class OnboardingModuleStatusDto
{
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsRequired { get; set; }
    public bool IsCompleted { get; set; }
}

public class PersonalDetailsDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string Country { get; set; } = default!;
}

public class QualificationDto
{
    public string Title { get; set; } = default!;
    public string Institution { get; set; } = default!;
    public int Year { get; set; }
    public string? Grade { get; set; }
}

public class NextOfKinDto
{
    public string FullName { get; set; } = default!;
    public string Relationship { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string? Address { get; set; }
}

public class HrInfoDto
{
    public DateTime DateOfBirth { get; set; }
    public string MaritalStatus { get; set; } = default!;
    public string NationalId { get; set; } = default!;
    public string BloodGroup { get; set; } = default!;
    public string Genotype { get; set; } = default!;
}

public class OnboardingSnapshotDto
{
    public OnboardingProgressDto Progress { get; set; } = new();
    public PersonalDetailsDto PersonalDetails { get; set; } = new();
    public List<QualificationDto> Qualifications { get; set; } = new();
    public NextOfKinDto? NextOfKin { get; set; }
    public HrInfoDto? HrInfo { get; set; }
}
