using Approval.Application.Contracts;
using Approval.Domain.Entities;

namespace Approval.Application.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
}

public interface IRequestRepository
{
    Task<IReadOnlyList<Request>> GetVisibleAsync(Guid userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public interface IJwtTokenService
{
    Task<(string Token, DateTime ExpiresAt)> CreateTokenAsync(User user, CancellationToken cancellationToken = default);
}

public interface IIdentityService
{
    Task<AuthResult?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<string[]> GetRolesAsync(string userId, CancellationToken cancellationToken = default);
}

public sealed record AuthResult(string UserId, string[] Roles, string Token, DateTime ExpiresAt);

public interface IRequestService
{
    Task<RequestResponse> CreateAsync(string userId, CreateRequestDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RequestResponse>> GetVisibleAsync(string userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default);
    Task<RequestResponse> DecideAsync(Guid requestId, string userId, IReadOnlyCollection<string> roles, string action, CancellationToken cancellationToken = default);
}

public interface IRoutingService
{
    string ResolveAssignedRole(decimal amount);
}
