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

        // Constructor to inject the database context
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get a paginated list of users
        public async Task<IPagedList<User>> GetUsersAsync(int pageNumber, int pageSize)
        {
            // Return a paginated list of users
            return await _context.Users.ToPagedListAsync(pageNumber, pageSize);
        }

        // Get a user by their ID
        public async Task<User> GetUserByIdAsync(int id)
        {
            // Find and return a user by their ID
            return await _context.Users.FindAsync(id);
        }

        // Add a new user to the database
        public async Task AddUserAsync(User user)
        {
            // Add the user to the Users DbSet
            await _context.Users.AddAsync(user);
            // Save the changes to the database
            await _context.SaveChangesAsync();
        }

        // Update an existing user in the database
        public async Task UpdateUserAsync(User user)
        {
            // Update the user in the Users DbSet
            _context.Users.Update(user);
            // Save the changes to the database
            await _context.SaveChangesAsync();
        }

        // Delete a user from the database
        public async Task DeleteUserAsync(int id)
        {
            // Find the user by their ID
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                // Remove the user from the Users DbSet
                _context.Users.Remove(user);
                // Save the changes to the database
                await _context.SaveChangesAsync();
            }
        }

        // Check if a user exists by their ID
        public bool UserExists(int id)
        {
            // Return true if a user with the given ID exists
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
