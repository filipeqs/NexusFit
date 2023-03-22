using System;
using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using MongoDB.Driver;
using NexusFit.Exercises.API.Dtos;
using NexusFit.Exercises.API.Entities;
using NexusFit.Exercises.IntegrationTests.Helpers;

namespace NexusFit.Exercises.IntegrationTests.Endpoints;

[Collection("Test Collection")]
public class CreateExerciseTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly IMongoCollection<Exercise> _exercisesCollection;
    private readonly Action _resetDatabase;

    public CreateExerciseTests(ExerciseApiFactory apiFactory)
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

    public Task InitializeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateExercise_ShouldReturn_Exercise()
    {
        var exercise = new Exercise
        {
            Name = "Exercise One",
            Description = "Exercise One Description"
        };
        var request = new StringContent(JsonSerializer.Serialize(exercise), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(ExercisesRoutes.Post.CreateExercise, request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var exerciseResponse = JsonSerializer.Deserialize<ExerciseDetailsDto>(content, options);

        exerciseResponse.Should().NotBeNull();
        exerciseResponse.Name.Should().Be(exercise.Name);
        exerciseResponse.Description.Should().Be(exercise.Description);
    }
}

