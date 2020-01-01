using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Outmatch.API.Data;
using Outmatch.API.Dtos;
using Outmatch.API.Models;

namespace Outmatch.API.Controllers
{
    // Route will be api/auth (http://localhost:5000/api/auth)
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Inject the auth repository and the programs configuration into the controller.
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public AuthController(IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _config = config;
        }

        // Create a new HTTP Post method (http://localhost:5000/api/auth/register) to login the user.  JSON Serialized Object will be passed
        // from the user when they enter it to sign in. Call the Username from the UserForRegisterDto class
        [Authorize(Policy = "RequireGlobalAdminRole")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            // Check if user already exists
            var newUser = await _userManager.FindByNameAsync(userForRegisterDto.username);

            if (newUser != null)
                return BadRequest("Username already exists");

            // If the user does not already exist, create the user and use AutoMapper to map the details to the database
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForRegisterDto>(userToCreate);

            if (result.Succeeded)
            {
                // Send back a location header with the request, and the ID of the user. 
                return CreatedAtRoute("GetUser", new { controller = "Users", Id = userToCreate.Id }, userToReturn);
            }
            return BadRequest(result.Errors);
        }

        // Create a method to allow users to login to the webAPI by returning a token to the users once logged in.
        // Route will be http://localhost:5000/api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // Check that the user trying to login exists
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);

            if (user == null)
                return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            // Check to see if there is anything inside the user from Repo.  If there  is, a user object is present. If not, the username doesnt exist.
            if (result.Succeeded)
            {
                var appUser = _mapper.Map<ClientForListDto>(user);
                // Return the token to the client as an object
                return Ok(new
                {
                    token = GenerateJwtToken(user).Result,
                    user = appUser
                });
            }

            return Unauthorized();
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            // Build a token that will be returned to the user when they login. Contains users ID and their username. 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                // new Claim(ClaimTypes.Role, userFromRepo.AccessLevel)   - THIS IS A ROLE BASED KEY FOR USER ACCESS. NOT USED AND LOOKING FOR AN ALTERNATIVE
            };

            // Get a list of roles the user is in

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create a secret key to sign the token. This key is hashed and not readable in the token.  
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            // Generate signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create a security token descriptor to contain the claims, exp date of the token, and signing credentials for the JWT token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // Generate Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create a token and pass in the token descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}