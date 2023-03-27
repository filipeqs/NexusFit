namespace NexusFit.BuildingBlocks.Common.EventBus.Events;

public class IntegrationBaseEvent
{
    public IntegrationBaseEvent()
    {
        EventId = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
        Published = false;
    }

    public IntegrationBaseEvent(Guid id, DateTime createDate)
    {
        EventId = id;
        CreationDate = createDate;
        Published = false;
    }

    public Guid EventId { get; private set; }
    public DateTime CreationDate { get; set; }
    public bool Published { get; set; }
}

