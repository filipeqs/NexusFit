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
public class UpdateExerciseTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly IMongoCollection<Exercise> _exercisesCollection;
    private readonly Action _resetDatabase;
    private Exercise _exercise = default!;

    public UpdateExerciseTests(ExerciseApiFactory apiFactory)
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
        _exercise = await _exercisesCollection.Find(_ => true).FirstOrDefaultAsync();
    }

    [Fact]
    public async Task UpdateExercise_ShouldReturn_Ok()
    {
        var exerciseUpdateDto = new ExerciseUpdateDto
        {
            Id = _exercise.Id,
            Name = "Updated Name",
            Description = "Updated Description"
        };
        var request = new StringContent(JsonSerializer.Serialize(exerciseUpdateDto), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync(ExercisesRoutes.Put.UpdateExercise, request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseGetExercise = await _client.GetAsync(ExercisesRoutes.Get.GetExerciseById(exerciseUpdateDto.Id));
        responseGetExercise.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await responseGetExercise.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var exercise = JsonSerializer.Deserialize<ExerciseDetailsDto>(content, options);

        exercise.Should().NotBeNull();
        exercise.Name.Should().Be(exerciseUpdateDto.Name);
        exercise.Description.Should().Be(exerciseUpdateDto.Description);
    }

    [Fact]
    public async Task UpdateUnexistingExercise_ShouldReturn_NotFound()
    {
        var exerciseUpdateDto = new ExerciseUpdateDto
        {
            Id = "5418fb383b229abb26395e6f",
            Name = "Updated Name",
            Description = "Updated Description"
        };
        var request = new StringContent(JsonSerializer.Serialize(exerciseUpdateDto), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync(ExercisesRoutes.Put.UpdateExercise, request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

