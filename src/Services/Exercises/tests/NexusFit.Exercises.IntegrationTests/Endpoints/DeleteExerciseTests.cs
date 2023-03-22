using System;
using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using MongoDB.Driver;
using NexusFit.Exercises.API.Dtos;
using NexusFit.Exercises.API.Entities;
using NexusFit.Exercises.IntegrationTests.Helpers;

namespace NexusFit.Exercises.IntegrationTests.Endpoints
{
    [Collection("Test Collection")]
    public class DeleteExerciseTests : IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly IMongoCollection<Exercise> _exercisesCollection;
        private readonly Action _resetDatabase;
        private Exercise _exercise = default!;

        public DeleteExerciseTests(ExerciseApiFactory apiFactory)
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
        public async Task DeleteExercise_ShouldReturn_Ok()
        {
            var response = await _client.DeleteAsync(ExercisesRoutes.Delete.DeleteExercise(_exercise.Id));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteUnexistingExercise_ShouldReturn_NotFound()
        {
            var response = await _client.DeleteAsync(ExercisesRoutes.Delete.DeleteExercise("5418fb383b229abb26395e6f"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}

