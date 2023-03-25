using System;
using System.Data.Common;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NexusFit.Exercises.API;
using NexusFit.Exercises.API.Entities;
using NexusFit.Exercises.API.Helpers;
using NexusFit.Exercises.IntegrationTests.Authentication;
using Respawn;
using Testcontainers.MongoDb;

namespace NexusFit.Exercises.IntegrationTests;

public class ExerciseApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    public HttpClient HttpClient { get; private set; } = default!;
    public IMongoCollection<Exercise> ExercisesCollection { get; set; } = default!;

    private MongoClient _mongoClient = default!;
    private readonly MongoDbContainer _mongoContainer =
			new MongoDbBuilder()
			.WithCleanUp(true)
			.Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var path = Assembly.GetAssembly(typeof(ExerciseApiFactory)).Location;

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
            var mongoConnection = _mongoContainer.GetConnectionString();
            services.Configure<ExercisesDatabaseSettings>((dbSettings) =>
            {
                dbSettings.CollectionName = configuration
                    .GetSection("ExercisesDatabaseSettings")
                    .GetValue<string>("CollectionName");
                dbSettings.DatabaseName = configuration
                    .GetSection("ExercisesDatabaseSettings")
                    .GetValue<string>("DatabaseName");
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            }).AddScheme<AuthenticationSchemeOptions, AuthenticationTestHandler>("Test", opt => { });
        });

        base.ConfigureWebHost(builder);
    }

    public void ResetDatabaseAsync()
    {
        _mongoClient.DropDatabase("ExerciseDB");
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
        _mongoClient = new MongoClient(_mongoContainer.GetConnectionString());
        var mongoDatabase = _mongoClient.GetDatabase("ExerciseDB");
        ExercisesCollection = mongoDatabase.GetCollection<Exercise>("Exercises");

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await _mongoContainer.StopAsync();
    }
}

