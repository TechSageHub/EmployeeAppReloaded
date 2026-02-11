namespace Data.Model;

public class EmployeeNextOfKin
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Relationship { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string? Address { get; set; }
}
