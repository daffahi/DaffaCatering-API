using DaffaCatering.Blazor.Models.Penjualan;

namespace DaffaCatering.Blazor.Services.Penjualan
{
    public class PesananPenjualanService : CrudServiceBase<PesananPenjualanModels>
    {
        public PesananPenjualanService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderPesanan") { }
    }
}