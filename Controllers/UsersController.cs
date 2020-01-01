using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outmatch.API.Data;
using Outmatch.API.Dtos;
using Outmatch.API.Helpers;

namespace Outmatch.API.Controllers
{
    // API controller for the users table to get, modify, add and delete from the users table
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IClientRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IClientRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        // Get a list of users from the user table
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var users = await _repo.GetUsers(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForRegisterDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        // Get individual users from the user table
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserForRegisterDto>(user);

            return Ok(userToReturn);
        }

        [Authorize(Policy = "RequireGlobalAdminRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, UserForEditDto userForEditDto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role).ToString();

            if (userRole != "http://schemas.microsoft.com/ws/2008/06/identity/claims/role: GlobalAdmin")
                return Unauthorized();
            
            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(userForEditDto, userFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating client with {id} failed to save");
        }
    }
}