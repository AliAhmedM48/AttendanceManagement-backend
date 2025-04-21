using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Data.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.CheckInTime)
               .IsRequired();

        builder.Property(a => a.CheckOutTime)
               .IsRequired(false);

        builder.HasIndex(a => a.EmployeeId)
               .HasDatabaseName("IX_Attendance_EmployeeId");
    }
}