using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterPelanggan
{
    public class PelangganDto
    {
        [Required(ErrorMessage = "Customer ID is required")]
        [StringLength(10, ErrorMessage = "Customer ID must be at most 10 characters")]
        public string IdPelanggan { get; set; } = null!;

        [Required(ErrorMessage = "Customer Name is required")]
        [StringLength(50, ErrorMessage = "Customer Name must be at most 50 characters")]
        public string NamaPelanggan { get; set; } = null!;

        [StringLength(20)]
        public string? NoKontak { get; set; }

        [StringLength(255, ErrorMessage = "Address must be at most 255 characters")]
        public string? Alamat { get; set; }

        public DateOnly TglBergabung { get; set; }

        public bool Status { get; set; }
    }
}