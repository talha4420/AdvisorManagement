using Advisor.Domain.Models;

namespace Advisor.Services.Models;
public interface IAdvisorQuery
{
    Task<IEnumerable<AdvisorProfile>> GetAdvisorsAsync();
    Task<PagedResult<AdvisorProfile>> GetAdvisorsAsyncWithPage(int pageNumber, int pageSize);
    Task<AdvisorProfile> GetAdvisorAsync(Guid id);
}