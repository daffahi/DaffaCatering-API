using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterSatuanBahanBaku
{
    public class SatuanBahanBakuDto
    {
        [Required(ErrorMessage = "ID Satuan Bahan Baku wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Satuan Bahan Baku maksimal 10 karakter")]
        public string IdSatuan { get; set; } = null!;

        [Required(ErrorMessage = "Nama Satuan Bahan Baku wajib diisi")]
        [StringLength(50, ErrorMessage = "Nama Satuan Bahan Baku maksimal 50 karakter")]
        public string NamaSatuan { get; set; } = null!;
    }
}