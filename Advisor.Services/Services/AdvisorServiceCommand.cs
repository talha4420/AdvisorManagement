using Advisor.Core.Repositories;
using Advisor.Domain.DomainServices;
using Advisor.Domain.Models;
using Advisor.Services.Models;
using Microsoft.Extensions.Logging;
using System.Data.Common;

public class AdvisorCommandService : IAdvisorCommand
{
    private readonly IDBRepository<AdvisorProfile> _advisorRepository;
    private readonly IHealthStatusGenerator _healthStatusGenerator;
    private readonly ILogger<AdvisorCommandService> _logger;

    public AdvisorCommandService(
        IDBRepository<AdvisorProfile> advisorRepository, 
        IHealthStatusGenerator healthStatusGenerator,
        ILogger<AdvisorCommandService> logger)
    {
        _advisorRepository = advisorRepository;
        _healthStatusGenerator = healthStatusGenerator;
        _logger = logger;
    }

    public async Task<AdvisorProfile> CreateAdvisorAsync(AdvisorProfile advisor)
    {
        try
        {
            _logger.LogInformation("Starting to create a new advisor at {Time}.", DateTime.UtcNow);
            advisor.UpdateHealthStatus(_healthStatusGenerator);
            var createdAdvisor = await _advisorRepository.CreateAsync(advisor);
            _logger.LogInformation("Successfully created a new advisor with ID: {AdvisorId} at {Time}.", createdAdvisor.Id, DateTime.UtcNow);
            return createdAdvisor;
        }
        catch (DbException dbEx)
        {
            _logger.LogError(dbEx, "Database error while creating a new advisor at {Time}.", DateTime.UtcNow);
            throw new ApplicationException("A database error occurred while creating the advisor.", dbEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating a new advisor at {Time}.", DateTime.UtcNow);
            throw;
        }
    }

    public async Task<AdvisorProfile> UpdateAdvisorAsync(Guid id, AdvisorProfile advisor)
    {
        try
        {
            _logger.LogInformation("Starting to update advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            var updatedAdvisor = await _advisorRepository.UpdateAsync(id, advisor);

            if (updatedAdvisor == null)
            {
                _logger.LogWarning("Advisor with ID: {AdvisorId} not found at {Time}.", id, DateTime.UtcNow);
            }
            else
            {
                _logger.LogInformation("Successfully updated advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            }

            return updatedAdvisor;
        }
        catch (DbException dbEx)
        {
            _logger.LogError(dbEx, "Database error while updating advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            throw new ApplicationException($"A database error occurred while updating advisor with ID: {id}.", dbEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            throw;
        }
    }

    public async Task<AdvisorProfile> DeleteAdvisorAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Starting to delete advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            var deletedAdvisor = await _advisorRepository.DeleteAsync(id);

            if (deletedAdvisor == null)
            {
                _logger.LogWarning("Advisor with ID: {AdvisorId} not found for deletion at {Time}.", id, DateTime.UtcNow);
            }
            else
            {
                _logger.LogInformation("Successfully deleted advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            }

            return deletedAdvisor;
        }
        catch (DbException dbEx)
        {
            _logger.LogError(dbEx, "Database error while deleting advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            throw new ApplicationException($"A database error occurred while deleting advisor with ID: {id}.", dbEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while deleting advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            throw;
        }
    }
}
