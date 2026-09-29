using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterBahanBaku
{
    public class BahanBakuModels
    {
        [Required(ErrorMessage = "Raw Material ID is required")]
        [StringLength(10)]
        public string IdBahanBaku { get; set; } = "";

        [Required(ErrorMessage = "Raw Material name is required")]
        [StringLength(50)]
        public string NamaBahanBaku { get; set; } = "";

        public bool Jenis { get; set; } = true;

        public bool Status { get; set; } = true;

        public List<DetailBahanBakuModel> Details { get; set; } = new();
    }

    public class DetailBahanBakuModel
    {
        [Required(ErrorMessage = "Unit ID is required")]
        [StringLength(10)]
        public string IdSatuan { get; set; } = "";

        [Required(ErrorMessage = "Expiry date is required")]
        public DateOnly TglKadaluwarsa { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public decimal StokAwal { get; set; }

        public decimal SisaStok { get; set; }
    }
}