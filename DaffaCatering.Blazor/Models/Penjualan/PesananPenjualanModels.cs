using DaffaCatering.Blazor.Models.Pembelian;
using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.Penjualan
{
    public class PesananPenjualanModels
    {
        [Required(ErrorMessage = "No. Pesanan Penjualan wajib diisi")]
        [StringLength(10)]
        public string IdPesanan { get; set; } = "";

        [Required(ErrorMessage = "ID Pelanggan wajib diisi")]
        [StringLength(10)]
        public string IdPelanggan { get; set; } = "";

        public string MetodePembayaran { get; set; } = "";

        public DateOnly TglPesanan { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public DateOnly TglPengambilan { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public decimal UangMuka { get; set; }

        public decimal Total { get; set; }

        [StringLength(255)]
        public string? Keterangan { get; set; }

        public string Status { get; set; } = "";

        public List<DetailPesananPenjualan> Details { get; set; } = new();
    }

    public class DetailPesananPenjualan
    {
        public string IdPesanan { get; set; } = "";
        public string IdMenu { get; set; } = "";
        public decimal Jumlah { get; set; }
        public decimal Harga { get; set; }
    }
}