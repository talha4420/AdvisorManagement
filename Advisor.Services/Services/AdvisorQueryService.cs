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
        try
        {
            logger.LogInformation("Fetching all advisors at {Time}", DateTime.UtcNow);
            var advisors = await advisorRepository.GetAllAsync();
            return advisors;
        }
        catch (DbException dbEx) 
        {
            logger.LogError(dbEx, "A database error occurred while fetching all advisors at {Time}", DateTime.UtcNow);
            throw new ApplicationException("A database error occurred while retrieving advisors.", dbEx);
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "An unexpected error occurred while fetching all advisors at {Time}", DateTime.UtcNow);
            throw new ApplicationException("An unexpected error occurred while retrieving advisors.", ex);
        }
    }

    public async Task<AdvisorProfile> GetAdvisorAsync(Guid id)
    {
        try
        {
            logger.LogInformation("Fetching advisor with ID: {Id} at {Time}", id, DateTime.UtcNow);
            var advisor = await advisorRepository.GetAsync(id);

            if (advisor == null)
            {
                logger.LogWarning("Advisor with ID: {Id} not found at {Time}", id, DateTime.UtcNow);
            }

            return advisor;
        }
        catch (DbException dbEx)
        {
            logger.LogError(dbEx, "A database error occurred while fetching the advisor with ID: {Id} at {Time}", id, DateTime.UtcNow);
            throw new ApplicationException($"A database error occurred while retrieving advisor with ID: {id}.", dbEx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while fetching the advisor with ID: {Id} at {Time}", id, DateTime.UtcNow);
            throw new ApplicationException($"An unexpected error occurred while retrieving advisor with ID: {id}.", ex);
        }
    }
}
