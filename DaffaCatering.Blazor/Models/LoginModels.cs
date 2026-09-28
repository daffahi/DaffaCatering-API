using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public string IdUser { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = "";
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string ExpiresIn { get; set; } = "";
    }
}