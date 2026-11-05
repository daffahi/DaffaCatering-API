using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.Persediaan
{
    public class PenggunaanBahanBakuModels
    {
        [Required(ErrorMessage = "No. Penggunaan wajib diisi")]
        [StringLength(10)]
        public string IdPenggunaan { get; set; } = "";

        public DateOnly TglPenggunaan { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public List<DetailPenggunaanBahanBakuModel> Details { get; set; } = new();
    }

    public class DetailPenggunaanBahanBakuModel
    {
        public string IdBahanBaku { get; set; } = "";

        public string IdSatuan { get; set; } = "";

        public string NamaBahanBaku { get; set; } = "";

        public DateOnly TglKadaluwarsa { get; set; }

        public decimal JumlahPenggunaan { get; set; }
    }
}