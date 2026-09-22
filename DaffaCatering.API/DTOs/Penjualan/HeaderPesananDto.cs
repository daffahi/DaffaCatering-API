using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.Penjualan
{
    public class HeaderPesananDto
    {
        [Required]
        [StringLength(10)]
        public string IdPesanan { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string IdPelanggan { get; set; } = null!;

        [Required]
        public DateOnly TglPesanan { get; set; }

        [Required]
        public DateOnly TglPengambilan { get; set; }

        [StringLength(10)]
        public string? MetodePembayaran { get; set; }

        public decimal UangMuka { get; set; }

        [StringLength(255)]
        public string? Keterangan { get; set; }

        [StringLength(10)]
        public string? Status { get; set; }

        // Total DIHITUNG OTOMATIS dari Details, tidak diminta dari input

        [Required, MinLength(1, ErrorMessage = "Minimal 1 menu dipesan")]
        public List<DetailPesananDto> Details { get; set; } = new();
    }
}