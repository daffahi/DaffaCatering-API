using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterUser
{
    public class UserDto
    {
        [Required(ErrorMessage = "ID User wajib diisi")]
        [StringLength(10, ErrorMessage = "ID User maksimal 10 karakter")]
        public string IdUser { get; set; } = null!;

        [Required(ErrorMessage = "ID Role wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Role maksimal 10 karakter")]
        public string IdRole { get; set; } = null!;

        [Required(ErrorMessage = "Nama User wajib diisi")]
        [StringLength(50, ErrorMessage = "Nama User maksimal 50 karakter")]
        public string NamaUser { get; set; } = null!;

        [Required(ErrorMessage = "Password wajib diisi")]
        [StringLength(25, ErrorMessage = "Password maksimal 25 karakter")]
        public string Password { get; set; } = null!;

        public bool Status { get; set; }
    }
}