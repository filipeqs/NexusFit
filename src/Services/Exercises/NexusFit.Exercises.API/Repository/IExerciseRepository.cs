using NexusFit.Exercises.API.Entities;

namespace NexusFit.Exercises.API.Repository;

public interface IExerciseRepository
{
    Task<IReadOnlyList<Exercise>> GetExercisesAsync();
    Task<Exercise?> GetExerciseAsync(string id);
    Task CreateExerciseAsync(Exercise exercise);
    Task UpdateExerciseAsync(string id, Exercise updatedExercise);
    Task RemoveExerciseAsync(string id);
}
