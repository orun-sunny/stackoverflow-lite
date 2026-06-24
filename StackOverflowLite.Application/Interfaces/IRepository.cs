using System.Linq.Expressions;

namespace StackOverflowLite.Application.Interfaces;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(
        Guid id,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes);

    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes);

    Task<T?> FindFirstAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

    Task<T?> FindFirstAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes);

    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
