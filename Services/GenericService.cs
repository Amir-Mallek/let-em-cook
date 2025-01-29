using System.Linq.Expressions;
using let_em_cook.Repositories;

namespace let_em_cook.Services;

public class GenericService<T> : IGenericService<T> where T : class
{
    protected readonly IRepository<T> _repository;

    public GenericService(IRepository<T> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public virtual async Task<T?> GetByIdAsync(params object[] keyValues)
    {
        return await _repository.GetByIdAsync(keyValues);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _repository.FindAsync(predicate);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _repository.FindAsync(predicate).ContinueWith(task => task.Result.Any());
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        return predicate != null 
            ? await _repository.FindAsync(predicate).ContinueWith(task => task.Result.Count())
            : await _repository.GetAllAsync().ContinueWith(task => task.Result.Count());
    }

    public virtual async Task<bool> SaveChangesAsync()
    {
        return await _repository.SaveChangesAsync();
    }
}