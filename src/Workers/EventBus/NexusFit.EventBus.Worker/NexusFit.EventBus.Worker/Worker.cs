using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NexusFit.BuildingBlocks.Common.EventBus.Events;
using NexusFit.EventBus.Worker.Helpers;

namespace NexusFit.EventBus.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBus _publishEndpoint;
    private readonly IMongoCollection<ExerciseCreatedEvent> _eventsCollection;

    public Worker(ILogger<Worker> logger, IOptions<EventsDatabaseSettings> databaseSettings,
        IBus publishEndpoint, IConfiguration config)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        var mongoClient = new MongoClient(config.GetConnectionString("MongoConnection"));
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _eventsCollection = mongoDatabase.GetCollection<ExerciseCreatedEvent>(databaseSettings.Value.CollectionName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var events = await _eventsCollection.Find(q => q.Published == false).ToListAsync();
            foreach (var exerciseCreatedEvent in events)
            {
                await _publishEndpoint.Publish(exerciseCreatedEvent, stoppingToken);
                exerciseCreatedEvent.Published = true;
                await _eventsCollection.ReplaceOneAsync(
                    q => q.Id == exerciseCreatedEvent.Id, 
                    exerciseCreatedEvent, cancellationToken: stoppingToken);
            }

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}