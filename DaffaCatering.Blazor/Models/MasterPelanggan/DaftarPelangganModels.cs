namespace DaffaCatering.Blazor.Models.MasterPelanggan
{
    public class DaftarPelangganModels
    {
        public string IdPelanggan { get; set; } = "";
        public string NamaPelanggan { get; set; } = "";
        public string? NoKontak { get; set; }
        public string? Alamat { get; set; }
        public DateOnly TglBergabung { get; set; }
        public bool Status { get; set; }
    }
}