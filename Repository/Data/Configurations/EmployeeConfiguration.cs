using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {

        builder.Property(e => e.PhoneNumber)
                     .IsRequired()
                     .HasMaxLength(11);

        builder.Property(e => e.NationalId)
               .IsRequired()
               .HasMaxLength(14);

        builder.Property(e => e.SignaturePath)
               .HasMaxLength(255);

        builder.HasMany(e => e.Attendances)
               .WithOne(a => a.Employee)
               .HasForeignKey(a => a.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.PhoneNumber)
             .IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Employee_PhoneNumber_Format", "LEN([PhoneNumber]) = 11 AND [PhoneNumber] NOT LIKE '%[^0-9]%'");
            t.HasCheckConstraint("CK_Employee_NationalId_Format", "LEN([NationalId]) = 14 AND [NationalId] NOT LIKE '%[^0-9]%'");
        });
    }
}