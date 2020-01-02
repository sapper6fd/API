using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Outmatch.API.Models;

namespace Outmatch.API.Data
{
    public class LocationsRepository : ILocationsRepository
    {
        private readonly DataContext _context;
        public LocationsRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        
        // get a location only one of them).  Pass in the location ID from the user interface, and pull the location that corrisponds to the 
        // id, returning it to the GUI 
        public async Task<Locations> GetLocation(int id)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(u => u.Id == id);
            return location;
        }

        // Get the lost of all locations and return them to the GUI
        public async Task<IEnumerable<Locations>> GetLocations()
        {
            var locations = await _context.Locations.ToListAsync();
            return locations;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;

        }
    }
}