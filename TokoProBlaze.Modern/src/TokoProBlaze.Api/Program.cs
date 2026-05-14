using TokoProBlaze.Application.Customers;
using TokoProBlaze.Infrastructure;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<ICustomerRepository>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = ResolveLegacyConnectionString(configuration);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return new FallbackCustomerRepository();
    }

    return new LegacyMySqlCustomerRepository(connectionString);
});
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();

var api = app.MapGroup("/api");

api.MapGet("/customers", async (string? q, ICustomerService customerService, CancellationToken cancellationToken) =>
{
    var customers = await customerService.GetCustomersAsync(q, cancellationToken);
    return Results.Ok(customers);
}).WithName("GetCustomers");

api.MapGet("/customers/page", async (string? q, int page, int pageSize, ICustomerService customerService, CancellationToken cancellationToken) =>
{
    var currentPage = page < 1 ? 1 : page;
    var currentPageSize = pageSize is < 1 or > 200 ? 20 : pageSize;
    var customers = await customerService.GetCustomersAsync(q, cancellationToken);
    var totalItems = customers.Count;
    var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)currentPageSize);
    if (currentPage > totalPages)
    {
        currentPage = totalPages;
    }

    var items = customers
        .Skip((currentPage - 1) * currentPageSize)
        .Take(currentPageSize)
        .ToArray();

    return Results.Ok(new
    {
        page = currentPage,
        pageSize = currentPageSize,
        totalItems,
        totalPages,
        items
    });
}).WithName("GetCustomersPaged");

api.MapGet("/customers/detail", async (string code, ICustomerService customerService, CancellationToken cancellationToken) =>
{
    var customer = await customerService.GetCustomerByCodeAsync(code, cancellationToken);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
}).WithName("GetCustomerByCode");

api.MapGet("/health/legacy-db", (IConfiguration configuration) =>
{
    var connectionString = ResolveLegacyConnectionString(configuration);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return Results.Ok(new
        {
            isHealthy = false,
            mode = "fallback",
            message = "Connection string LegacyMySql belum diisi."
        });
    }

    try
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        return Results.Ok(new
        {
            isHealthy = true,
            mode = "realtime",
            message = "Koneksi ke legacy DB berhasil."
        });
    }
    catch (Exception ex)
    {
        return Results.Ok(new
        {
            isHealthy = false,
            mode = "fallback",
            message = $"Koneksi legacy DB gagal: {ex.Message}"
        });
    }
}).WithName("GetLegacyDbHealth");

app.Run();

static string? ResolveLegacyConnectionString(IConfiguration configuration)
{
    var env = Environment.GetEnvironmentVariable("TOKOPRO_LEGACY_MYSQL");
    if (!string.IsNullOrWhiteSpace(env))
    {
        return env;
    }

    var config = configuration.GetConnectionString("LegacyMySql");
    if (string.IsNullOrWhiteSpace(config) || config.StartsWith("USE_ENV_", StringComparison.OrdinalIgnoreCase))
    {
        return null;
    }

    return config;
}
