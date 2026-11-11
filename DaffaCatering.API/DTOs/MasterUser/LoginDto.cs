using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.User
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username wajib diisi")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; } = null!;
    }
}