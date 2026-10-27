namespace DaffaCatering.Blazor.Models
{
    public class AppRoles
    {
        public const string Admin = "R001";
        public const string Penjualan = "R002";
        public const string Pembelian = "R003";
        public const string Persediaan = "R004";

        public const string AdminPenjualan = Admin + "," + Penjualan;
        public const string AdminPembelian = Admin + "," + Pembelian;
        public const string AdminPersediaan = Admin + "," + Persediaan;
        public const string Semua = Admin + "," + Penjualan + "," + Pembelian + "," + Persediaan;
    }
}
