using Microsoft.EntityFrameworkCore;

namespace Advisor.Core.Pagination;
public class PaginationService : IPaginator
{
    public async Task<PagedResult<T>> PaginateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize) where T : class
    {
        // Calculate total records
        var totalRecords = await query.CountAsync();

        // Apply pagination
        var items = await query.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}