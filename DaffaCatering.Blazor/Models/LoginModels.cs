using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username wajib diisi")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Kata sandi wajib diisi")]
        public string Password { get; set; } = "";
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string ExpiresIn { get; set; } = "";
    }
}