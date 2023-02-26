using System.ComponentModel.DataAnnotations;

namespace NexusFit.Exercises.API.Dtos;

public class ExerciseCreateDto
{
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
}