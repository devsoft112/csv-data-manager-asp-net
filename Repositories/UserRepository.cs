using System.Threading.Tasks;
using CSVDataManager.Data;
using CSVDataManager.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CSVDataManager.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get a paginated list of users
        public async Task<IPagedList<User>> GetUsersAsync(int pageNumber, int pageSize)
        {
            return await _context.Users.ToPagedListAsync(pageNumber, pageSize);
        }

        // Get a user by their ID
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Add a new user to the database
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // Update an existing user in the database
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Delete a user from the database
        public async Task DeleteUserAsync(int id)
        {
            // Find the user by their ID
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        // Check if a user exists by their ID
        public bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
