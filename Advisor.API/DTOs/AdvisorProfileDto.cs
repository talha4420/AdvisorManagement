namespace Advisor.API.DTOs;

public class AdvisorProfileRequestDto
{
    public string FullName { get; set; }
    public string SIN { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
}

public class AdvisorProfileResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string? Address { get; set; }
    public string SIN { get; set; }
    public string PhoneNumber { get; set; }
    public string HealthStatus { get; private set; }
}
