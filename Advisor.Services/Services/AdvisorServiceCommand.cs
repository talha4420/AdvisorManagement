using Advisor.Core.Repositories;
using Advisor.Domain.DomainServices;
using Advisor.Domain.Models;
using Advisor.Services.Models;
using Microsoft.EntityFrameworkCore;
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
        logger.LogInformation("Starting to create a new advisor at {Time}.", DateTime.UtcNow);
        modelValidator.Validate(advisor);
        advisor.UpdateHealthStatus(healthStatusGenerator);

        //Explicit fix for inmemory DB unique Issue
        var existingAdvisor = advisorRepository.GetAllQueryable().Where(p => p.SIN == advisor.SIN);
        if (await existingAdvisor.AnyAsync())
        {
            logger.LogWarning("Advisor with SIN: {SIN} already exists at {Time}.", advisor.SIN, DateTime.UtcNow);
            throw new ValidationException("SIN must be unique. A record with this SIN already exists.");
        }
        
        var createdAdvisor = await advisorRepository.CreateAsync(advisor);
        logger.LogInformation("Successfully created a new advisor with ID: {AdvisorId} at {Time}.", createdAdvisor.Id, DateTime.UtcNow);
        return createdAdvisor;
    }

    public async Task<AdvisorProfile> UpdateAdvisorAsync(Guid id, AdvisorProfile advisor)
    {
        logger.LogInformation("Starting to update advisor with ID: {AdvisorId} at {Time}.", id, DateTime.UtcNow);
        modelValidator.Validate(advisor);

        if(advisor.Id != id)
        {
            advisor.Id = id;
        }
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

    public async Task<AdvisorProfile> DeleteAdvisorAsync(Guid id)
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
}
