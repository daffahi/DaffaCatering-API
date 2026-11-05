using DaffaCatering.Blazor.Models.Persediaan;

namespace DaffaCatering.Blazor.Services.Persediaan
{
    public class PenggunaanService : CrudServiceBase<PenggunaanBahanBakuModels>
    {
        public PenggunaanService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderPenggunaan") { }
    }
}