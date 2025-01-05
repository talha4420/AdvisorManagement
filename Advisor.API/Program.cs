using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Advisor.Core.DBContexts;
using Advisor.Domain.Models;
using Advisor.Core.Repositories;
using Advisor.Services.Models;
using Advisor.API.ExceptionHandling;
using Advisor.Domain.DomainServices;
using Advisor.Core.Pagination;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Add ProblemDetails and Exception Handlers
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<ArgumentNullExceptionHandler>();
builder.Services.AddExceptionHandler<DatabaseExceptionHandler>();
builder.Services.AddExceptionHandler<GeneralExceptionHandler>();
builder.Services.AddProblemDetails();

// Add services to the container.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Configure EF Core to use an in-memory database
builder.Services.AddDbContext<AdvisorDBContext>(options =>
    options.UseInMemoryDatabase("AdvisorDB"));



// Register repositories and services
builder.Services.AddScoped<IDBRepository<AdvisorProfile>, DBRepository<AdvisorProfile, AdvisorDBContext>>();
builder.Services.AddScoped<IPaginator, PaginationService>();

builder.Services.AddScoped<IAdvisorCommand, AdvisorCommandService>();
builder.Services.AddScoped<IAdvisorQuery, AdvisorQueryService>();
builder.Services.AddScoped<IHealthStatusGenerator, HealthStatusGeneratorService>();
builder.Services.AddScoped<IModelValidator<AdvisorProfile>, AdvisorProfileValidator>();

// Configure API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTheme(ScalarTheme.DeepSpace);
        options.WithTitle("Advisor API");
        options.WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
    });
    //app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Advisor.API | v1");
    });

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AdvisorDBContext>();
        dbContext.Database.EnsureCreated();
    }
}

app.UseHttpsRedirection();
app.UseExceptionHandler(); 

app.NewVersionedApi("Advisor API")
    .MapAdvisorApiV1();

app.Run();

