using BuildMart.API.Common;
using BuildMart.Application.DTOs.Auth;
using BuildMart.Application.DTOs.Common;
using BuildMart.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildMart.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Creates a new Customer account and returns a JWT.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<AuthResponseDto>.SuccessResponse(result, "Account created successfully."));
    }

    /// <summary>Authenticates a user and returns a JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful."));
    }

    /// <summary>Returns the profile of the currently authenticated user.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _authService.GetCurrentUserAsync(User.GetUserId());
        return Ok(ApiResponse<UserDto>.SuccessResponse(user));
    }

    /// <summary>Updates the profile of the currently authenticated user.</summary>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var user = await _authService.UpdateProfileAsync(User.GetUserId(), dto);
        return Ok(ApiResponse<UserDto>.SuccessResponse(user, "Profile updated successfully."));
    }

    /// <summary>
    /// Stateless JWT logout: there is no server-side session to invalidate,
    /// so this simply confirms the request — the client discards the token.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Logged out successfully."));
    }
}
