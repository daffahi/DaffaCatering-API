using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.Resep
{
    public class DetailResepDto
    {
        [Required]
        [StringLength(10)]
        public string IdBahanBaku { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string IdSatuan { get; set; } = null!;

        [Required(ErrorMessage = "Jumlah bahan baku wajib diisi")]
        public decimal Jumlah { get; set; }
    }
}