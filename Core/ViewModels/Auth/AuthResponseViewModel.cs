﻿namespace Core.ViewModels.Auth;

public class AuthResponseViewModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
