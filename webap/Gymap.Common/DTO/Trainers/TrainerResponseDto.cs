namespace Gymap.Common.DTO.Trainers;

public record TrainerResponseDto(
    Guid TrainerId,
    string Name,
    string Email);
