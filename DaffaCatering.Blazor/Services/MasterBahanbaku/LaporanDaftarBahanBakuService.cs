using DaffaCatering.Blazor.Models.MasterBahanBaku;

namespace DaffaCatering.Blazor.Services.MasterBahanbaku
{
    public class LaporanDaftarBahanBakuService : ApiServiceBase
    {
        public LaporanDaftarBahanBakuService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth) { }

        public async Task<(List<LaporanDaftarBahanBakuModels>? Data, string Error)> AmbilAsync()
        {
            await SiapkanTokenAsync();

            var response = await Http.GetAsync("api/LaporanDaftarBahanBaku");
            if (!response.IsSuccessStatusCode)
            {
                var pesan = await response.Content.ReadAsStringAsync();
                return (null, string.IsNullOrWhiteSpace(pesan)
                    ? $"Server menjawab {(int)response.StatusCode}"
                    : pesan);
            }

            var data = await response.Content.ReadFromJsonAsync<List<LaporanDaftarBahanBakuModels>>() ?? new();
            return (data, "");
        }
    }
}