using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.Penjualan
{
    public class DetailPenjualanDto
    {
        [Required]
        [StringLength(10)]
        public string IdMenu { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string IdSatuan { get; set; } = null!;

        [Required(ErrorMessage = "Jumlah wajib diisi")]
        public int Jumlah { get; set; }

        [Required(ErrorMessage = "Harga wajib diisi")]
        public decimal Harga { get; set; }
    }
}