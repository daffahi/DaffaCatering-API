using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterBahanBaku
{
    public class DetailBahanBakuDto
    {
        [Required(ErrorMessage = "ID Satuan Bahan Baku wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Satuan Bahan Baku maksimal 10 karakter")]
        public string IdSatuan { get; set; } = null!;

        [Required(ErrorMessage = "Tanggal Kadaluwarsa wajib diisi")]
        public DateOnly TglKadaluwarsa { get; set; }

        public decimal StokAwal { get; set; }

        public decimal SisaStok { get; set; }
    }
}