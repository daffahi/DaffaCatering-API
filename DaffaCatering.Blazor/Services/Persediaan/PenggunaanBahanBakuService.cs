using DaffaCatering.Blazor.Models.Persediaan;

namespace DaffaCatering.Blazor.Services.Persediaan
{
    public class PenggunaanBahanBakuService : CrudServiceBase<PenggunaanBahanBakuModels>
    {
        public PenggunaanBahanBakuService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderPenggunaan") { }
    }
}