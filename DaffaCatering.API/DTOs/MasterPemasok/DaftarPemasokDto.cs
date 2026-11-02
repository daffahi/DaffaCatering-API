namespace DaffaCatering.API.DTOs.MasterPemasok
{
    public class DaftarPemasokDto
    {
        public string IdPemasok { get; set; } = string.Empty;
        public string NamaPemasok { get; set; } = string.Empty;
        public string? NoKontak { get; set; }
        public string? Alamat { get; set; }
        public DateOnly TglBergabung { get; set; }
        public bool Status { get; set; }
    }
}