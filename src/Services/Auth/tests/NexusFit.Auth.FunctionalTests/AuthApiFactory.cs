using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NexusFit.Auth.API;
using NexusFit.Auth.API.Data;
using NexusFit.Auth.API.Helpers;
using Respawn;
using System.Data.Common;
using System.Reflection;

namespace NexusFit.Auth.FunctionalTests;

public class AuthApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    public HttpClient HttpClient { get; private set; }
    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;
    private readonly TestcontainerDatabase _dbContainer =
        new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration()
            {
                Password = "localdevpassword#123",
            })
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

    private readonly IContainer _elasticSearchContainer =
        new ContainerBuilder()
            .WithImage("docker.elastic.co/elasticsearch/elasticsearch:7.9.2")
            .WithExposedPort(9200)
            .WithCleanUp(true)
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var path = Assembly.GetAssembly(typeof(AuthApiFactory)).Location;

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Tests.json")
            .Build();

        builder.UseContentRoot(Path.GetDirectoryName(path))
            .UseEnvironment("Tests")
            .ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<IdentityContext>));
                services.AddDbContext<IdentityContext>(options =>
                {
                    options.UseSqlServer($"{_dbContainer.ConnectionString}TrustServerCertificate=true;");
                });

                services.AddSingleton<IConfiguration>(configuration);

                services.Configure<IdentityServerSettings>(options => 
                    configuration.GetSection("IdentityServer").Bind(options));
            });

        base.ConfigureWebHost(builder);
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _dbConnection = new SqlConnection($"{_dbContainer.ConnectionString}TrustServerCertificate=true;");

        await _elasticSearchContainer.StartAsync();

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
        await _dbContainer.StopAsync();
        await _elasticSearchContainer.StartAsync();
    }
}
