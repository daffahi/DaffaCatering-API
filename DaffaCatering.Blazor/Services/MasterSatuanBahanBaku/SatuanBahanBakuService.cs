using DaffaCatering.Blazor.Models.MasterSatuanBahanBaku;

namespace DaffaCatering.Blazor.Services.MasterSatuanBahanBaku
{
    public class SatuanBahanBakuService : CrudServiceBase<SatuanBahanBakuModels>
    {
        public SatuanBahanBakuService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/SatuanBahanBaku") { }
    }
}