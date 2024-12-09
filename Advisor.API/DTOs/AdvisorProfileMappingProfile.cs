using AutoMapper;
using Advisor.Domain.Models;
using Advisor.API.DTOs;

public class AdvisorProfileMappingProfile : Profile
{
    public AdvisorProfileMappingProfile()
    {
        // Map Request DTO to Domain Model
        CreateMap<AdvisorProfileRequestDto, AdvisorProfile>();

        // Map Domain Model to Response DTO with masking
        CreateMap<AdvisorProfile, AdvisorProfileResponseDto>()
            .ForMember(dest => dest.SIN, opt => opt.MapFrom(src => MaskSIN(src.SIN)))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => MaskPhoneNumber(src.PhoneNumber)));
    }

    private static string MaskSIN(string sin)
    {
        if (string.IsNullOrEmpty(sin) || sin.Length != 9) return "*********";
        return $"***-**-{sin[^4..]}";
    }

    private static string MaskPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length != 10) return "***-***-****";
        return $"***-***-{phoneNumber[^4..]}";
    }
}
