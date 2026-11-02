namespace DaffaCatering.API.DTOs.Pembelian
{
    public class LaporanPembelianKelompokDto
    {
        public string Id { get; set; } = string.Empty;
        public int JumlahTransaksi { get; set; }
        public decimal TotalJumlah { get; set; }
        public decimal Subtotal { get; set; }
        public List<LaporanPembelianBarisDto> Baris { get; set; } = new();
    }

    public class LaporanPembelianBarisDto
    {
        public string IdTransaksi { get; set; } = string.Empty;
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