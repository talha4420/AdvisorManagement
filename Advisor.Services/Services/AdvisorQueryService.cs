using Advisor.Core.Repositories;
using Advisor.Domain.Models;
using Advisor.Services.Models;

public class AdvisorQueryService : IAdvisorQuery
{
    private readonly IDBRepository<AdvisorProfile> _advisorRepository;

    public AdvisorQueryService(IDBRepository<AdvisorProfile> advisorRepository)
    {
        _advisorRepository = advisorRepository;
    }

    public async Task<IEnumerable<AdvisorProfile>> GetAdvisorsAsync()
    {
        return await _advisorRepository.GetAllAsync();
    }

    public async Task<AdvisorProfile> GetAdvisorAsync(Guid id)
    {
        return await _advisorRepository.GetAsync(id);
    }

}