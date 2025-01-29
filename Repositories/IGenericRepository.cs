using System.Linq.Expressions;

namespace let_em_cook.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(params object[] keyValues);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> SaveChangesAsync();
}