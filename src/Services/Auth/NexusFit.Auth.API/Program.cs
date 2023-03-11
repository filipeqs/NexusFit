using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using NexusFit.Auth.API.Data;
using NexusFit.Auth.API.Entities;
using NexusFit.Auth.API.Extensions;
using NexusFit.Auth.API.services;
using NexusFit.BuildingBlocks.ExceptionHandling.Extensions;
using NexusFit.BuildingBlocks.ExceptionHandling.Middleware;
using NexusFit.BuildingBlocks.Extensions;
using NexusFit.BuildingBlocks.Logging;
using NexusFit.BuildingBlocks.Logging.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth.API", Version = "v1" })
);

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "authdb-check",
        tags: new string[] { "authdb", "sqlserver" })
    .AddElasticsearch(
        builder.Configuration.GetConnectionString("LogConnection"),
        name: "logdb-check",
        tags: new string[] { "logdb", "elasticsearch" });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddDbContext<IdentityContext>(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentityServices(
    builder.Configuration);

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

app.MapHealthChecks("/api/auth/hc", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MigrateDbContext<IdentityContext>((context, services) =>
{
    var env = services.GetService<IHostEnvironment>();
    var signInManager = services.GetRequiredService<SignInManager<ApplicationUser>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

    new IdentityContextSeed()
        .SeedAsync(signInManager, userManager, roleManager, env)
        .Wait();
});

app.UseCors("clientapp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
