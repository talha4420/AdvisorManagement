
namespace Advisor.Domain.DomainServices;
public enum HealthStatus
{
    Green,
    Yellow,
    Red
}
public interface IHealthStatusGenerator
{
    HealthStatus GenerateHealthStatus();
}