using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Advisor.Core.Repositories;
public class DBRepository<T, TContext> : IDBRepository<T> 
    where T : class
    where TContext : DbContext
{
    private readonly TContext _context;
    public DBRepository(TContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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
            throw new ValidationException($"Entity with Id '{Id}' not found.");
        }

        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public DbContext GetDBContext()
    {
        return _context;
    }

    public IQueryable<T> GetAllQueryable()
    {
        return _context.Set<T>().AsQueryable();
    }
}