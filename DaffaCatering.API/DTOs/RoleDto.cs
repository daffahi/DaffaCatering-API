using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs
{
    public class RoleDto
    {
        [Required(ErrorMessage = "ID Role wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Role maksimal 10 karakter")]
        public string IdRole { get; set; } = null!;

        [Required(ErrorMessage = "Nama Role wajib diisi")]
        [StringLength(50, ErrorMessage = "Nama Role maksimal 50 karakter")]
        public string NamaRole { get; set; } = null!;
    }
}