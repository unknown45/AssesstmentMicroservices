using AssesstmentMicroservices.Application.DTO;
using AssesstmentMicroservices.Application.Interfaces;
using AssesstmentMicroservices.Domain.Entities;
using Moq;

namespace AssesstmentMicroservices.Tests;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_Create_Order_When_Valid()
    {
        var mockUow = new Mock<IUnitOfWork>();
        var customerRepo = new Mock<IGenericRepository<Customer>>();
        var productRepo = new Mock<IGenericRepository<Product>>();
        var orderRepo = new Mock<IGenericRepository<Order>>();

        var customer = new Customer { Id = 1, Name = "Test" };
        var products = new List<Product> { new() { Id = 10, Name = "Husni", Price = 100 } };

        mockUow.Setup(x => x.Customers).Returns(customerRepo.Object);
        mockUow.Setup(x => x.Products).Returns(productRepo.Object);
        mockUow.Setup(x => x.Orders).Returns(orderRepo.Object);

        customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
        productRepo.Setup(x => x.FindAsync(It.IsAny<Func<Product, bool>>())).ReturnsAsync(products);

        var service = new OrderService(mockUow.Object);
        var dto = new CreateOrderDto { CustomerId = 1, ProductIds = new() { 10 } };

        var result = await service.CreateOrderAsync(dto);

        result.Should().BeGreaterThan(0);
        orderRepo.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
        mockUow.Verify(x => x.SaveChangesAsync(), Times.Once);
        mockUow.Verify(x => x.CommitAsync(), Times.Once);
    }
}
