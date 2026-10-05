using Approval.Application.Interfaces;
using Approval.Domain.Entities;
using Approval.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Approval.Infrastructure.Repositories;

public sealed class Repository<T>(ApplicationDbContext db) : IRepository<T> where T : class
{
    public IQueryable<T> Query() => db.Set<T>();
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => await db.Set<T>().FindAsync([id], cancellationToken);
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) => await db.Set<T>().AddAsync(entity, cancellationToken);
    public void Update(T entity) => db.Set<T>().Update(entity);
}

public sealed class RequestRepository(ApplicationDbContext db) : IRequestRepository
{
    public async Task<IReadOnlyList<Request>> GetVisibleAsync(Guid userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default)
    {
        return await db.Requests
            .AsNoTracking()
            .Where(x => x.CreatedByUserId == userId || roles.Contains(x.AssignedRole))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

public sealed class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).FirstOrDefaultAsync(x => x.Email == email.Trim().ToLowerInvariant(), cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(x => x.Email == email.Trim().ToLowerInvariant(), cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) => await db.Users.AddAsync(user, cancellationToken);
}

public sealed class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}
