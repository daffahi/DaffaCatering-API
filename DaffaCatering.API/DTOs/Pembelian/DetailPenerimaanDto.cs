using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.Pembelian
{
    public class DetailPenerimaanDto
    {
        [Required(ErrorMessage = "ID Bahan Baku wajib diisi")]
        [StringLength(10)]
        public string IdBahanBaku { get; set; } = null!;

        [Required(ErrorMessage = "ID Satuan wajib diisi")]
        [StringLength(10)]
        public string IdSatuan { get; set; } = null!;

        [Required(ErrorMessage = "Jumlah wajib diisi")]
        public decimal Jumlah { get; set; }

        [Required(ErrorMessage = "Tanggal Kadaluwarsa wajib diisi")]
        public DateOnly TglKadaluwarsa { get; set; }

        [StringLength(10)]
        public string? StatusBahanBaku { get; set; }
    }
}