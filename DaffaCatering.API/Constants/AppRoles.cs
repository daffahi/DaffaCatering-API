namespace DaffaCatering.API.Constants
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

        // Dibaca modul lain untuk mengisi dropdown di form
        public const string BacaPelanggan = Admin + "," + Penjualan + ",";
        public const string BacaMenuMakanan = Admin + "," + Penjualan + ",";
        public const string BacaBahanBaku = Admin + "," + Penjualan + "," + Pembelian + "," + Persediaan + ",";
        public const string BacaSatuan = Admin + "," + Penjualan + "," + Pembelian + "," + Persediaan + ",";
    }
}
