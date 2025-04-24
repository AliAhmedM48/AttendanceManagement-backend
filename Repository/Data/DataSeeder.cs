using Core.Enums;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;

namespace Repository.Data;
public static class DataSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            var email = configuration["AdminUser:Email"]!;
            var password = configuration["AdminUser:Password"]!;
            var firstName = configuration["AdminUser:FirstName"]!;
            var lastName = configuration["AdminUser:LastName"]!;

            using var hmac = new HMACSHA512();
            byte[] salt = hmac.Key;
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            string passwordHashBase64 = Convert.ToBase64String(hash);
            string passwordSaltBase64 = Convert.ToBase64String(salt);

            var adminUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = passwordHashBase64,
                PasswordSalt = passwordSaltBase64,
                Role = UserRole.Admin,
                CreatedAt = DateTime.Now,
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
