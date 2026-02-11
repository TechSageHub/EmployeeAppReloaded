namespace Data.Model;

public class EmployeeHrInfo
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public string MaritalStatus { get; set; } = default!;
    public string NationalId { get; set; } = default!;
    public string BloodGroup { get; set; } = default!;
    public string Genotype { get; set; } = default!;
}
