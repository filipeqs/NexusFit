using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NexusFit.Auth.API;
using NexusFit.Auth.API.Data;
using NexusFit.Auth.API.Entities;
using Respawn;
using System.Data.Common;
using System.Reflection;
using Testcontainers.MsSql;

namespace NexusFit.Auth.FunctionalTests;

public class AuthApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    public HttpClient HttpClient { get; private set; } = default!;

    private ServiceProvider _serviceProvider = default!;
    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;
    private readonly MsSqlContainer _msSqlContainer =
        new MsSqlBuilder()
        .WithCleanUp(true)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var path = Assembly.GetAssembly(typeof(AuthApiFactory)).Location;

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Tests.json")
            .Build();

        builder.ConfigureLogging((WebHostBuilderContext context, ILoggingBuilder loggingBuilder) =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole(options => options.IncludeScopes = true);
        });

        builder.UseContentRoot(Path.GetDirectoryName(path))
            .UseEnvironment("Tests")
            .ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<IdentityContext>));
                services.AddDbContext<IdentityContext>(options =>
                {
                    options.UseSqlServer(_msSqlContainer.GetConnectionString());
                });

                _serviceProvider = services.BuildServiceProvider();
            });

        base.ConfigureWebHost(builder);
    }

    public async Task UpdateApplicationRoles()
    {
        var roleManager = _serviceProvider.GetService<RoleManager<ApplicationRole>>()!;

        var roles = new List<string> { "Admin", "Student" };
        foreach (var roleName in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
                await roleManager.CreateAsync(new ApplicationRole(roleName));
        }
    }


    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        _dbConnection = new SqlConnection(_msSqlContainer.GetConnectionString());

        HttpClient = CreateClient();
        await InitializeRespawner();
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
        });
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }
}
