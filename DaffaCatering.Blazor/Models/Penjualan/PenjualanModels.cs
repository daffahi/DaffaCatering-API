using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.Penjualan
{
    public class PenjualanModels
    {
        [Required(ErrorMessage = "No. Penjualan wajib diisi")]
        [StringLength(10)]
        public string IdPenjualan { get; set; } = "";

        [StringLength(10)]
        public string? IdPesanan { get; set; }

        [Required(ErrorMessage = "ID Pelanggan wajib diisi")]
        [StringLength(10)]
        public string IdPelanggan { get; set; } = "";

        public string MetodePembayaran { get; set; } = "";

        public DateOnly TglPenjualan { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public decimal? UangMuka { get; set; }

        public decimal Total { get; set; }

        public string Status { get; set; } = "";

        public List<DetailPenjualan> Details { get; set; } = new();
    }

    public class DetailPenjualan
    {
        public string IdPenjualan { get; set; } = "";
        public string IdMenu { get; set; } = "";
        public string IdSatuan { get; set; } = "";
        public int Jumlah { get; set; }
        public decimal Harga { get; set; }
    }
}