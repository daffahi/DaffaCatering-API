using DaffaCatering.Blazor.Models;

namespace DaffaCatering.Blazor.Services
{
    public class PelangganService : CrudServiceBase<PelangganModels>
    {
        public PelangganService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/Pelanggan") { }
    }
}