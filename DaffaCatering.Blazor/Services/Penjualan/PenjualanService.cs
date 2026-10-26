using DaffaCatering.Blazor.Models.Penjualan;

namespace DaffaCatering.Blazor.Services.Penjualan
{
    public class PenjualanService : CrudServiceBase<PenjualanModels>
    {
        public PenjualanService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderPenjualan") { }
    }
}
