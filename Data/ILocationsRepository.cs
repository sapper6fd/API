using System.Collections.Generic;
using System.Threading.Tasks;
using Outmatch.API.Models;

namespace Outmatch.API.Data
{
    // This interface Repository queries the DB and retreives the locations
    public interface ILocationsRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        Task<bool> SalveAll();
        Task<IEnumerable<Locations>> GetLocations();
        Task<Locations> GetLocation(int id);
    }
}