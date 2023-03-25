using MassTransit;
using NexusFit.BuildingBlocks.Common.EventBus.Events;

namespace NexusFit.Playground.API.Events.Consumers;

public class ExerciseCreatedEventConsumer : IConsumer<ExerciseCreatedEvent>
{
    public Task Consume(ConsumeContext<ExerciseCreatedEvent> context)
    {
        throw new NotImplementedException();
    }
}
