using Microsoft.AspNetCore.Mvc;
using NexusFit.Exercises.API.Repository;
using NexusFit.Exercises.API.Entities;
using NexusFit.Exercises.API.Dtos;
using AutoMapper;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using NexusFit.BuildingBlocks.Common.Models;

namespace NexusFit.Exercises.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseRepository _repository;
    private readonly IMapper _mapper;

    public ExercisesController(IExerciseRepository repo, IMapper mapper)
    {
        _repository = repo;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExerciseDetailsDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<Exercise>>> GetExercises()
    {
        var exercises = await _repository.GetExercisesAsync();

        var exercisesDetails = _mapper.Map<List<ExerciseDetailsDto>>(exercises);

        return Ok(exercisesDetails);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExerciseDetailsDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ExerciseDetailsDto>> GetExerciseById(string id)
    {
        var exercise = await _repository.GetExerciseAsync(id);

        if (exercise is null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, 
            $"Failed to find exercise with id ${id}"));

        var exerciseDetails = _mapper.Map<ExerciseDetailsDto>(exercise);

        return Ok(exerciseDetails);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateExercise(ExerciseCreateDto exerciseCreateDto) 
    {
        var exercise = _mapper.Map<Exercise>(exerciseCreateDto);

        await _repository.CreateExerciseAsync(exercise);

        var exerciseDetails = _mapper.Map<ExerciseDetailsDto>(exercise);

        return CreatedAtAction(nameof(GetExerciseById), new { id = exerciseDetails.Id }, exerciseDetails);
    }

    [HttpPut]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateExercise(ExerciseUpdateDto updatedExercise) 
    {
        var exercise = await _repository.GetExerciseAsync(updatedExercise.Id);

        if (exercise is null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, 
            $"Failed to find exercise with id ${updatedExercise.Id}"));

        exercise = _mapper.Map<Exercise>(updatedExercise);

        await _repository.UpdateExerciseAsync(updatedExercise.Id, exercise);

        return Ok();
    }

    [HttpDelete]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteExercise(string id)
    {
        var exercise = await _repository.GetExerciseAsync(id);

        if (exercise is null)
            return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, 
            $"Failed to find exercise with id ${id}"));

        await _repository.RemoveExerciseAsync(id);

        return Ok();
    }
}