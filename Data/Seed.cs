using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Outmatch.API.Models;

namespace Outmatch.API.Data
{

    // Seeds the database when it is first created with dummy users.  The data for this comes from the Data/UserSeedData.json file
    public class Seed
    {
        public static void SeedClients(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                // Create Roles
                var roles = new List<Role>
                {
                    new Role{Name = "GlobalAdmin"},
                    new Role{Name = "ClientAdmin"},
                    new Role{Name = "LocationAdmin"},
                    new Role{Name = "ReportsOnly"}
                };
                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }

                foreach (var user in users)
                {
                    userManager.CreateAsync(user, "password").Wait();
                    userManager.AddToRoleAsync(user, "LocationAdmin");
                }

                // Create admin user
                var adminUser = new User
                {
                    UserName = "admin"
                };

                var result = userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = userManager.FindByNameAsync("admin").Result;
                    userManager.AddToRolesAsync(admin, new[] {"GlobalAdmin", "ClientAdmin"});
                }
            }
        }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // Hash the password using SHA512
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // Set the password salt to a randomly generated key
                passwordSalt = hmac.Key;
                // Compute the hash
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // Once complete, the values are stores in the byte[] array variabled just a few lines up
            }
            
        }
    }
}