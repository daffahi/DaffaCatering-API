namespace DaffaCatering.Blazor.Models.Persediaan
{
    public class LaporanStokBahanBakuModels
    {
    }

    public class LaporanStokBahanBakuKelompok
    {
        public string Id { get; set; } = "";
        public string? IdSatuan { get; set; }
        public decimal StokAwal { get; set; }
        public decimal TotalMasuk { get; set; }
        public decimal TotalKeluar { get; set; }
        public decimal StokAkhir { get; set; }
        public int JumlahTransaksi { get; set; }
        public List<LaporanStokBahanBakuBaris> Baris { get; set; } = new();
    }

    public class LaporanStokBahanBakuBaris
    {
        public string IdTransaksi { get; set; } = "";
        public DateOnly Tanggal { get; set; }
        public string? Keterangan { get; set; }
        public decimal QtyMasuk { get; set; }
        public decimal QtyKeluar { get; set; }
        public decimal SisaStok { get; set; }
        public DateOnly? TglKadaluwarsa { get; set; }
        public int? SisaHari { get; set; }
        public decimal Jumlah { get; set; }
        public decimal Selisih { get; set; }
        public string? Alasan { get; set; }
    }
}