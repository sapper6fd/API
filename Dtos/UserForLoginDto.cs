namespace Outmatch.API.Dtos
{
    public class UserForLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        // public string AccessLevel { get; set; } THIS IS A ROLE BASED KEY FOR USER ACCESS. NOT USED AND LOOKING FOR AN ALTERNATIVE
    }
}