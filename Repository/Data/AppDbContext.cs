using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Repository.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Employee> Employees { get; set; } = default!;
    public DbSet<Attendance> Attendances { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.EnableSensitiveDataLogging().EnableDetailedErrors()
            .LogTo(Console.WriteLine, LogLevel.Information);
    }
}
