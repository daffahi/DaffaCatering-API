using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterMakanan
{
    public class MenuMakananModels
    {
        [Required(ErrorMessage = "ID Menu wajib diisi")]
        [StringLength(10)]
        public string IdMenu { get; set; } = "";

        [Required(ErrorMessage = "Satuan wajib dipilih")]
        [StringLength(10)]
        public string IdSatuan { get; set; } = "";

        [Required(ErrorMessage = "Nama menu wajib diisi")]
        [StringLength(50)]
        public string NamaMenu { get; set; } = "";

        public int Jumlah { get; set; } = 1;

        [Required(ErrorMessage = "Harga wajib diisi")]
        [Range(1, double.MaxValue, ErrorMessage = "Harga harus lebih dari 0")]
        public decimal Harga { get; set; }

        public bool Status { get; set; } = true;
    }
}