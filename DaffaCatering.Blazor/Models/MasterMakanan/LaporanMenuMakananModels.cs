namespace DaffaCatering.Blazor.Models.MasterMakanan
{
    public class LaporanMenuMakananModels
    {
        public string IdMenu { get; set; } = "";
        public string NamaMenu { get; set; } = "";
        public int Jumlah { get; set; }
        public string NamaSatuan { get; set; } = "";
        public decimal Harga { get; set; }
    }
}