using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class PayrollRecordConfiguration : IEntityTypeConfiguration<PayrollRecord>
{
    public void Configure(EntityTypeBuilder<PayrollRecord> builder)
    {
        builder.Property(p => p.GrossSalary)
            .HasPrecision(18, 2);

        builder.Property(p => p.TaxDeduction)
            .HasPrecision(18, 2);

        builder.Property(p => p.PensionDeduction)
            .HasPrecision(18, 2);

        builder.Property(p => p.OtherDeductions)
            .HasPrecision(18, 2);

        builder.Property(p => p.Bonuses)
            .HasPrecision(18, 2);

        builder.Property(p => p.NetSalary)
            .HasPrecision(18, 2);
    }
}
