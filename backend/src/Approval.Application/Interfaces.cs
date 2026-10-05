using Approval.Domain.Entities;
using Approval.Application.Contracts;

namespace Approval.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    IQueryable<T> Query();
    Task<List<T>> ToListAsync(IQueryable<T> query, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
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
