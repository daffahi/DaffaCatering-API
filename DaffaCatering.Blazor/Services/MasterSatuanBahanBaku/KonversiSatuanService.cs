using DaffaCatering.Blazor.Models.MasterSatuanBahanBaku;

namespace DaffaCatering.Blazor.Services.MasterSatuanBahanBaku
{
    public class KonversiSatuanService : CrudServiceBase<KonversiSatuanModels>
    {
        public KonversiSatuanService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/KonversiSatuan") { }
    }
}