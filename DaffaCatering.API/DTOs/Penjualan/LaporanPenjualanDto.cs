namespace DaffaCatering.API.DTOs.Penjualan
{
    public class LaporanKelompokDto
    {
        public string Id { get; set; } = string.Empty;
        public int JumlahTransaksi { get; set; }
        public int TotalJumlah { get; set; }
        public decimal Subtotal { get; set; }
        public List<LaporanBarisDto> Baris { get; set; } = new();
    }

    public class LaporanBarisDto
    {
        public string IdTransaksi { get; set; } = string.Empty;
        public DateOnly Tanggal { get; set; }
        public string? IdPelanggan { get; set; }
        public string? IdMenu { get; set; }
        public string? IdSatuan { get; set; }
        public string? Metode { get; set; }
        public string? Status { get; set; }
        public int Jumlah { get; set; }
        public decimal Harga { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? UangMuka { get; set; }
        public decimal TotalTransaksi { get; set; }
    }
}