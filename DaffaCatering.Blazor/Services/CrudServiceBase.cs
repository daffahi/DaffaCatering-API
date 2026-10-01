using System.Net.Http.Json;

namespace DaffaCatering.Blazor.Services
{
    public abstract class CrudServiceBase<T> : ApiServiceBase where T : class
    {
        private readonly string _endpoint;

        protected CrudServiceBase(IHttpClientFactory factory, JwtAuthStateProvider auth, string endpoint)
            : base(factory, auth)
        {
            _endpoint = endpoint;
        }

        public async Task<List<T>> GetAllAsync()
        {
            await SiapkanTokenAsync();
            return await Http.GetFromJsonAsync<List<T>>(_endpoint) ?? new List<T>();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            await SiapkanTokenAsync();
            var response = await Http.GetAsync($"{_endpoint}/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<(bool Success, string Error)> CreateAsync(T data)
        {
            await SiapkanTokenAsync();
            var response = await Http.PostAsJsonAsync(_endpoint, data);
            return response.IsSuccessStatusCode ? (true, "") : (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<(bool Success, string Error)> UpdateAsync(string id, T data)
        {
            await SiapkanTokenAsync();
            var response = await Http.PutAsJsonAsync($"{_endpoint}/{id}", data);
            return response.IsSuccessStatusCode ? (true, "") : (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<(bool Success, string Error)> DeleteAsync(string id)
        {
            await SiapkanTokenAsync();
            var response = await Http.DeleteAsync($"{_endpoint}/{id}");
            return response.IsSuccessStatusCode ? (true, "") : (false, await response.Content.ReadAsStringAsync());
        }
    }
}