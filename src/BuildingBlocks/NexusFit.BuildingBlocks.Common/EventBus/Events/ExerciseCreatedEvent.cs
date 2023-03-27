namespace NexusFit.BuildingBlocks.Common.EventBus.Events;

public class ExerciseCreatedEvent : IntegrationBaseEvent
{
    public ExerciseCreatedEvent(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
