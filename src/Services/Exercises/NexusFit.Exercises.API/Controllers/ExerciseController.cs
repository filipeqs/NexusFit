using Microsoft.AspNetCore.Mvc;
using NexusFit.Exercises.API.Repository;
using NexusFit.Exercises.API.Entities;
using NexusFit.Exercises.API.Dtos;
using AutoMapper;

namespace NexusFit.Exercises.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseRepository _repository;
    private readonly IMapper _mapper;

    public ExerciseController(IExerciseRepository repo, IMapper mapper)
    {
        _repository = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Exercise>>> GetExercises()
    {
        var exercises = await _repository.GetExercisesAsync();

        var exercisesDetails = _mapper.Map<List<ExerciseDetailsDto>>(exercises);

        return Ok(exercisesDetails);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExerciseDetailsDto>> GetExerciseById(string id)
    {
        var exercise = await _repository.GetExerciseAsync(id);

        if (exercise is null)
            return NotFound();

        var exerciseDetails = _mapper.Map<ExerciseDetailsDto>(exercise);

        return Ok(exerciseDetails);
    }

    [HttpPost]
    public async Task<IActionResult> CreateExercise(ExerciseCreateDto exerciseCreateDto) 
    {
        var exercise = _mapper.Map<Exercise>(exerciseCreateDto);

        await _repository.CreateExerciseAsync(exercise);

        var exerciseDetails = _mapper.Map<ExerciseDetailsDto>(exercise);

        return CreatedAtAction(nameof(GetExerciseById), new { id = exerciseDetails.Id }, exerciseDetails);
    }

    [HttpPut()]
    public async Task<IActionResult> UpdateExercise(ExerciseUpdateDto updatedExercise) 
    {
        var exercise = await _repository.GetExerciseAsync(updatedExercise.Id);

        if (exercise is null)
            return NotFound();

        exercise = _mapper.Map<Exercise>(updatedExercise);

        await _repository.UpdateExerciseAsync(updatedExercise.Id, exercise);

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