using DaffaCatering.Blazor.Models.MasterPelanggan;
using DaffaCatering.Blazor.Models.MasterPemasok;
using System.Net.Http.Json;

namespace DaffaCatering.Blazor.Services.MasterPemasok
{
    public class DaftarPemasokService : ApiServiceBase
    {
        public DaftarPemasokService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth) { }

        public async Task<(List<DaftarPemasokModels>? Data, string Error)> AmbilAsync(string status)
        {
            await SiapkanTokenAsync();

            var response = await Http.GetAsync($"api/DaftarPemasok?status={status}");
            if (!response.IsSuccessStatusCode)
            {
                var pesan = await response.Content.ReadAsStringAsync();
                return (null, string.IsNullOrWhiteSpace(pesan)
                    ? $"Server menjawab {(int)response.StatusCode}"
                    : pesan);
            }

            var data = await response.Content.ReadFromJsonAsync<List<DaftarPemasokModels>>() ?? new();
            return (data, "");
        }
    }
}