using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterSatuanBahanBaku
{
    public class SatuanBahanBakuModels
    {
        [Required(ErrorMessage = "ID Satuan Bahan Baku wajib diisi")]
        [StringLength(10)]
        public string IdSatuan { get; set; } = "";

        [Required(ErrorMessage = "Nama Satuan Bahan Baku wajib diisi")]
        [StringLength(50)]
        public string NamaSatuan { get; set; } = "";
    }
}