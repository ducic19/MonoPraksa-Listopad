using WebApplication3.Model;

namespace WebApplication3.Service.Common
{
    public interface ISongService
    {
        Task<List<Member>> GetAllAsync();
        Task<List<Trainer>> GetAllEmployeesAsync();
    }
}