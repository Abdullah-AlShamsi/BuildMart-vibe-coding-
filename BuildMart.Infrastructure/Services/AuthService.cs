using BuildMart.Application.DTOs.Auth;
using BuildMart.Application.Interfaces;
using BuildMart.Domain.Entities;
using BuildMart.Domain.Enums;
using BuildMart.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace BuildMart.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing is not null)
        {
            throw new ConflictException("An account with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, nameof(UserRole.Customer));

        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAppException("Invalid email or password.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
        {
            throw new UnauthorizedAppException("Invalid email or password.");
        }

        return await BuildAuthResponseAsync(user);
    }

    public async Task<UserDto> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        return await MapToUserDtoAsync(user);
    }

    public async Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Address = dto.Address;
        user.City = dto.City;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));
        }

        return await MapToUserDtoAsync(user);
    }

    private async Task<AuthResponseDto> BuildAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtService.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = await MapToUserDtoAsync(user, roles)
        };
    }

    private async Task<UserDto> MapToUserDtoAsync(ApplicationUser user, IList<string>? roles = null)
    {
        roles ??= await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            City = user.City,
            Role = roles.FirstOrDefault() ?? nameof(UserRole.Customer),
            CreatedAt = user.CreatedAt
        };
    }
}
