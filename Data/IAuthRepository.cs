using System.Threading.Tasks;
using Outmatch.API.Models;

namespace Outmatch.API.Data
{
    public interface IAuthRepository
    {
        // Register the user
        // Return a task of User.  We will call this method Register. The method is passed a User object and string of Password
        Task<User> Register(User user, string password);

        // Login the user to the API
        // Return a User.  We will call this method Login, pass a string of username, and a string of Password 
        Task<User> Login(string username, string password);

        // Check if the user already exists
        // Return a boolean task, call the method UserExists.  Check against a string of username to see if that username already exists.
        Task<bool> UserExists(string username);
    }
}