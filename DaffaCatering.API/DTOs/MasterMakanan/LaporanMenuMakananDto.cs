namespace DaffaCatering.API.DTOs.MasterMakanan
{
    public class LaporanMenuMakananDto
    {
        public string IdMenu { get; set; } = string.Empty;
        public string NamaMenu { get; set; } = string.Empty;
        public int Jumlah { get; set; }
        public string NamaSatuan { get; set; } = string.Empty;
        public decimal Harga { get; set; }
    }
}