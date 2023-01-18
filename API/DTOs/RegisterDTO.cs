using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "username cannot be empty")]
        public string Username { get; set; }
        [Required(ErrorMessage = "password cannot be empty")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",ErrorMessage ="min 8,upper,lower,number,special char")]
        public string Password { get; set; }
    }
}
