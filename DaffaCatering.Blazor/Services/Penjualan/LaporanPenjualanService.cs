using DaffaCatering.Blazor.Models.Penjualan;
using System.Globalization;
using System.Net.Http.Json;

namespace DaffaCatering.Blazor.Services.Penjualan
{
    public class LaporanPenjualanService : ApiServiceBase
    {
        public LaporanPenjualanService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth) { }

        public async Task<(List<LaporanKelompok>? Data, string Error)> AmbilAsync(
            string jenis, string kelompok, DateOnly dari, DateOnly sampai, string metode, string status)
        {
            await SiapkanTokenAsync();

            var url = "api/LaporanPenjualan"
                + $"?jenis={jenis}&kelompok={kelompok}"
                + $"&dari={dari.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}"
                + $"&sampai={sampai.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}"
                + $"&metode={Uri.EscapeDataString(metode)}"
                + $"&status={Uri.EscapeDataString(status)}";

            var response = await Http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var pesan = await response.Content.ReadAsStringAsync();
                return (null, string.IsNullOrWhiteSpace(pesan)
                    ? $"Server menjawab {(int)response.StatusCode}"
                    : pesan);
            }

            var data = await response.Content.ReadFromJsonAsync<List<LaporanKelompok>>() ?? new();
            return (data, "");
        }
    }
}