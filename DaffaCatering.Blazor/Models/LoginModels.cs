using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "ID User wajib diisi")]
        public string IdUser { get; set; } = "";

        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; } = "";
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string ExpiresIn { get; set; } = "";
    }
}