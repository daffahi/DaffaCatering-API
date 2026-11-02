using DaffaCatering.Blazor.Models.Pembelian;
using System.Globalization;
using System.Net.Http.Json;

namespace DaffaCatering.Blazor.Services.Pembelian
{
    public class LaporanPembelianService : ApiServiceBase
    {
        public LaporanPembelianService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth) { }

        public async Task<(List<LaporanPembelianKelompok>? Data, string Error)> AmbilAsync(
            string jenis, string kelompok, DateOnly dari, DateOnly sampai)
        {
            await SiapkanTokenAsync();

            var url = "api/LaporanPembelian"
                + $"?jenis={jenis}&kelompok={kelompok}"
                + $"&dari={dari.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}"
                + $"&sampai={sampai.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";

            var response = await Http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var pesan = await response.Content.ReadAsStringAsync();
                return (null, string.IsNullOrWhiteSpace(pesan)
                    ? $"Server menjawab {(int)response.StatusCode}"
                    : pesan);
            }

            var data = await response.Content.ReadFromJsonAsync<List<LaporanPembelianKelompok>>() ?? new();
            return (data, "");
        }
    }
}