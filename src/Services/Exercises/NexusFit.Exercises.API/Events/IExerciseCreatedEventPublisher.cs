using System;
using NexusFit.Exercises.API.Entities;

namespace NexusFit.Exercises.API.Events;

public interface IExerciseCreatedEventPublisher
{
    Task Publish(Exercise exercise);
}

