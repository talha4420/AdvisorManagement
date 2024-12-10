using Advisor.Core.Repositories;
using Advisor.Domain.DomainServices;
using Advisor.Domain.Models;
using Advisor.Services.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

public class AdvisorCommandService(
    IDBRepository<AdvisorProfile> advisorRepository,
    IHealthStatusGenerator healthStatusGenerator,
    IModelValidator<AdvisorProfile> modelValidator,
    ILogger<AdvisorCommandService> logger) : IAdvisorCommand
{
    public async Task<AdvisorProfile> CreateAdvisorAsync(AdvisorProfile advisor)
    {
        try
        {
            logger.LogInformation("Starting to create a new advisor at {Time}.", DateTime.UtcNow);
            modelValidator.Validate(advisor);
            advisor.UpdateHealthStatus(healthStatusGenerator);

            //Explicit fix for inmemory DB unique Issue
            if (advisorRepository.GetDBContext().Set<AdvisorProfile>().Any(p => p.SIN == advisor.SIN))
            {
                throw new ValidationException("SIN must be unique. A record with this SIN already exists.");
            }

            var createdAdvisor = await advisorRepository.CreateAsync(advisor);
            logger.LogInformation("Successfully created a new advisor with ID: {AdvisorId} at {Time}.", createdAdvisor.Id, DateTime.UtcNow);
            return createdAdvisor;
        }
        catch (DbException dbEx)
        {
            logger.LogError(dbEx, "Database error while creating a new advisor at {Time}.", DateTime.UtcNow);
            throw new ApplicationException("A database error occurred while creating the advisor.", dbEx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while creating a new advisor at {Time}.", DateTime.UtcNow);
            throw;
        }
    }

    public async Task<AdvisorProfile> UpdateAdvisorAsync(Guid id, AdvisorProfile advisor)
    {
        try
        {
            logger.LogInformation("Starting to update advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            modelValidator.Validate(advisor);
            var updatedAdvisor = await advisorRepository.UpdateAsync(id, advisor);

            if (updatedAdvisor == null)
            {
                logger.LogWarning("Advisor with ID: {AdvisorId} not found at {Time}.", id, DateTime.UtcNow);
            }
            else
            {
                logger.LogInformation("Successfully updated advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            }

            return updatedAdvisor;
        }
        catch (DbException dbEx)
        {
            logger.LogError(dbEx, "Database error while updating advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            throw new ApplicationException($"A database error occurred while updating advisor with ID: {id}.", dbEx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while updating advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            throw;
        }
    }

    public async Task<AdvisorProfile> DeleteAdvisorAsync(Guid id)
    {
        try
        {
            logger.LogInformation("Starting to delete advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            var deletedAdvisor = await advisorRepository.DeleteAsync(id);

            if (deletedAdvisor == null)
            {
                logger.LogWarning("Advisor with ID: {AdvisorId} not found for deletion at {Time}.", id, DateTime.UtcNow);
            }
            else
            {
                logger.LogInformation("Successfully deleted advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            }

            return deletedAdvisor;
        }
        catch (DbException dbEx)
        {
            logger.LogError(dbEx, "Database error while deleting advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            throw new ApplicationException($"A database error occurred while deleting advisor with ID: {id}.", dbEx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while deleting advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
            throw;
        }
    }
}
