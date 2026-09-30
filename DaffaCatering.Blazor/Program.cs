using Microsoft.AspNetCore.Components.Authorization;
using DaffaCatering.Blazor.Components;
using DaffaCatering.Blazor.Services;
using DaffaCatering.Blazor.Services.MasterBahanBaku;
using DaffaCatering.Blazor.Services.MasterPelanggan;
using DaffaCatering.Blazor.Services.MasterPemasok;
using DaffaCatering.Blazor.Services.MasterSatuanBahanBaku;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
});

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<JwtAuthStateProvider>());

builder.Services.AddScoped<PelangganService>();
builder.Services.AddScoped<PemasokService>();
builder.Services.AddScoped<BahanBakuService>();
builder.Services.AddScoped<SatuanBahanBakuService>();
builder.Services.AddScoped<KonversiSatuanService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
