using DaffaCatering.Blazor.Models.MasterBahanBaku;

namespace DaffaCatering.Blazor.Services.MasterBahanBaku
{
    public class PenggunaanService : CrudServiceBase<PenggunaanModels>
    {
        public PenggunaanService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderPenggunaan") { }
    }
}