using System;

namespace Outmatch.API.Models
{
    public class Locations
    {
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPhone { get; set; }
        public DateTime ActiveDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }
    }
}