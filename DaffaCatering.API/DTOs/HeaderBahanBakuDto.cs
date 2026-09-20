using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs
{
    public class HeaderBahanBakuDto
    {
        [Required(ErrorMessage = "ID Bahan Baku wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Bahan Baku maksimal 10 karakter")]
        public string IdBahanBaku { get; set; } = null!;

        [Required(ErrorMessage = "Nama Bahan Baku wajib diisi")]
        [StringLength(50, ErrorMessage = "Nama Bahan Baku maksimal 50 karakter")]
        public string NamaBahanBaku { get; set; } = null!;

        public bool Jenis { get; set; }

        public bool Status { get; set; }
    }
}
