namespace Advisor.Core.Repositories;
public interface IBaseRepository<T> where T : class
{
    Task<T> GetAsync(Guid id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(Guid Id, T entity);
    Task<T> DeleteAsync(Guid id);
}