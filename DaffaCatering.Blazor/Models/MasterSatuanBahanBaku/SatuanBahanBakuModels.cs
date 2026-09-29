using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterSatuanBahanBaku
{
    public class SatuanBahanBakuModels
    {
        [Required(ErrorMessage = "Raw Material Unit ID is required")]
        [StringLength(10)]
        public string IdSatuan { get; set; } = "";

        [Required(ErrorMessage = "Raw Material Unit Name is required")]
        [StringLength(50)]
        public string NamaSatuan { get; set; } = "";
    }
}