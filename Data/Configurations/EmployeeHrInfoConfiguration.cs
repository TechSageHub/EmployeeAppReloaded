using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class EmployeeHrInfoConfiguration : IEntityTypeConfiguration<EmployeeHrInfo>
{
    public void Configure(EntityTypeBuilder<EmployeeHrInfo> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.MaritalStatus)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(h => h.NationalId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(h => h.BloodGroup)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(h => h.Genotype)
            .IsRequired()
            .HasMaxLength(5);

        builder.HasOne(h => h.Employee)
            .WithOne(e => e.HrInfo)
            .HasForeignKey<EmployeeHrInfo>(h => h.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
