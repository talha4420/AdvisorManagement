
namespace Advisor.Domain.DomainServices;
public class HealthStatusGeneratorService : IHealthStatusGenerator
{
    public HealthStatus GenerateHealthStatus()
    {
        var random = new Random();
        var value = random.Next(0, 100);

        if (value < 60)
        {
            return HealthStatus.Green; 
        }
        else if (value < 80)
        {
            return HealthStatus.Yellow; 
        }
        else
        {
            return HealthStatus.Red; 
        }
    }
}