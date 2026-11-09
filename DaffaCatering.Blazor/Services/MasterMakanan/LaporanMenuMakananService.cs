using DaffaCatering.Blazor.Models.MasterMakanan;
using System.Net.Http.Json;

namespace DaffaCatering.Blazor.Services.MasterMakanan
{
    public class LaporanMenuMakananService : ApiServiceBase
    {
        public LaporanMenuMakananService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth) { }

        public async Task<(List<LaporanMenuMakananModels>? Data, string Error)> AmbilAsync()
        {
            await SiapkanTokenAsync();

            var response = await Http.GetAsync("api/LaporanMenuMakanan");
            if (!response.IsSuccessStatusCode)
            {
                var pesan = await response.Content.ReadAsStringAsync();
                return (null, string.IsNullOrWhiteSpace(pesan)
                    ? $"Server menjawab {(int)response.StatusCode}"
                    : pesan);
            }

            var data = await response.Content.ReadFromJsonAsync<List<LaporanMenuMakananModels>>() ?? new();
            return (data, "");
        }
    }
}