using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.API.DTOs.MasterBahanBaku
{
    public class DetailPenyesuaianBahanBakuDto
    {
        [Required]
        [StringLength(10)]
        public string IdBahanBaku { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string IdSatuan { get; set; } = null!;

        [Required(ErrorMessage = "Stok fisik hasil hitung wajib diisi")]
        public decimal StokFisik { get; set; }

        [Required]
        public DateOnly TglKadaluwarsa { get; set; }

        [StringLength(255)]
        public string? Keterangan { get; set; }
    }
}