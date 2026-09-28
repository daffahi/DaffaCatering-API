using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterPemasok
{
    public class PemasokDto
    {
        [Required(ErrorMessage = "Supplier ID is required")]
        [StringLength(10, ErrorMessage = "Supplier ID must be at most 10 characters")]
        public string IdPemasok { get; set; } = null!;

        [Required(ErrorMessage = "Supplier Name is required")]
        [StringLength(50, ErrorMessage = "Supplier Name must be at most 50 characters")]
        public string NamaPemasok { get; set; } = null!;

        [StringLength(20)]
        public string? NoKontak { get; set; }

        [StringLength(255, ErrorMessage = "Address must be at most 255 characters")]
        public string? Alamat { get; set; }

        public DateOnly TglBergabung { get; set; }

        public bool Status { get; set; }
    }
}