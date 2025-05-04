using AssesstmentMicroservices.Application.DTO;
using AssesstmentMicroservices.Application.Interfaces;
using AssesstmentMicroservices.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AssesstmentMicroservices.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var orderId = await _orderService.CreateOrderAsync(dto);
            return Ok(new { orderId });
        }
    }
}
