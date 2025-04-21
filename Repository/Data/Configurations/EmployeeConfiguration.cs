using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(e => e.LastName)
               .IsRequired()
               .HasMaxLength(50);

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
    }
}