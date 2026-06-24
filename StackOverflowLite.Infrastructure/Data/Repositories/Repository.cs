using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Infrastructure.Data.DbContext;
using System.Linq.Expressions;

namespace StackOverflowLite.Infrastructure.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    private static IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[] includes)
    {
        foreach (var include in includes)
        {
            var path = ExpressionPathHelper.GetIncludePath(include);
            if (path is not null) query = query.Include(path);
        }

        return query;
    }

    public async Task<T?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default) =>
        trackChanges
            ? await _dbSet.FindAsync(new object[] { id }, cancellationToken)
            : await _dbSet.AsNoTracking().FirstOrDefaultAsync(
                e => EF.Property<Guid>(e, "Id") == id,
                cancellationToken);

    public async Task<T?> GetByIdAsync(
        Guid id,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_dbSet, includes);

        return trackChanges
            ? await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken)
            : await query.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        var count = await _dbSet.CountAsync(cancellationToken);
        var items = trackChanges
            ? await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken)
            : await _dbSet.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, count);
    }

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges = false,
        CancellationToken cancellationToken = default) =>
        trackChanges
            ? await _dbSet.Where(predicate).ToListAsync(cancellationToken)
            : await _dbSet.Where(predicate).AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_dbSet, includes).Where(predicate);

        return trackChanges
            ? await query.ToListAsync(cancellationToken)
            : await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<T?> FindFirstAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges = false,
        CancellationToken cancellationToken = default) =>
        trackChanges
            ? await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken)
            : await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<T?> FindFirstAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_dbSet, includes).Where(predicate);

        return trackChanges
            ? await query.FirstOrDefaultAsync(cancellationToken)
            : await query.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(predicate);
        var count = await query.CountAsync(cancellationToken);
        var items = trackChanges
            ? await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken)
            : await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, count);
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_dbSet, includes).Where(predicate);
        var count = await query.CountAsync(cancellationToken);

        var items = trackChanges
            ? await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken)
            : await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, count);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity) => _dbSet.Update(entity);
    public void Remove(T entity) => _dbSet.Remove(entity);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
}
