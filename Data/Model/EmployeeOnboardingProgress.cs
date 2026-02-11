namespace Data.Model;

public class EmployeeOnboardingProgress
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
    public Guid OnboardingModuleId { get; set; }
    public OnboardingModule OnboardingModule { get; set; } = default!;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}
