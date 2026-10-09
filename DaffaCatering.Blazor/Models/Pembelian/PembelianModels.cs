using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.Pembelian
{
    public class PembelianModels
    {
        [Required(ErrorMessage = "No. Pembelian wajib diisi")]
        [StringLength(10)]
        public string IdPembelian { get; set; } = "";

        [Required(ErrorMessage = "ID Pemasok wajib diisi")]
        [StringLength(10)]
        public string IdPemasok { get; set; } = "";

        public DateOnly TglPembelian { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public decimal total { get; set; }

        public string? Status { get; set; }

        public List<DetailPembelian> Details { get; set; } = new();
    }

    public class DetailPembelian
    {
        public string IdBahanBaku { get; set; } = "";
        public string IdSatuan { get; set; } = "";
        public decimal Jumlah { get; set; }
        public decimal harga { get; set; }
    }
}