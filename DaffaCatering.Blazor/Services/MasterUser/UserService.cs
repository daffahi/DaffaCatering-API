using DaffaCatering.Blazor.Models.MasterUser;
using System.Net.Http.Json;

namespace DaffaCatering.Blazor.Services.MasterUser
{
    public class UserService : CrudServiceBase<UserModels>
    {
        public UserService(IHttpClientFactory factory, JwtAuthStateProvider auth)
            : base(factory, auth, "api/User") { }

        public async Task<(bool Success, string Error)> ResetPasswordAsync(string id, string passwordBaru)
        {
            await SiapkanTokenAsync();
            var response = await Http.PutAsJsonAsync($"api/User/{id}/password", new { passwordBaru });
            return response.IsSuccessStatusCode ? (true, "") : (false, await response.Content.ReadAsStringAsync());
        }
    }
}