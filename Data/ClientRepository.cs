using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Outmatch.API.Helpers;
using Outmatch.API.Models;

namespace Outmatch.API.Data
{
    // Contains the concrete methods of interacting with the users table within the database, via the IClientRepository interface.
    public class ClientRepository : IClientRepository
    { 

        // Connect to the database using _context m
        private readonly DataContext _context;
        public ClientRepository(DataContext context)
        {
            _context = context;

        }

        // Add method for the users table
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        // Delete method for the users table
        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        // Get (user) method from the users table
        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
        // Get (All users) from the users table
        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users;
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }
        // Save changes to the users table
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}