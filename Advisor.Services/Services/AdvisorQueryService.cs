using Advisor.Core.Repositories;
using Advisor.Domain.Models;
using Advisor.Services.Models;
using Microsoft.Extensions.Logging;
using System.Data.Common;

public class AdvisorQueryService(IDBRepository<AdvisorProfile> advisorRepository,
                            ILogger<AdvisorQueryService> logger) : IAdvisorQuery
{
    public async Task<IEnumerable<AdvisorProfile>> GetAdvisorsAsync()
    {
        logger.LogInformation("Fetching all advisors at {Time}", DateTime.UtcNow);
        var advisors = await advisorRepository.GetAllAsync();
        return advisors;
    }

    public async Task<AdvisorProfile> GetAdvisorAsync(Guid id)
    {
        logger.LogInformation("Fetching advisor with ID: {Id} at {Time}", id, DateTime.UtcNow);
        var advisor = await advisorRepository.GetAsync(id);

        if (advisor == null)
        {
            logger.LogWarning("Advisor with ID: {Id} not found at {Time}", id, DateTime.UtcNow);
        }

        return advisor;
    }
}
