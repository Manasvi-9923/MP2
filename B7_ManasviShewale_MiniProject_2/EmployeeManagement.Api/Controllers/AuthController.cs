using EmployeeManagement.Api.DTOs.Auth;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new { success = false, message = result.Message });
        }

        return Ok(result.Data ?? new AuthResponse
        {
            Success = true,
            Message = result.Message
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new { success = false, message = result.Message });
        }

        return Ok(result.Data);
    }
}
