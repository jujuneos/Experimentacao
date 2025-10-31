public class OrderService
{
    private readonly ICustomerRepository customerRepository;

    public OrderService(ICustomerRepository customerRepository)
    {
        this.customerRepository = customerRepository;
    }

    public decimal CalculateFinalPrice(Guid customerId, decimal subtotal)
    {
        if (subtotal < 0)
            throw new ArgumentException("Subtotal cannot be negative.");

        var customer = customerRepository.GetById(customerId);
        if (customer == null)
            throw new InvalidOperationException("Customer not found.");

        decimal discount = 0m;

        if (customer.IsPremium)
            discount = subtotal * 0.15m;
        else if (subtotal > 1000)
            discount = subtotal * 0.05m;

        return subtotal - discount;
    }
}
