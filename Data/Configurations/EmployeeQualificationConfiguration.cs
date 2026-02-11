using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class EmployeeQualificationConfiguration : IEntityTypeConfiguration<EmployeeQualification>
{
    public void Configure(EntityTypeBuilder<EmployeeQualification> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(q => q.Institution)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(q => q.Grade)
            .HasMaxLength(100);

        builder.HasOne(q => q.Employee)
            .WithMany(e => e.Qualifications)
            .HasForeignKey(q => q.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
