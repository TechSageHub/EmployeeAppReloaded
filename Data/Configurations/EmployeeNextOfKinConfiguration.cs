using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class EmployeeNextOfKinConfiguration : IEntityTypeConfiguration<EmployeeNextOfKin>
{
    public void Configure(EntityTypeBuilder<EmployeeNextOfKin> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(k => k.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(k => k.Relationship)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.PhoneNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(k => k.Address)
            .HasMaxLength(300);

        builder.HasOne(k => k.Employee)
            .WithOne(e => e.NextOfKin)
            .HasForeignKey<EmployeeNextOfKin>(k => k.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
