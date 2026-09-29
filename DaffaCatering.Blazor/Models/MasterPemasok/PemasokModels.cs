using System.ComponentModel.DataAnnotations;


namespace DaffaCatering.Blazor.Models.MasterPemasok
{
    public class PemasokModels
    {
        [Required(ErrorMessage = "ID Pemasok wajib diisi")]
        [StringLength(10)]
        public string IdPemasok { get; set; } = "";

        [Required(ErrorMessage = "Nama Pemasok wajib diisi")]
        [StringLength(50)]
        public string NamaPemasok { get; set; } = "";

        [StringLength(20)]
        public string? NoKontak { get; set; }

        [StringLength(255)]
        public string? Alamat { get; set; }

        public DateOnly TglBergabung { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public bool Status { get; set; } = true;
    }
}