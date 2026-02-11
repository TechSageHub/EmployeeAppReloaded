namespace Data.Model;

public class EmployeeQualification
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Institution { get; set; } = default!;
    public int Year { get; set; }
    public string? Grade { get; set; }
}
