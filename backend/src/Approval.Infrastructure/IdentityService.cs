using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Approval.Application.Contracts;
using Approval.Application.Interfaces;
using Approval.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Approval.Infrastructure.Services;

public sealed class IdentityService(UserManager<ApplicationUser> userManager, IConfiguration configuration) : IIdentityService
{
    public async Task<AuthResult?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FirstName = request.FirstName, LastName = request.LastName };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return null;
        await userManager.AddToRoleAsync(user, "Employee");
        return await CreateAuthResult(user);
    }

    public async Task<AuthResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password)) return null;
        return await CreateAuthResult(user);
    }

    public async Task<string[]> GetRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException("User not found.");
        return (await userManager.GetRolesAsync(user)).ToArray();
    }

    private async Task<AuthResult> CreateAuthResult(ApplicationUser user)
    {
        var roles = (await userManager.GetRolesAsync(user)).ToArray();
        var expirationHours = int.TryParse(configuration["Jwt:ExpirationHours"], out var configuredHours) ? configuredHours : 2;
        var expiresAt = DateTime.UtcNow.AddHours(expirationHours);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id), new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty), new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims, expires: expiresAt, signingCredentials: credentials);
        return new AuthResult(user.Id, roles, new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
