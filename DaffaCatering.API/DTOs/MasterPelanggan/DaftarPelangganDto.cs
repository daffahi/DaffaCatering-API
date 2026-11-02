namespace DaffaCatering.API.DTOs.MasterPelanggan
{
    public class DaftarPelangganDto
    {
        public string IdPelanggan { get; set; } = string.Empty;
        public string NamaPelanggan { get; set; } = string.Empty;
        public string? NoKontak { get; set; }
        public string? Alamat { get; set; }
        public DateOnly TglBergabung { get; set; }
        public bool Status { get; set; }
    }
}