using Gymap.Common;

namespace Gymap.DAL.Trainers;

public interface ITrainerRepository
{
    Task<List<Trainer>> GetAllAsync();
    Task<List<Trainer>> SearchAsync(string searchName);
    Task<Trainer?> GetByIdAsync(Guid id);
    Task<bool> IsInUseAsync(Guid id);
    Task AddAsync(Trainer trainer);
    Task UpdateAsync(Trainer trainer);
    Task DeleteAsync(Guid id);
}
