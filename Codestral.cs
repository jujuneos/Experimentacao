using Xunit;
using NSubstitute;
using System;

public class OrderServiceTests
{
    private readonly ICustomerRepository _customerRepository;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _customerRepository = Substitute.For<ICustomerRepository>();
        _orderService = new OrderService(_customerRepository);
    }

    [Fact]
    public void CalculateFinalPrice_PremiumCustomer_AppliesFifteenPercentDiscount()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var subtotal = 1000m;
        var customer = new Customer { Id = customerId, IsPremium = true };
        _customerRepository.GetById(customerId).Returns(customer);

        // Act
        var result = _orderService.CalculateFinalPrice(customerId, subtotal);

        // Assert
        Assert.Equal(850m, result);
    }

    [Fact]
    public void CalculateFinalPrice_NonPremiumCustomerSubtotalOver1000_AppliesFivePercentDiscount()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var subtotal = 1000m;
        var customer = new Customer { Id = customerId, IsPremium = false };
        _customerRepository.GetById(customerId).Returns(customer);

        // Act
        var result = _orderService.CalculateFinalPrice(customerId, subtotal);

        // Assert
        Assert.Equal(950m, result);
    }

    [Fact]
    public void CalculateFinalPrice_NonPremiumCustomerSubtotalUnder1000_NoDiscountApplied()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var subtotal = 500m;
        var customer = new Customer { Id = customerId, IsPremium = false };
        _customerRepository.GetById(customerId).Returns(customer);

        // Act
        var result = _orderService.CalculateFinalPrice(customerId, subtotal);

        // Assert
        Assert.Equal(500m, result);
    }

    [Fact]
    public void CalculateFinalPrice_NegativeSubtotal_ThrowsArgumentException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var subtotal = -100m;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _orderService.CalculateFinalPrice(customerId, subtotal));
        Assert.Equal("Subtotal cannot be negative.", ex.Message);
    }

    [Fact]
    public void CalculateFinalPrice_CustomerNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var subtotal = 100m;
        _customerRepository.GetById(customerId).Returns((Customer)null);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _orderService.CalculateFinalPrice(customerId, subtotal));
        Assert.Equal("Customer not found.", ex.Message);
    }

    [Fact]
    public void CalculateFinalPrice_ZeroSubtotal_NoDiscountApplied()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var subtotal = 0m;
        var customer = new Customer { Id = customerId, IsPremium = true };
        _customerRepository.GetById(customerId).Returns(customer);

        // Act
        var result = _orderService.CalculateFinalPrice(customerId, subtotal);

        // Assert
        Assert.Equal(0m, result);
    }
}

// Supporting classes/interfaces for compilation
public interface ICustomerRepository
{
    Customer GetById(Guid customerId);
}

public class Customer
{
    public Guid Id { get; set; }
    public bool IsPremium { get; set; }
}
