using DaffaCatering.Blazor.Models.MasterPelanggan;

namespace DaffaCatering.Blazor.Services.MasterPelanggan
{
    public class PelangganService : CrudServiceBase<PelangganModels>
    {
        public PelangganService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/Pelanggan") { }
    }
}