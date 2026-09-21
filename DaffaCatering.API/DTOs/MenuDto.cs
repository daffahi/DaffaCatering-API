using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace DaffaCatering.API.DTOs
{
    public class MenuDto
    {
        [Required(ErrorMessage = "ID Menu Makanan wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Menu Makanan maksimal 10 karakter")]
        public string IdMenu { get; set; } = null!;

        [Required(ErrorMessage = "ID Satuan Bahan Baku wajib diisi")]
        [StringLength(10, ErrorMessage = "ID Satuan Bahan Baku maksimal 10 karakter")]
        public string IdSatuan { get; set; } = null!;

        [Required(ErrorMessage = "Nama Menu Makanan wajib diisi")]
        [StringLength(50, ErrorMessage = "Nama Menu Makanan maksimal 50 karakter")]
        public string NamaMenu { get; set; } = null!;

        public int Jumlah { get; set; }

        public decimal Harga { get; set; }

        public bool Status { get; set; }
    }
}