using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterPelanggan
{
    public class PelangganDto
    {
        [Required(ErrorMessage = "ID Pelanggan wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Pelanggan maksimal 10 karakter")]
        public string IdPelanggan { get; set; } = null!;

        [Required(ErrorMessage = "Nama Pelanggan wajib diisi")]
        [StringLength(50, ErrorMessage = "Nama Pelanggan maksimal 50 karakter")]
        public string NamaPelanggan { get; set; } = null!;

        [StringLength(20)]
        public string? NoKontak { get; set; }

        [StringLength(255, ErrorMessage = "Alamat maksimal 255 karakter")]
        public string? Alamat { get; set; }

        public DateOnly TglBergabung { get; set; }

        public bool Status { get; set; }
    }
}