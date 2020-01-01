// No longer used.  Can remove at a later date.  Cose is useless
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Outmatch.API.Models;

namespace Outmatch.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        // Inject the DataContext so the AuthRepository can query the database
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }

        // Login the user.  Return a task of type User. Takes a string of username and a string of password
        // Use the username to identify the user in the db.  Take the password and compare it with the hashed password of the user to autenticate
        public async Task<User> Login(string username, string password)
        {
            // Created a variable to store the user in.  _context.Users signifies the Users table in the db
            // This will look in the DB for a user that matches the entered username and return it, or return null if it doesnt exist
            var user = await _context.Users.FirstOrDefaultAsync( x => x.UserName == username);

            // If the user is returned null from the DB (IE, username does not exist), then return null.  This would return a 401 not authorized in the browser
            if (user == null)
                return null;

            // Return true or false depending on weather the password matches or doesnt match what the user supplied when loggin in (in a hashed format)
            // If the password returns null, the we will return null and a 401 not authorized in the browser

            // Added Microsoft Identity, Signin manager will now take care of the below  
            // if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            //     return null;

            // If the comparison of the verifyPasswordHash method returns true, then return the user.
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // Use the password salt as a key to hash and salt the password of the user logging in, so it can be compared with the password in the DB. 
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                // Compute the hash from the password, using the key being passed.  Comupted has will be the same as the Register method hash 
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                
                // Loop through the hashed password and compare it to the eash element in the array to ensure the has matches what is stores in the DB
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) 
                        return false;
                }
            }
            // If each element of the password hash array matches, return true
            return true;
        }

        // Takes the user model (entity) and their chosen password.  
        public async Task<User> Register(User user, string password)
        {
            //Turn the password which is in plain text and store is as a salted hash
            byte[] passwordHash, passwordSalt;
            // We want to pass the passwordHash and passwordSalt as a referece, and not as a value.  So this will be done using thr out keyword
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            // Added Microsoft Identity, Signin manager will now take care of the below 
            // user.PasswordHash = passwordHash;
            // user.PasswordSalt = passwordSalt;

            // Forwarf the new user to the database to be stored.
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
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

        //  Check to see if the username exists in the databse. 
        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == username))
                return true;
            return false;
        }
    }
}