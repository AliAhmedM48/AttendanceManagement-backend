using Core.ViewModels.Auth;

namespace Core.Interfaces.Services.Auth;
public interface IAuthenticationService
{
    Task<AuthResponseViewModel> LoginAsync(LoginRequestViewModel loginRequestViewModel);
    public (string, string) CreatePasswordHash(string password);


    public Task<bool> IsUserExists(int userId);


}
