using DaffaCatering.Blazor.Models.MasterPemasok;

namespace DaffaCatering.Blazor.Services.MasterPemasok
{
    public class PemasokService : CrudServiceBase<PemasokModels>
    {
        public PemasokService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/Pemasok") { }
    }
}
