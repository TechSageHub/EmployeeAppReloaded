using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class EmployeeOnboardingProgressConfiguration : IEntityTypeConfiguration<EmployeeOnboardingProgress>
{
    public void Configure(EntityTypeBuilder<EmployeeOnboardingProgress> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => new { p.EmployeeId, p.OnboardingModuleId })
            .IsUnique();

        builder.HasOne(p => p.Employee)
            .WithMany(e => e.OnboardingProgress)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.OnboardingModule)
            .WithMany(m => m.ProgressEntries)
            .HasForeignKey(p => p.OnboardingModuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
