using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterMakanan
{
    public class MenuMakananModels
    {
        [Required(ErrorMessage = "ID Menu Makanan wajib diisi")]
        [StringLength(10)]
        public string IdMenu { get; set; } = "";

        [Required(ErrorMessage = "ID Satuan Bahan Baku wajib diisi")]
        [StringLength(10)]
        public string IdSatuan { get; set; } = "";

        [Required(ErrorMessage = "Nama Menu Makanan wajib diisi")]
        [StringLength(50)]
        public string NamaMenu { get; set; } = "";

        public decimal Jumlah { get; set; }

        public decimal Harga { get; set; }

        public bool Status { get; set; } = true;
    }
}