namespace Advisor.Core.Pagination;

public interface IPaginator
{
    Task<PagedResult<T>> PaginateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize) where T : class;
}