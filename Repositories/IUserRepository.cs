using System.Threading.Tasks;
using CSVDataManager.Models;
using X.PagedList;

namespace CSVDataManager.Repositories
{
    public interface IUserRepository
    {
        Task<IPagedList<User>> GetUsersAsync(int pageNumber, int pageSize);
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        bool UserExists(int id);
    }
}
