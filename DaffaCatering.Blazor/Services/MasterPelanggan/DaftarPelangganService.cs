using DaffaCatering.Blazor.Models.MasterPelanggan;
using System.Net.Http.Json;

namespace DaffaCatering.Blazor.Services.MasterPelanggan
{
    public class DaftarPelangganService : ApiServiceBase
    {
        public DaftarPelangganService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth) { }

        public async Task<(List<DaftarPelangganModels>? Data, string Error)> AmbilAsync(string status)
        {
            await SiapkanTokenAsync();

            var response = await Http.GetAsync($"api/DaftarPelanggan?status={status}");
            if (!response.IsSuccessStatusCode)
            {
                var pesan = await response.Content.ReadAsStringAsync();
                return (null, string.IsNullOrWhiteSpace(pesan)
                    ? $"Server menjawab {(int)response.StatusCode}"
                    : pesan);
            }

            var data = await response.Content.ReadFromJsonAsync<List<DaftarPelangganModels>>() ?? new();
            return (data, "");
        }
    }
}