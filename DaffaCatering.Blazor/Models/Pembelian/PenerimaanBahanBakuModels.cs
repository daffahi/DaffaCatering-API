using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DaffaCatering.Blazor.Models.Pembelian
{
    public class PenerimaanBahanBakuModels
    {
        [Required(ErrorMessage = "No. Penerimaan Bahan Baku wajib diisi")]
        [StringLength(10)]
        public string IdPenerimaan { get; set; } = "";

        [Required(ErrorMessage = "ID Pemasok wajib diisi")]
        [StringLength(10)]
        public string IdPemasok { get; set; } = "";

        [Required(ErrorMessage = "No. Pembelian wajib diisi")]
        [StringLength(10)]
        public string IdPembelian { get; set; } = "";

        public DateOnly TglPenerimaan { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public string Status { get; set; } = "";

        public List<DetailPenerimaan> Details { get; set; } = new();

        [JsonPropertyName("detailPenerimaans")]
        public List<DetailPenerimaan> DetailPenerimaans
        {
            get => Details;
            set => Details = value;
        }
    }

    public class DetailPenerimaan
    {
        public string IdBahanBaku { get; set; } = "";

        public string IdSatuan { get; set; } = "";

        public decimal Jumlah { get; set; }

        public DateOnly TglKadaluwarsa { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public string StatusBahanBaku { get; set; } = "";
    }
}