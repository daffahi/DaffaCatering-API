using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.Pembelian
{
    public class HeaderPenerimaanDto
    {
        [Required(ErrorMessage = "ID Penerimaan Bahan Baku wajib diisi")]
        [StringLength(10)]
        public string IdPenerimaan { get; set; } = null!;

        [Required(ErrorMessage = "ID Pemasok wajib diisi")]
        [StringLength(10)]
        public string IdPemasok { get; set; } = null!;

        [Required(ErrorMessage = "ID Pembelian wajib diisi")]
        [StringLength(10)]
        public string IdPembelian { get; set; } = null!;

        [Required(ErrorMessage = "Tanggal Penerimaan wajib diisi")]
        public DateOnly TglPenerimaan { get; set; }

        [StringLength(10)]
        public string? Status { get; set; }

        [Required, MinLength(1, ErrorMessage = "Minimal 1 item diterima")]
        public List<DetailPenerimaanDto> Details { get; set; } = new();
    }
}