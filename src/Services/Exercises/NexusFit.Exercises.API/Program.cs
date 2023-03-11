using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(opt => 
{
    opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
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
