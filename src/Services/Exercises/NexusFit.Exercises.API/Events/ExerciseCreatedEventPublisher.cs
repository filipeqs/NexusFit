using System;
using MassTransit;
using NexusFit.BuildingBlocks.Common.EventBus.Events;
using NexusFit.Exercises.API.Entities;

namespace NexusFit.Exercises.API.Events;

public class ExerciseCreatedEventPublisher : IExerciseCreatedEventPublisher
{
	private readonly IPublishEndpoint _publishEndpoint;

    public ExerciseCreatedEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Publish(Exercise exercise)
    {
        var eventMessage = new ExerciseCreatedEvent(exercise.Id, exercise.Name, exercise.Description);

        await _publishEndpoint.Publish(eventMessage);
    }
}

