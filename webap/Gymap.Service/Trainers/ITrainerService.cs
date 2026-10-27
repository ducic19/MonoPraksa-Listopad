using Gymap.Common;
using Gymap.Common.DTO.Trainers;

namespace Gymap.Service.Trainers;

public interface ITrainerService
{
    Task<List<Trainer>> SearchAsync(string? name);
    Task<Trainer?> GetByIdAsync(Guid id);
    Task<Trainer> CreateAsync(TrainerAddDto dto);
    Task<SaveResult> UpdateAsync(Guid id, TrainerEditDto dto);
    Task<SaveResult> DeleteAsync(Guid id);
}
