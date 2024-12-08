using Advisor.Domain.Models;

namespace Advisor.Services.Models;
public interface IAdvisorQuery
{
    Task<IEnumerable<AdvisorProfile>> GetAdvisorsAsync();
    Task<AdvisorProfile> GetAdvisorAsync(Guid id);
}