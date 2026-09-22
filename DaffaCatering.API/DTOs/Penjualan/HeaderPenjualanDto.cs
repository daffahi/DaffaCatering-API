using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.Penjualan
{
    public class HeaderPenjualanDto
    {
        [Required]
        [StringLength(10)]
        public string IdPenjualan { get; set; } = null!;

        // Opsional: kalau penjualan ini berasal dari pesanan sebelumnya
        [StringLength(10)]
        public string? IdPesanan { get; set; }

        [Required]
        [StringLength(10)]
        public string IdPelanggan { get; set; } = null!;

        [StringLength(10)]
        public string? MetodePembayaran { get; set; }

        [Required]
        public DateOnly TglPenjualan { get; set; }

        public decimal UangMuka { get; set; }

        [StringLength(10)]
        public string? Status { get; set; }

        [Required, MinLength(1, ErrorMessage = "Minimal 1 menu terjual")]
        public List<DetailPenjualanDto> Details { get; set; } = new();
    }
}