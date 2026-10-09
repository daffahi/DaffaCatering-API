using DaffaCatering.Blazor.Models.MasterBahanBaku;

namespace DaffaCatering.Blazor.Services.MasterBahanbaku
{
    public class PenyesuaianBahanBakuService : CrudServiceBase<PenyesuaianBahanBakuModels>
    {
        public PenyesuaianBahanBakuService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderPenyesuaianBahanBaku") { }
    }
}