using System.Linq;
using AssesstmentMicroservices.Application.DTO;
using AssesstmentMicroservices.Application.Events;
using AssesstmentMicroservices.Application.Interfaces;
using AssesstmentMicroservices.Domain.Entities;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IMessagePublisher _publisher;

    public OrderService(
        IUnitOfWork uow,
        IMessagePublisher publisher)
    {
        _uow = uow;
        _publisher = publisher;
    }

    public async Task<int> CreateOrderAsync(CreateOrderDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.ProductIds == null || !dto.ProductIds.Any())
            throw new ArgumentException("Product IDs cannot be empty", nameof(dto.ProductIds));

        await _uow.BeginTransactionAsync();

        try
        {
            // 1. Validasi Customer
            var customer = await _uow.Customers.GetByIdAsync(dto.CustomerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {dto.CustomerId} not found");
            }

            // 2. Validasi Products
            var products = (await _uow.Products
                .FindAsync(p => dto.ProductIds.Contains(p.Id)))
                .ToList();

            var missingProductIds = dto.ProductIds.Except(products.Select(p => p.Id)).ToList();
            if (missingProductIds.Any())
            {
                throw new KeyNotFoundException($"Products not found: {string.Join(",", missingProductIds)}");
            }

            // 3. Buat Order
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.UtcNow,
                OrderProducts = products.Select(p => new OrderProduct
                {
                    ProductId = p.Id
                }).ToList()
            };

            // 4. Simpan ke Database
            await _uow.Orders.AddAsync(order);
            await _uow.SaveChangesAsync();

            // 5. Publish Event (outbox pattern recommended for production)
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                ProductIds = products.Select(p => p.Id).ToList(),
                CreatedAt = order.OrderDate
            };

            try
            {
                await _publisher.PublishOrderCreatedAsync(orderEvent);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // 6. Commit Transaction
            await _uow.CommitAsync();

            return order.Id;
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            throw; // Re-throw untuk error handling di layer atas
        }
    }
}