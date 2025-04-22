using Core.Interfaces;
using Core.Interfaces.Services.Auth;
using Core.Models;
using Core.ViewModels.Auth;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Service.Auth;
public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticationService(IJwtService jwtService, IUnitOfWork unitOfWork)
    {
        this._jwtService = jwtService;
        this._unitOfWork = unitOfWork;
    }
    public async Task<AuthResponseViewModel> LoginAsync(LoginRequestViewModel loginRequestViewModel)
    {
        var user = await _unitOfWork.GetRepository<User>().GetAll(u => u.Email == loginRequestViewModel.Email).FirstOrDefaultAsync();
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (!VerifyPassword(loginRequestViewModel.Password, user.PasswordHash, user.PasswordSalt))
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());
        return new AuthResponseViewModel { Token = token, Email = user.Email, Role = user.Role.ToString() };
    }



    public (string, string) CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        var passwordHashBase64 = Convert.ToBase64String(hash);
        var passwordSaltBase64 = Convert.ToBase64String(salt);

        return (passwordHashBase64, passwordSaltBase64);
    }

    public bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] hashBytes = Convert.FromBase64String(storedHash);
        byte[] saltBytes = Convert.FromBase64String(storedSalt);

        using var hmac = new HMACSHA512(saltBytes);
        var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return hashBytes.SequenceEqual(computed);
    }
}
