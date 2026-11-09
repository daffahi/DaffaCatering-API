using DaffaCatering.Blazor.Models.Persediaan;
using System.Globalization;
using System.Net.Http.Json;

namespace DaffaCatering.Blazor.Services.Persediaan
{
    public class LaporanPersediaanService : ApiServiceBase
    {
        public LaporanPersediaanService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth) { }

        // dari & sampai null = semua periode
        // status hanya dipakai jenis kadaluwarsa (sudah | 7hari | 30hari)
        public async Task<(List<LaporanPersediaanKelompok>? Data, string Error)> AmbilAsync(
            string jenis, DateOnly? dari, DateOnly? sampai, string? status = null)
        {
            await SiapkanTokenAsync();

            var url = $"api/LaporanPersediaan?jenis={jenis}";

            if (dari is not null && sampai is not null)
                url += $"&dari={dari.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}"
                     + $"&sampai={sampai.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";

            if (!string.IsNullOrEmpty(status))
                url += $"&status={status}";

            var response = await Http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var pesan = await response.Content.ReadAsStringAsync();
                return (null, string.IsNullOrWhiteSpace(pesan)
                    ? $"Server menjawab {(int)response.StatusCode}"
                    : pesan);
            }

            var data = await response.Content.ReadFromJsonAsync<List<LaporanPersediaanKelompok>>() ?? new();
            return (data, "");
        }
    }
}