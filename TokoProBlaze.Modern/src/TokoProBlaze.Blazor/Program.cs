using MudBlazor.Services;
using TokoProBlaze.Blazor.Components;
using TokoProBlaze.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices(config =>
{
    // Blazor Web App: @Body interaktif tidak selalu menjadi turunan MudPopoverProvider di pohon komponen.
    // Tanpa ini, MudDataGrid (menu filter / panel kolom) dapat melempar saat provider hanya sejajar di layout.
    config.PopoverOptions.CheckForPopoverProvider = false;
});
builder.Services.AddHttpClient<CustomerApiClient>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<ProductApiClient>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<SupplierApiClient>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(apiBaseUrl);
});

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
