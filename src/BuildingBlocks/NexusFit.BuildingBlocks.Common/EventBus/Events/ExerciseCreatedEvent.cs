using System;
namespace NexusFit.BuildingBlocks.Common.EventBus.Events;

public class ExerciseCreatedEvent : IntegrationBaseEvent
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
