using Microsoft.AspNetCore.Mvc;
using NexusFit.Exercises.API.Repository;
using NexusFit.Exercises.API.Entities;

namespace NexusFit.Exercises.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseRepository _repository;

    public ExerciseController(IExerciseRepository repo)
    {
        _repository = repo;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Exercise>>> GetExercises() => 
        Ok(await _repository.GetExercisesAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Exercise>> GetExerciseById(string id)
    {
        var exercise = await _repository.GetExerciseAsync(id);

        if (exercise is null)
            return NotFound();

        return Ok(exercise);
    }

    [HttpPost]
    public async Task<IActionResult> CreateExercise(Exercise exercise) 
    {
        await _repository.CreateExerciseAsync(exercise);

        return CreatedAtAction(nameof(GetExerciseById), new { id = exercise.Id }, exercise);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExercise(string id, Exercise updatedExercise) 
    {
        var exercise = await _repository.GetExerciseAsync(id);

        if (exercise is null)
            return NotFound();

        await _repository.UpdateExerciseAsync(id, updatedExercise);

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteExercise(string id)
    {
        var exercise = await _repository.GetExerciseAsync(id);

        if (exercise is null)
            return NotFound();

        await _repository.RemoveExerciseAsync(id);

        return Ok();
    }
}