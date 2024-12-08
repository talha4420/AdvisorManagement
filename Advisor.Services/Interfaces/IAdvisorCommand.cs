using Advisor.Domain.Models;

namespace Advisor.Services.Models;
public interface IAdvisorCommand
{
    Task<AdvisorProfile> CreateAdvisorAsync(AdvisorProfile advisor);
    Task<AdvisorProfile> UpdateAdvisorAsync(Guid Id, AdvisorProfile advisor);
    Task<AdvisorProfile> DeleteAdvisorAsync(Guid Id);
}