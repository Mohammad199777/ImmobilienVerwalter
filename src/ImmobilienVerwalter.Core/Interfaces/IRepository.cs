using System.Linq.Expressions;
using ImmobilienVerwalter.Core.Entities;

namespace ImmobilienVerwalter.Core.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id); // Soft Delete
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}
