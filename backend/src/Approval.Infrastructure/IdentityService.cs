using Approval.Application.Contracts;
using Approval.Application.Interfaces;
using Approval.Domain.Entities;

namespace Approval.Infrastructure.Services;

public sealed class IdentityService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork) : IIdentityService
{
    public async Task<AuthResult?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await userRepository.ExistsByEmailAsync(email, cancellationToken)) return null;

        var role = new Role { Id = Guid.NewGuid(), Name = "Employee" };
        var user = new User
        {
            Id = Guid.NewGuid(), Email = email,
            PasswordHash = passwordHasher.Hash(request.Password),
            FirstName = request.FirstName.Trim(), LastName = request.LastName.Trim(),
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id, User = user, Role = role });
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await CreateAuthResult(user, cancellationToken);
    }

    public async Task<AuthResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !user.IsActive) return null;
        if (!passwordHasher.Verify(request.Password, user.PasswordHash)) return null;
        return await CreateAuthResult(user, cancellationToken);
    }

    public async Task<string[]> GetRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var id)) throw new InvalidOperationException("Invalid user id.");
        var user = await userRepository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("User not found.");
        return user.UserRoles.Select(x => x.Role.Name).ToArray();
    }

    private async Task<AuthResult> CreateAuthResult(User user, CancellationToken cancellationToken)
    {
        var roles = user.UserRoles.Select(x => x.Role.Name).ToArray();
        var token = await jwtTokenService.CreateTokenAsync(user, cancellationToken);
        return new AuthResult(user.Id.ToString(), roles, token.Token, token.ExpiresAt);
    }
}
