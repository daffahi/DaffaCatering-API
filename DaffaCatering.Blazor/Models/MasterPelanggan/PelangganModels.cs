using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterPelanggan
{
    public class PelangganModels
    {
        [Required(ErrorMessage = "Customer ID is required")]
        [StringLength(10)]
        public string IdPelanggan { get; set; } = "";

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(50)]
        public string NamaPelanggan { get; set; } = "";

        [StringLength(20)]
        public string? NoKontak { get; set; }

        [StringLength(255)]
        public string? Alamat { get; set; }

        public DateOnly TglBergabung { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public bool Status { get; set; } = true;
    }
}