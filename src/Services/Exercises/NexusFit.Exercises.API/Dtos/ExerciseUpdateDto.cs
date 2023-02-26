using System.ComponentModel.DataAnnotations;

namespace NexusFit.Exercises.API.Dtos;

public class ExerciseUpdateDto
{
    [Required]
    public string Id { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
}