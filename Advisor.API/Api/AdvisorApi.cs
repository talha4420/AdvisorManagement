using Advisor.Domain.Models;
using Advisor.Services.Models;
using Asp.Versioning.Conventions;

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

    public static async Task<IResult> GetAdvisors(IAdvisorQuery service)
    {
        var advisors = await service.GetAdvisorsAsync();
        return Results.Ok(advisors);
    }

    public static async Task<IResult> GetAdvisor(Guid id, IAdvisorQuery service)
    {
        var advisor = await service.GetAdvisorAsync(id);

        if (advisor == null)
        {
            return Results.NotFound($"Advisor with ID {id} not found.");
        }

        return Results.Ok(advisor);
    }

    public static async Task<IResult> CreateAdvisor(AdvisorProfile advisor, IAdvisorCommand service)
    {
        var createdAdvisor = await service.CreateAdvisorAsync(advisor);
        return Results.Created($"/api/v1/advisors/{createdAdvisor.Id}", createdAdvisor);
    }

    public static async Task<IResult> UpdateAdvisor(Guid id, AdvisorProfile advisor, IAdvisorCommand service)
    {
        var updatedAdvisor = await service.UpdateAdvisorAsync(id, advisor);

        if (updatedAdvisor == null)
        {
            return Results.NotFound($"Advisor with ID {id} not found.");
        }

        return Results.Ok(updatedAdvisor);
    }

    public static async Task<IResult> DeleteAdvisor(Guid id, IAdvisorCommand service)
    {
        var deletedAdvisor = await service.DeleteAdvisorAsync(id);

        if (deletedAdvisor == null)
        {
            return Results.NotFound($"Advisor with ID {id} not found.");
        }

        return Results.Ok(deletedAdvisor);
    }
}