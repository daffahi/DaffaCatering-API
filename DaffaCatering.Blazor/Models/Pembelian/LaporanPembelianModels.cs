namespace DaffaCatering.Blazor.Models.Pembelian
{
    public class LaporanPembelianKelompok
    {
        public string Id { get; set; } = "";
        public int JumlahTransaksi { get; set; }
        public decimal TotalJumlah { get; set; }
        public decimal Subtotal { get; set; }
        public List<LaporanPembelianBaris> Baris { get; set; } = new();
    }

    public class LaporanPembelianBaris
    {
        public string IdTransaksi { get; set; } = "";
        public DateOnly Tanggal { get; set; }
        public string? IdPembelian { get; set; }
        public string? IdPemasok { get; set; }
        public string? IdBahanBaku { get; set; }
        public string? IdSatuan { get; set; }
        public string? Status { get; set; }
        public DateOnly? TglKadaluwarsa { get; set; }
        public decimal Jumlah { get; set; }
        public decimal Harga { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalTransaksi { get; set; }
    }
}