using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.Pembelian
{
    public class HeaderPembelianDto
    {
        [Required(ErrorMessage = "ID Pembelian wajib diisi")]
        [StringLength(10)]
        public string IdPembelian { get; set; } = null!;

        [Required(ErrorMessage = "ID Pemasok wajib diisi")]
        [StringLength(10)]
        public string IdPemasok { get; set; } = null!;

        [Required(ErrorMessage = "Tanggal Pembelian wajib diisi")]
        public DateOnly TglPembelian { get; set; }

        [StringLength(10)]
        public string? Status { get; set; }

        // TIDAK ada field Total — dihitung otomatis di controller

        [Required(ErrorMessage = "Detail pembelian wajib diisi, minimal 1 item")]
        [MinLength(1, ErrorMessage = "Minimal harus ada 1 item bahan baku")]
        public List<DetailPembelianDto> Details { get; set; } = new();
    }
}