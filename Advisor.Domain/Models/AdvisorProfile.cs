using System.ComponentModel.DataAnnotations;
using Advisor.Domain.DomainServices;

namespace Advisor.Domain.Models;
public class AdvisorProfile
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; }

    [Required]
    [StringLength(9, MinimumLength = 9)]
    public string SIN { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    [StringLength(10, MinimumLength = 10)]
    public string? PhoneNumber { get; set; }

    public HealthStatus HealthStatus { get; private set; }


    public void UpdateHealthStatus(IHealthStatusGenerator _healthStatusGenerator)
    {
        HealthStatus = _healthStatusGenerator.GenerateHealthStatus();
    }
 
}

