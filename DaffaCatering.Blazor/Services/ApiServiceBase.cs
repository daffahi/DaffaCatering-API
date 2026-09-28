using System.Net.Http.Headers;

namespace DaffaCatering.Blazor.Services
{
    public abstract class ApiServiceBase
    {
        protected readonly HttpClient Http;
        private readonly JwtAuthStateProvider _auth;

        protected ApiServiceBase(IHttpClientFactory factory, JwtAuthStateProvider auth)
        {
            Http = factory.CreateClient("API");
            _auth = auth;
        }

        protected async Task SiapkanTokenAsync()
        {
            var token = await _auth.AmbilTokenAsync();
            Http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
        }
    }
}