using System;
namespace NexusFit.BuildingBlocks.Common.EventBus.Events;

public class IntegrationBaseEvent
{
    public IntegrationBaseEvent()
    {
        EventId = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    public IntegrationBaseEvent(Guid id, DateTime createDate)
    {
        EventId = id;
        CreationDate = createDate;
    }

    public Guid EventId { get; private set; }
    public DateTime CreationDate { get; set; }
}

