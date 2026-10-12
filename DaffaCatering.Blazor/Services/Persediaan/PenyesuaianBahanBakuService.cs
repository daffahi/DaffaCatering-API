using DaffaCatering.Blazor.Models.Persediaan;

namespace DaffaCatering.Blazor.Services.Persediaan
{
    public class PenyesuaianBahanBakuService : CrudServiceBase<PenyesuaianBahanBakuModels>
    {
        public PenyesuaianBahanBakuService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderPenyesuaianBahanBaku") { }
    }
}