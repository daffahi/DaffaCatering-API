using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterMakanan
{
    public class ResepModels
    {
        [Required(ErrorMessage = "ID Resep wajib diisi")]
        [StringLength(10)]
        public string IdResep { get; set; } = "";

        [Required(ErrorMessage = "ID Menu wajib diisi")]
        [StringLength(10)]
        public string IdMenu { get; set; } = "";

        public bool Status { get; set; } = true;

        public List<DetailResepModel> Details { get; set; } = new();
    }

    public class DetailResepModel
    {
        [Required(ErrorMessage = "ID Bahan Baku wajib diisi")]
        [StringLength(10)]
        public string IdBahanBaku { get; set; } = "";

        [Required(ErrorMessage = "ID Satuan Bahan Baku wajib diisi")]
        [StringLength(10)]
        public string IdSatuan { get; set; } = "";

        public decimal Jumlah { get; set; }
    }
}