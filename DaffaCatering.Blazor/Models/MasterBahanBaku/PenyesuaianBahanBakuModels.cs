using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterBahanBaku
{
    public class PenyesuaianBahanBakuModels
    {
        [Required(ErrorMessage = "No. Penyesuaian wajib diisi")]
        [StringLength(10)]
        public string IdPenyesuaian { get; set; } = "";

        public DateOnly TglPenyesuaian { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public List<DetailPenyesuaianBahanBakuModel> Details { get; set; } = new();
    }

    public class DetailPenyesuaianBahanBakuModel
    {
        public string IdBahanBaku { get; set; } = "";

        public string IdSatuan { get; set; } = "";

        public decimal StokFisik { get; set; }

        public decimal SelisihStok { get; set; }

        public DateOnly TglKadaluwarsa { get; set; }

        public string Keterangan { get; set; } = "";
    }
}