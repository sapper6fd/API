using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Outmatch.API.Data;
using Outmatch.API.Dtos;
using Outmatch.API.Models;

namespace Outmatch.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireGlobalAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await _context.Users
                .OrderBy(x => x.UserName)
                .Select(user => new 
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = (from userRole in user.UserRoles
                                join role in _context.Roles
                                on userRole.Role.Id
                                equals role.Id
                                select role.Name).ToList()
                }).ToListAsync();

            return Ok(userList);
        }

        // Add or remove roles from users (clients)
        [Authorize(Policy = "RequireGlobalAdminRole")]
        [HttpPost("editRoles/{username}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto rollEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = rollEditDto.RoleNames;

            selectedRoles = selectedRoles ?? new string[] {};
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to remove the roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "RequireLocationAdminRole")]
        [HttpGet("locationsRole")]
        public IActionResult GetLocations()
        {
            return Ok("Global Admins and Location Admins can see this.");
        }
    }
}