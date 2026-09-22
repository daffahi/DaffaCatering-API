using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterBahanBaku
{
    public class HeaderPenggunaanBahanBakuDto
    {
        [Required]
        [StringLength(10)]
        public string IdPenggunaan { get; set; } = null!;

        [Required]
        public DateOnly TglPenggunaan { get; set; }

        [Required, MinLength(1, ErrorMessage = "Minimal 1 item bahan baku digunakan")]
        public List<DetailPenggunaanBahanBakuDto> Details { get; set; } = new();
    }
}