using AutoMapper;
using NexusFit.Exercises.API.Dtos;
using NexusFit.Exercises.API.Entities;

namespace NexusFit.Exercises.API.Mappings;

public class ExerciseProfile : Profile
{
    public ExerciseProfile()
    {
        CreateMap<Exercise, ExerciseDetailsDto>().ReverseMap();
        CreateMap<Exercise, ExerciseCreateDto>().ReverseMap();
        CreateMap<Exercise, ExerciseUpdateDto>().ReverseMap();
    }
}