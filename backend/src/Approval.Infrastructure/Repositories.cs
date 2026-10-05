using Approval.Application.Interfaces;
using Approval.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Approval.Infrastructure.Repositories;

public sealed class Repository<T>(ApplicationDbContext db) : IRepository<T> where T : class
{
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => await db.Set<T>().FindAsync([id], cancellationToken);
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) => await db.Set<T>().AddAsync(entity, cancellationToken);
    public void Update(T entity) => db.Set<T>().Update(entity);
    public IQueryable<T> Query() => db.Set<T>().AsNoTracking();
    public Task<List<T>> ToListAsync(IQueryable<T> query, CancellationToken cancellationToken = default) => query.ToListAsync(cancellationToken);
}

public sealed class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}
