using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NexusFit.Exercises.API.Entities;
using NexusFit.Exercises.API.Helpers;

namespace NexusFit.Exercises.API.Repository;

public class ExerciseRepository : IExerciseRepository
{
    private readonly IMongoCollection<Exercise> _exercisesCollection;

	public ExerciseRepository(IOptions<DatabaseSettings> databaseSettings)
	{
		var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
		var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
		_exercisesCollection = mongoDatabase.GetCollection<Exercise>(databaseSettings.Value.CollectionName);
	}

	public async Task<IReadOnlyList<Exercise>> GetExercisesAsync() => 
		await _exercisesCollection.Find(_ => true).ToListAsync();

	public async Task<Exercise?> GetExerciseAsync(string id) =>
		await _exercisesCollection.Find(q => q.Id == id).FirstOrDefaultAsync();

	public async Task CreateExerciseAsync(Exercise exercise) =>
		await _exercisesCollection.InsertOneAsync(exercise);

	public async Task UpdateExerciseAsync(string id, Exercise updatedExercise) =>
		await _exercisesCollection.ReplaceOneAsync(q => q.Id == id, updatedExercise);

	public async Task RemoveExerciseAsync(string id) =>
		await _exercisesCollection.DeleteOneAsync(q => q.Id == id);
}
