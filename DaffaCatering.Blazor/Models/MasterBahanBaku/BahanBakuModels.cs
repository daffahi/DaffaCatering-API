using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterBahanBaku
{
    public class BahanBakuModels
    {
        [Required(ErrorMessage = "ID Bahan Baku wajib diisi")]
        [StringLength(10)]
        public string IdBahanBaku { get; set; } = "";

        [Required(ErrorMessage = "Nama bahan baku wajib diisi")]
        [StringLength(50)]
        public string NamaBahanBaku { get; set; } = "";

        public bool Jenis { get; set; } = true;

        public bool Status { get; set; } = true;

        public List<DetailBahanBakuModel> Details { get; set; } = new();
    }

    public class DetailBahanBakuModel
    {
        [Required(ErrorMessage = "ID Unit wajib diisi")]
        [StringLength(10)]
        public string IdSatuan { get; set; } = "";

        [Required(ErrorMessage = "Tanggal kedaluwarsa wajib diisi")]
        public DateOnly TglKadaluwarsa { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public decimal StokAwal { get; set; }

        public decimal SisaStok { get; set; }
    }
}