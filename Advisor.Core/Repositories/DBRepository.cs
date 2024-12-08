using Microsoft.EntityFrameworkCore;

namespace Advisor.Core.Repositories;
public class DBRepository<T> : IDBRepository<T> where T : class
{
    private readonly DbContext _context;
    public DBRepository(DbContext context)
    {
        _context = context;
    }
    public async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task<T> DeleteAsync(Guid id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
        {
            return entity;
        }
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task<T> GetAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }
    public async Task<T> UpdateAsync(Guid Id, T entity)
    {
        var existingEntity = await GetAsync(Id);

        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"Entity with Id '{Id}' not found.");
        }

        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public DbContext GetDBContext()
    {
        return _context;
    }
}