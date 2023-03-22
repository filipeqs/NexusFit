using System;

namespace NexusFit.Exercises.IntegrationTests.Helpers;

public static class ExercisesRoutes
{
	public static class Get
	{
		public const string GetExercises = "api/exercises";
		public static string GetExerciseById(string id) => $"api/exercises/{id}";
	}

	public static class Post
	{
		public const string CreateExercise = "api/exercises";
	}

	public static class Put
	{
		public const string UpdateExercise = "api/exercises";
	}

	public static class Delete
	{
		public static string DeleteExercise(string id) => $"api/exercises/{id}";
	}
}

