using Advisor.Domain.Models;
using Advisor.Services.Models;
using Asp.Versioning.Conventions;
using AutoMapper;
using Advisor.API.DTOs;
using Microsoft.AspNetCore.Mvc;

public static class AdvisorApi
{
    public static void MapAdvisorApiV1(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v{version:apiVersion}/advisors", GetAdvisors)
            .WithName("GetAdvisors")
            .MapToApiVersion(1.0);

        app.MapGet("/api/v{version:apiVersion}/advisors/{id}", GetAdvisor)
            .WithName("GetAdvisor")
            .MapToApiVersion(1.0);

        app.MapPost("/api/v{version:apiVersion}/advisors", CreateAdvisor)
            .WithName("CreateAdvisor")
            .MapToApiVersion(1.0);

        app.MapPut("/api/v{version:apiVersion}/advisors/{id}", UpdateAdvisor)
            .WithName("UpdateAdvisor")
            .MapToApiVersion(1.0);

        app.MapDelete("/api/v{version:apiVersion}/advisors/{id}", DeleteAdvisor)
            .WithName("DeleteAdvisor")
            .MapToApiVersion(1.0);
    }

    public static async Task<IResult> GetAdvisors(
        [FromServices] IAdvisorQuery service,
        [FromServices] IMapper mapper)
    {
        var advisors = await service.GetAdvisorsAsync();
        var responseDtos = mapper.Map<IEnumerable<AdvisorProfileResponseDto>>(advisors);
        return Results.Ok(responseDtos);
    }

    public static async Task<IResult> GetAdvisor(
        Guid id,
        [FromServices] IAdvisorQuery service,
        [FromServices] IMapper mapper)
    {
        var advisor = await service.GetAdvisorAsync(id);

        if (advisor == null)
        {
            return Results.NotFound( $"Advisor with ID {id} not found." );
        }

        var responseDto = mapper.Map<AdvisorProfileResponseDto>(advisor);
        return Results.Ok(responseDto);
    }

    public static async Task<IResult> CreateAdvisor(
        [FromBody] AdvisorProfileRequestDto requestDto,
        [FromServices] IAdvisorCommand service,
        [FromServices] IMapper mapper)
    {
        if (requestDto == null)
        {
            return Results.BadRequest(new { Message = "Invalid request data." });
        }

        var advisor = mapper.Map<AdvisorProfile>(requestDto);
        var createdAdvisor = await service.CreateAdvisorAsync(advisor);

        var responseDto = mapper.Map<AdvisorProfileResponseDto>(createdAdvisor);
        return Results.Created($"/api/v1/advisors/{responseDto.Id}", responseDto);
    }

    public static async Task<IResult> UpdateAdvisor(
        Guid id,
        [FromBody] AdvisorProfileRequestDto requestDto,
        [FromServices] IAdvisorCommand service,
        [FromServices] IMapper mapper)
    {
        if (requestDto == null)
        {
            return Results.BadRequest(new { Message = "Invalid request data." });
        }

        var advisor = mapper.Map<AdvisorProfile>(requestDto);
        var updatedAdvisor = await service.UpdateAdvisorAsync(id, advisor);

        if (updatedAdvisor == null)
        {
            return Results.NotFound( $"Advisor with ID {id} not found." );
        }

        var responseDto = mapper.Map<AdvisorProfileResponseDto>(updatedAdvisor);
        return Results.Ok(responseDto);
    }

    public static async Task<IResult> DeleteAdvisor(
        Guid id,
        [FromServices] IAdvisorCommand service,
        [FromServices] IMapper mapper)
    {
        var deletedAdvisor = await service.DeleteAdvisorAsync(id);

        if (deletedAdvisor == null)
        {
            return Results.NotFound( $"Advisor with ID {id} not found." );
        }

        var responseDto = mapper.Map<AdvisorProfileResponseDto>(deletedAdvisor);
        return Results.Ok(responseDto);
    }
}
