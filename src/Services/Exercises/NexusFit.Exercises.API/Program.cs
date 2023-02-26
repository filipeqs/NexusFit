using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NexusFit.BuildingBlocks.ExceptionHandling.Extensions;
using NexusFit.BuildingBlocks.ExceptionHandling.Middleware;
using NexusFit.BuildingBlocks.Logging.Middleware;
using NexusFit.Exercises.API.Helpers;
using NexusFit.Exercises.API.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddMongoDb(
        builder.Configuration.GetSection("DatabaseSettings").GetValue<string>("ConnectionString"),
        name: "exercisedb-check",
        tags: new string[] { "exercisedb", "mongodb" })
    .AddElasticsearch(
        builder.Configuration.GetConnectionString("LogConnection"),
        name: "logdb-check",
        tags: new string[] { "logdb", "elasticsearch" });

builder.Services.AddAuthentication("Bearer")
    .AddIdentityServerAuthentication("Bearer", options =>
    {
        options.ApiName = builder.Configuration.GetValue<string>("IdentityServer:Resource");;
        options.Authority = builder.Configuration.GetValue<string>("IdentityServer:Url");
        options.RequireHttpsMetadata = false;
    });

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddExceptionHandlingServices();

builder.Services.AddCors(p => p.AddPolicy("clientapp", builder =>
{
    builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/api/exercises/hc", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("clientapp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
