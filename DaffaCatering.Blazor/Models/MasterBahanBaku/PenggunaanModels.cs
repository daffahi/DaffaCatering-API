using System.ComponentModel.DataAnnotations;

namespace DaffaCatering.Blazor.Models.MasterBahanBaku
{
    public class PenggunaanModels
    {
        [Required(ErrorMessage = "No. Penggunaan wajib diisi")]
        [StringLength(10)]
        public string IdPenggunaan { get; set; } = "";

        public DateOnly TglPenggunaan { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        // Dikirim ke API saat POST (bentuknya sama dengan HeaderPenggunaanBahanBakuDto)
        public List<DetailPenggunaanInputModel> Details { get; set; } = new();
    }

    public class DetailPenggunaanInputModel
    {
        public string IdBahanBaku { get; set; } = "";

        public DateOnly TglKadaluwarsa { get; set; }

        public decimal JumlahPenggunaan { get; set; }
    }
}