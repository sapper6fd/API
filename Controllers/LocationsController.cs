using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Outmatch.API.Data;
using Outmatch.API.Dtos;
using Outmatch.API.Models;

namespace Outmatch.API.Controllers
{
    // Controller for getting location and locations.
    // [Authorize]
    // [Route("api/[controller]")]
    // [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationsRepository _repo;
        private readonly IMapper _mapper;
        private readonly IClientRepository _clientRepo;
        private readonly UserManager<User> _userManager;
        public LocationsController(ILocationsRepository repo, IMapper mapper, IClientRepository clientRepo, UserManager<User> userManager)
        {
            _userManager = userManager;
            _clientRepo = clientRepo;
            _mapper = mapper;
            _repo = repo;
        }

        // Get a list of all locations from the database
        [HttpGet]
        public async Task<IActionResult> GetLocations()
        {
            var locations = await _repo.GetLocations();

            var locationsToReturn = _mapper.Map<IEnumerable<LocationForListDto>>(locations);

            return Ok(locations);
        }
        // Create a location
        [HttpPost]
        public async Task<IActionResult> CreateLocation(int clientId, LocationCreateDto locationCreateDto)
        {
            //  Grab the current users roles to verify they have access to this API call
            var userRole = User.FindFirst(ClaimTypes.Role).ToString();

            if (userRole != "http://schemas.microsoft.com/ws/2008/06/identity/claims/role: GlobalAdmin")
                return Unauthorized();

            // Create the new location
            var newLocation = _mapper.Map<Locations>(locationCreateDto);

            _repo.Add(newLocation);

            if (await _repo.SaveAll())
            {
                var locationToReturn = _mapper.Map<LocationCreateDto>(newLocation);
                return CreatedAtRoute("GetLocation", new { clientId, id = locationToReturn.Id }, locationToReturn);
            }
            throw new Exception("Unable to create new location. Failed to save.");
        }

        // Get a specific location from the database
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocation(int id)
        {
            var location = await _repo.GetLocation(id);

            var locationToReturn = _mapper.Map<LocationCreateDto>(location);

            return Ok(locationToReturn);
        }
    }
}