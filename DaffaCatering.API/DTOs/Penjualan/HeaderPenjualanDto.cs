namespace DaffaCatering.API.DTOs.Penjualan
{
    public class HeaderPenjualanDto
    {
        public string IdPenjualan { get; set; } = string.Empty;
        public string? IdPesanan { get; set; }
        public string IdPelanggan { get; set; } = string.Empty;
        public string MetodePembayaran { get; set; } = string.Empty;
        public DateOnly TglPenjualan { get; set; }
        public decimal? UangMuka { get; set; }
        public decimal Total { get; set; }
        public string? Status { get; set; }

        public List<DetailPenjualanDto> Details { get; set; } = new();
    }
}