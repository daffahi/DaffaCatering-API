using DaffaCatering.Blazor.Models.MasterBahanBaku;

namespace DaffaCatering.Blazor.Services.MasterBahanBaku
{
    public class BahanBakuService : CrudServiceBase<BahanBakuModels>
    {
        public BahanBakuService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/HeaderBahanBaku") { }
    }
}