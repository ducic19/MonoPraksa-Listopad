using WebApplication3.Model;

namespace WebApplication3.Repository.Common
{
    public interface ISongRepository
    {
        Task<List<Member>> GetAllAsync();
        Task<List<Trainer>> GetAllEmployeesAsync();
    }
}