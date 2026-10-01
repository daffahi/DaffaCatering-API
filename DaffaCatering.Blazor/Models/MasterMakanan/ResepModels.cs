using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models
{
    public class ResepModel
    {
        [Required(ErrorMessage = "ID Resep wajib diisi")]
        [StringLength(10)]
        public string IdResep { get; set; } = "";

        [Required(ErrorMessage = "Menu Makanan wajib dipilih")]
        [StringLength(10)]
        public string IdMenu { get; set; } = "";

        public bool Status { get; set; } = true;

        public List<DetailResepModel> Details { get; set; } = new();
    }

    public class DetailResepModel
    {
        public string IdBahanBaku { get; set; } = "";
        public string IdSatuan { get; set; } = "";
        public decimal Jumlah { get; set; }
    }
}