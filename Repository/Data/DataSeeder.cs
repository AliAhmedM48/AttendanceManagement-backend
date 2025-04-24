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
    public static async Task SeedData(IServiceProvider services)
    {
        await SeedAdminAsync(services);
        await SeedEmployeeDataAsync(services);
    }

    private static async Task SeedAdminAsync(IServiceProvider services)
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
    private static async Task SeedEmployeeDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await context.Employees.AnyAsync()) return;

        var firstNames = new[] { "Ahmed", "Mohamed", "Omar", "Youssef", "Mostafa", "Ali", "Khaled", "Ibrahim", "Karim", "Tarek" };
        var lastNames = new[] { "Hassan", "Ali", "Mahmoud", "Fahmy", "Saeed", "Gamal", "Mostafa", "Farouk", "Tawfik", "Abdallah" };
        var governorates = new[] { "Cairo", "Giza", "Alexandria", "Dakahlia", "Qalyubia", "Sharqia", "Asyut", "Sohag", "Mansoura", "Minya" };

        var random = new Random();

        for (int i = 0; i < 20; i++)
        {
            using var hmac = new HMACSHA512();
            var password = "123456";
            var passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            var passwordSalt = Convert.ToBase64String(hmac.Key);

            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var fullName = $"{firstName}.{lastName}".ToLower();

            var centuryDigit = random.Next(2, 4); // 2 or 3
            var year = random.Next(0, 100).ToString("D2"); // YY
            var month = random.Next(1, 13).ToString("D2"); // MM
            var day = random.Next(1, 29).ToString("D2"); // DD
            var govCode = random.Next(1, 28).ToString("D2"); // Governorate code
            var serial = random.Next(0, 10000).ToString("D4"); // Serial
            var checkDigit = random.Next(0, 10); // Check digit

            var employee = new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                Email = $"{fullName}{random.Next(100, 999)}@company.com",
                PhoneNumber = $"01{random.Next(0, 10)}{random.Next(10000000, 99999999)}",
                NationalId = $"{centuryDigit}{year}{month}{day}{govCode}{serial}{checkDigit}", // 11 digits after "29X" => total 14
                Governorate = governorates[random.Next(governorates.Length)],
                BirthDate = new DateTime(random.Next(1985, 2000), random.Next(1, 13), random.Next(1, 28)),
                Gender = UserGender.Male,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = UserRole.Employee,
                CreatedAt = DateTime.Now
            };

            var attendances = new List<Attendance>();
            var startDate = DateTime.Today.AddYears(-2);
            var endDate = DateTime.Today;

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Friday || random.NextDouble() < 0.2) continue; // skip weekends + some random off days

                var checkIn = date.AddHours(9).AddMinutes(random.Next(0, 30));
                var checkOut = checkIn.AddHours(8).AddMinutes(random.Next(-30, 30));
                var hoursWorked = (checkOut - checkIn).TotalHours;

                attendances.Add(new Attendance
                {
                    CheckInTime = checkIn,
                    CheckOutTime = checkOut,
                    TotalWorkedHours = Math.Round(hoursWorked, 2),
                    CreatedAt = checkIn
                });
            }

            employee.Attendances = attendances;

            await context.Employees.AddAsync(employee);
        }

        await context.SaveChangesAsync();
    }


}
