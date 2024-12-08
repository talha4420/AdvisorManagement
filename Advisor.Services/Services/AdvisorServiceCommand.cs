using Advisor.Core.Repositories;
using Advisor.Domain.DomainServices;
using Advisor.Domain.Models;
using Advisor.Services.Models;

public class AdvisorCommandService : IAdvisorCommand
{
    private readonly IDBRepository<AdvisorProfile> _advisorRepository;
    private readonly IHealthStatusGenerator _healthStatusGenerator;

    public AdvisorCommandService(IDBRepository<AdvisorProfile> advisorRepository, IHealthStatusGenerator healthStatusGenerator)
    {
        _advisorRepository = advisorRepository;
        _healthStatusGenerator = healthStatusGenerator;
    }

    public async Task<AdvisorProfile> CreateAdvisorAsync(AdvisorProfile advisor)
    {
        advisor.UpdateHealthStatus(_healthStatusGenerator);
        return await _advisorRepository.CreateAsync(advisor);
    }

    public async Task<AdvisorProfile> UpdateAdvisorAsync(Guid Id, AdvisorProfile advisor)
    {
        return await _advisorRepository.UpdateAsync(Id, advisor);
    }

    public async Task<AdvisorProfile> DeleteAdvisorAsync(Guid Id)
    {
        return await _advisorRepository.DeleteAsync(Id);
    }
}