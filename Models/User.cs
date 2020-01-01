using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Outmatch.API.Models
{
    // List of properties for the User (Client) table in the db
    public class User : IdentityUser<int>
    {
        public string OrganizationName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public ICollection<Locations> Locations { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}