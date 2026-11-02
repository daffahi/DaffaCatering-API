namespace DaffaCatering.Blazor.Models.MasterPemasok
{
    public class DaftarPemasokModels
    {
        public string IdPemasok { get; set; } = "";
        public string NamaPemasok { get; set; } = "";
        public string? NoKontak { get; set; }
        public string? Alamat { get; set; }
        public DateOnly TglBergabung { get; set; }
        public bool Status { get; set; }
    }
}