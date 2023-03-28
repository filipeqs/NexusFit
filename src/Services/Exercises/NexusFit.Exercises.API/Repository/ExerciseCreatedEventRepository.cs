using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NexusFit.BuildingBlocks.Common.EventBus.Events;
using NexusFit.Exercises.API.Helpers;

namespace NexusFit.Exercises.API.Repository;

public class ExerciseCreatedEventRepository : IExerciseCreatedEventRepository
{
    private readonly IMongoCollection<ExerciseCreatedEvent> _mongoCollection;
    public ExerciseCreatedEventRepository(IOptions<EventsDatabaseSettings> eventsSettings, IConfiguration config)
    {
        var mongoClient = new MongoClient(config.GetConnectionString("MongoConnection"));
        var mongoDatabase = mongoClient.GetDatabase(eventsSettings.Value.DatabaseName);
        _mongoCollection = mongoDatabase.GetCollection<ExerciseCreatedEvent>(eventsSettings.Value.CollectionName);
    }

    public async Task AddExerciseCreatedEvent(ExerciseCreatedEvent exercisecreate) =>
        await _mongoCollection.InsertOneAsync(exercisecreate);
}
