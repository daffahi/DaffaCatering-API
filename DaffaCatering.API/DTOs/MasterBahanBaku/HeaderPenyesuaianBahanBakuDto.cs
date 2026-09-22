using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterBahanBaku
{
    public class HeaderPenyesuaianBahanBakuDto
    {
        [Required]
        [StringLength(10)]
        public string IdPenyesuaian { get; set; } = null!;

        [Required]
        public DateOnly TglPenyesuaian { get; set; }

        [Required, MinLength(1, ErrorMessage = "Minimal 1 item disesuaikan")]
        public List<DetailPenyesuaianBahanBakuDto> Details { get; set; } = new();
    }
}