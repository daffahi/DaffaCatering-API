using DaffaCatering.Blazor.Models;

namespace DaffaCatering.Blazor.Services
{
    public class PemasokService : CrudServiceBase<PemasokModels>
    {
        public PemasokService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/Pemasok") { }
    }
}
