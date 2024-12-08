using Microsoft.EntityFrameworkCore;

namespace Advisor.Core.Repositories;
public interface IDBRepository<T> : IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    DbContext GetDBContext();
}