using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.User
{
    public class LoginDto
    {
        [Required(ErrorMessage = "ID User wajib diisi")]
        public string IdUser { get; set; } = null!;

        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; } = null!;
    }
}