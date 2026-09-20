namespace DaffaCatering.API.DTOs
{
    public class HeaderBahanBakuDto
    {
        public string IdBahanBaku { get; set; } = null!;
        public string NamaBahanBaku { get; set; } = null!;
        public bool Jenis { get; set; }   
        public bool Status { get; set; }     
    }
}
