using BuildMart.Application.DTOs.Auth;

namespace BuildMart.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto> GetCurrentUserAsync(string userId);
    Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);
}
