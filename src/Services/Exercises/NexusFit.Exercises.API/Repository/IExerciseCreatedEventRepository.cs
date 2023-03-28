using NexusFit.BuildingBlocks.Common.EventBus.Events;

namespace NexusFit.Exercises.API.Repository;

public interface IExerciseCreatedEventRepository
{
    Task AddExerciseCreatedEvent(ExerciseCreatedEvent exercisecreate);
}
