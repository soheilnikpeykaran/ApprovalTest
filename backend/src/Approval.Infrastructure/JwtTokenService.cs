using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Approval.Application.Interfaces;
using Approval.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Approval.Infrastructure.Services;

public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public Task<(string Token, DateTime ExpiresAt)> CreateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        var expiresAt = DateTime.UtcNow.AddHours(int.TryParse(configuration["Jwt:ExpirationHours"], out var hours) ? hours : 2);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Email)
        };
        claims.AddRange(user.UserRoles.Select(x => new Claim(ClaimTypes.Role, x.Role.Name)));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims, expires: expiresAt, signingCredentials: credentials);
        return Task.FromResult((new JwtSecurityTokenHandler().WriteToken(jwt), expiresAt));
    }
}
