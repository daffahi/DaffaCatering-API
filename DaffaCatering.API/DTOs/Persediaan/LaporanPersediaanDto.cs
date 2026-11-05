namespace DaffaCatering.API.DTOs.Persediaan
{
    public class LaporanPersediaanDto
    {
        public class LaporanPersediaanKelompokDto
        {
            public string Id { get; set; } = string.Empty;          // IdBahanBaku
            public string? IdSatuan { get; set; }
            public decimal StokAwal { get; set; }                   // saldo awal periode
            public decimal TotalMasuk { get; set; }
            public decimal TotalKeluar { get; set; }
            public decimal StokAkhir { get; set; }
            public int JumlahTransaksi { get; set; }
            public List<LaporanPersediaanBarisDto> Baris { get; set; } = new();
        }

        public class LaporanPersediaanBarisDto
        {
            public string IdTransaksi { get; set; } = string.Empty;
            public DateOnly Tanggal { get; set; }
            public string? Keterangan { get; set; }

            // Stok
            public decimal QtyMasuk { get; set; }
            public decimal QtyKeluar { get; set; }
            public decimal SisaStok { get; set; }

            // Kadaluwarsa
            public DateOnly? TglKadaluwarsa { get; set; }
            public int? SisaHari { get; set; }

            // Penggunaan & kadaluwarsa (sisa stok batch)
            public decimal Jumlah { get; set; }

            // Penyesuaian
            public decimal Selisih { get; set; }
            public string? Alasan { get; set; }
        }
    }
}