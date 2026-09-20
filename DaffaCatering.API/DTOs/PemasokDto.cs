using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs
{
    public class PemasokDto
    {
        [Required(ErrorMessage = "ID Pemasok wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Pemasok maksimal 10 karakter")]
        public string IdPemasok { get; set; } = null!;

        [Required(ErrorMessage = "Nama Pemasok wajib diisi")]
        [StringLength(50, ErrorMessage = "Nama Pemasok maksimal 50 karakter")]
        public string NamaPemasok { get; set; } = null!;

        [StringLength(20)]
        public string? NoKontak { get; set; }

        [StringLength(255, ErrorMessage = "Alamat maksimal 255 karakter")]
        public string? Alamat { get; set; }

        public DateOnly TglBergabung { get; set; }

        public bool Status { get; set; }
    }
}