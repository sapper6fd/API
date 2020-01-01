using System.Collections.Generic;
using System.Threading.Tasks;
using Outmatch.API.Helpers;
using Outmatch.API.Models;

namespace Outmatch.API.Data
{
    public interface IClientRepository
    {
        // Queries the repository so we can get our users and thir details.  The IClientRepository has the blurprint of the methods. 
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id);
    }
}