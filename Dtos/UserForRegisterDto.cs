using System;
using System.ComponentModel.DataAnnotations;

namespace Outmatch.API.Dtos
{
    public class UserForRegisterDto
    {
        // Validate the request to register with the [Required] flag.  Make sure a username has been provided when registering as a new user. 
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string username { get; set; }
        // Validate the request to register with the [Required] flag.  Make sure a password has been provided when registering as a new user. 
        [Required]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "The password must be between 6 and 12 characters")]
        public string Password { get; set; }
        [Required]
        public string OrganizationName { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        // [Required]
        public string AccessLevel { get; set; }
        [Required]
        public DateTime ValidFrom { get; set; }
        [Required]
        public DateTime ValidTo { get; set; }
    }
}