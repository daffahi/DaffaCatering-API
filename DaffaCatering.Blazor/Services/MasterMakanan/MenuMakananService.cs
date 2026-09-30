using DaffaCatering.Blazor.Models.MasterMakanan;

namespace DaffaCatering.Blazor.Services.MasterMakanan
{
    public class MenuMakananService : CrudServiceBase<MenuMakananModels>
    {
        public MenuMakananService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/MenuMakanan") { }
    }
}