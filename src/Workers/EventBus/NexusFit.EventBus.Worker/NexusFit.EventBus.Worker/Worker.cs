using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NexusFit.EventBus.Worker.Entities;
using NexusFit.EventBus.Worker.Helpers;

namespace NexusFit.EventBus.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMongoCollection<EventBase> _eventsCollection;

    public Worker(ILogger<Worker> logger, IOptions<EventsDatabaseSettings> databaseSettings,
        IPublishEndpoint publishEndpoint, IConfiguration config)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        var mongoClient = new MongoClient(config.GetConnectionString("MongoConnection"));
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _eventsCollection = mongoDatabase.GetCollection<EventBase>(databaseSettings.Value.CollectionName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var events = await _eventsCollection.Find(q => q.Published == false).ToListAsync();
            foreach (var eventdata in events)
            {
                await _publishEndpoint.Publish(eventdata, stoppingToken);
                eventdata.Published = true;
                await _eventsCollection.ReplaceOneAsync(
                    q => q.Id == eventdata.Id, 
                    eventdata, cancellationToken: stoppingToken);
            }

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}