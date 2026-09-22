using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.Resep
{
    public class HeaderResepDto
    {
        [Required]
        [StringLength(10)]
        public string IdResep { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string IdMenu { get; set; } = null!;

        public bool Status { get; set; }

        [Required, MinLength(1, ErrorMessage = "Resep minimal punya 1 bahan baku")]
        public List<DetailResepDto> Details { get; set; } = new();
    }
}