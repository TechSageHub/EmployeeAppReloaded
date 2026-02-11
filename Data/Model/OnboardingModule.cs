namespace Data.Model;

public class OnboardingModule
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public int Weight { get; set; }
    public ICollection<EmployeeOnboardingProgress> ProgressEntries { get; set; } = new List<EmployeeOnboardingProgress>();
}
