using TokoProBlaze.Domain.Customers;

namespace TokoProBlaze.Application.Customers;

public sealed record CustomerDto(string Code, string Name, string City, bool IsActive);

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetCustomersAsync(string? keyword, CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetCustomerByCodeAsync(string code, CancellationToken cancellationToken = default);
}

public sealed class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerDto>> GetCustomersAsync(string? keyword, CancellationToken cancellationToken = default)
    {
        var customers = await customerRepository.GetAllAsync(cancellationToken);
        IEnumerable<Customer> query = customers;

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c =>
                c.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                c.City.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderBy(c => c.Name)
            .Select(c => new CustomerDto(c.Code, c.Name, c.City, c.IsActive))
            .ToArray();
    }

    public async Task<CustomerDto?> GetCustomerByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var customers = await customerRepository.GetAllAsync(cancellationToken);
        var customer = customers.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        return customer is null
            ? null
            : new CustomerDto(customer.Code, customer.Name, customer.City, customer.IsActive);
    }
}
