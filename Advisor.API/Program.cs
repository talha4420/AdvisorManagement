using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Advisor.Core.DBContexts;
using Advisor.Domain.DomainServices;
using Advisor.Domain.Models;
using Advisor.Core.Repositories;
using Advisor.Services.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure EF Core to use an in-memory database
builder.Services.AddDbContext<AdvisorDBContext>(options =>
    options.UseInMemoryDatabase("AdvisorDB"));

// Register services
builder.Services.AddScoped<IDBRepository<AdvisorProfile>, DBRepository<AdvisorProfile, AdvisorDBContext>>();
builder.Services.AddScoped<IAdvisorCommand, AdvisorCommandService>();
builder.Services.AddScoped<IAdvisorQuery, AdvisorQueryService>();
builder.Services.AddScoped<IHealthStatusGenerator, HealthStatusGeneratorService>();

// Configure API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options=>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.NewVersionedApi("Advisor API")
    .MapAdvisorApiV1();

app.Run();

public partial class Program { }