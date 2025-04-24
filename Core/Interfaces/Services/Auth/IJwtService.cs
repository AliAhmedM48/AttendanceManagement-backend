namespace Core.Interfaces.Services.Auth;
public interface IJwtService
{
    string GenerateToken(int userId, string email, string role);
    public IDictionary<string, object> ValidateToken(string token);
}
