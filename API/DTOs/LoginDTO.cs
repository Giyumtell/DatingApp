using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "username cannot be empty")]
        public string Username { get; set; }
        [Required(ErrorMessage = "password cannot be empty")]
        public string Password { get; set; }
    }
}
