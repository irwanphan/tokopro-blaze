using TokoProBlaze.Application.Customers;
using TokoProBlaze.Application.Products;
using TokoProBlaze.Application.Suppliers;
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
builder.Services.AddScoped<IProductRepository>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = ResolveLegacyConnectionString(configuration);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return new FallbackProductRepository();
    }

    return new LegacyMySqlProductRepository(connectionString);
});
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISupplierRepository>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = ResolveLegacyConnectionString(configuration);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return new FallbackSupplierRepository();
    }

    return new LegacyMySqlSupplierRepository(connectionString);
});
builder.Services.AddScoped<ISupplierService, SupplierService>();
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

api.MapGet("/products", async (string? q, IProductService productService, CancellationToken cancellationToken) =>
{
    var products = await productService.GetProductsAsync(q, cancellationToken);
    return Results.Ok(products);
}).WithName("GetProducts");

api.MapGet("/products/page", async (string? q, int page, int pageSize, IProductService productService, CancellationToken cancellationToken) =>
{
    var currentPage = page < 1 ? 1 : page;
    var currentPageSize = pageSize is < 1 or > 200 ? 20 : pageSize;
    var products = await productService.GetProductsAsync(q, cancellationToken);
    var totalItems = products.Count;
    var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)currentPageSize);
    if (currentPage > totalPages)
    {
        currentPage = totalPages;
    }

    var items = products
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
}).WithName("GetProductsPaged");

api.MapGet("/products/detail", async (string code, IProductService productService, CancellationToken cancellationToken) =>
{
    var product = await productService.GetProductByCodeAsync(code, cancellationToken);
    return product is null ? Results.NotFound() : Results.Ok(product);
}).WithName("GetProductByCode");

api.MapGet("/suppliers", async (string? q, ISupplierService supplierService, CancellationToken cancellationToken) =>
{
    var suppliers = await supplierService.GetSuppliersAsync(q, cancellationToken);
    return Results.Ok(suppliers);
}).WithName("GetSuppliers");

api.MapGet("/suppliers/page", async (string? q, int page, int pageSize, ISupplierService supplierService, CancellationToken cancellationToken) =>
{
    var currentPage = page < 1 ? 1 : page;
    var currentPageSize = pageSize is < 1 or > 200 ? 20 : pageSize;
    var suppliers = await supplierService.GetSuppliersAsync(q, cancellationToken);
    var totalItems = suppliers.Count;
    var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)currentPageSize);
    if (currentPage > totalPages)
    {
        currentPage = totalPages;
    }

    var items = suppliers
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
}).WithName("GetSuppliersPaged");

api.MapGet("/suppliers/detail", async (string code, ISupplierService supplierService, CancellationToken cancellationToken) =>
{
    var supplier = await supplierService.GetSupplierByCodeAsync(code, cancellationToken);
    return supplier is null ? Results.NotFound() : Results.Ok(supplier);
}).WithName("GetSupplierByCode");

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
