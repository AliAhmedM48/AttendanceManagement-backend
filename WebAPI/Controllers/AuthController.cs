using Core.Interfaces.Services.Auth;
using Core.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        this._authenticationService = authenticationService;
    }

    [HttpGet("validate-token")]
    public async Task<IActionResult> ValidateToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userExists = await _authenticationService.IsUserExists(int.Parse(userId!));

        if (!userExists)
            return Unauthorized("Invalid token");

        return Ok();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestViewModel loginRequestViewModel)
    {
        try
        {
            var response = await _authenticationService.LoginAsync(loginRequestViewModel);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid credentials.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
