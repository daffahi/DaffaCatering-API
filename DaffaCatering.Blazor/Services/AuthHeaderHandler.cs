namespace DaffaCatering.Blazor.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly JwtAuthStateProvider _authProvider;

        public AuthHeaderHandler(JwtAuthStateProvider authProvider)
        {
            _authProvider = authProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authProvider.AmbilTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}