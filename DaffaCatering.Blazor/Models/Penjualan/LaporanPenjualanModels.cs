namespace DaffaCatering.Blazor.Models.Penjualan
{
    public class LaporanKelompok
    {
        public string Id { get; set; } = "";
        public int JumlahTransaksi { get; set; }
        public decimal TotalJumlah { get; set; }
        public decimal Subtotal { get; set; }
        public List<LaporanBaris> Baris { get; set; } = new();
    }

    public class LaporanBaris
    {
        public string IdTransaksi { get; set; } = "";
        public DateOnly Tanggal { get; set; }
        public string? IdPelanggan { get; set; }
        public string? IdMenu { get; set; }
        public string? IdSatuan { get; set; }
        public string? Metode { get; set; }
        public string? Status { get; set; }
        public decimal Jumlah { get; set; }
        public decimal Harga { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? UangMuka { get; set; }
        public decimal TotalTransaksi { get; set; }
    }
}