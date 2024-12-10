using Advisor.Domain.DomainServices;

namespace Advisor.Domain.Models
{
    public class AdvisorProfile
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string SIN { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public HealthStatus HealthStatus { get; private set; }

        public void UpdateHealthStatus(IHealthStatusGenerator _healthStatusGenerator)
        {
            HealthStatus = _healthStatusGenerator.GenerateHealthStatus();
        }
    }
}
