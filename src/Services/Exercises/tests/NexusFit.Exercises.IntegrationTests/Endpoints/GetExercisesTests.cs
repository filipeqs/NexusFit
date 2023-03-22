using System;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using MongoDB.Driver;
using NexusFit.Exercises.API.Dtos;
using NexusFit.Exercises.API.Entities;
using NexusFit.Exercises.IntegrationTests.Helpers;

namespace NexusFit.Exercises.IntegrationTests.Endpoints;

[Collection("Test Collection")]
public class GetExercisesTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly IMongoCollection<Exercise> _exercisesCollection;
    private readonly Action _resetDatabase;

    public GetExercisesTests(ExerciseApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _exercisesCollection = apiFactory.ExercisesCollection;
        _resetDatabase = apiFactory.ResetDatabaseAsync;
    }

    public Task DisposeAsync()
    {
        _resetDatabase();
        return Task.CompletedTask;
    }

    public List<Exercise> GetTestExercises() => new List<Exercise>
    {
        new Exercise
        {
            Name = "Exercise One",
            Description = "Exercise One Description"
        },
        new Exercise
        {
            Name = "Exercise Two",
            Description = "Exercise Two Description"
        },
    };

    public async Task InitializeAsync()
    {
        var exercises = GetTestExercises();
        await _exercisesCollection.InsertManyAsync(exercises);
    }

    [Fact]
    public async Task GetExercises_ShouldReturn_Exercises()
    {
        var response = await _client.GetAsync(ExercisesRoutes.Get.GetExercises);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var exercises = JsonSerializer.Deserialize<List<ExerciseDetailsDto>>(content, options);

        exercises.Should().NotBeEmpty();
        exercises.Count.Should().Be(2);
    }
}

