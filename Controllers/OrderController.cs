using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using ECommerceWebAPI.DTOs;
using AutoMapper;
using ECommerceWebAPI.Data;
using MongoDB.Driver;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        //private readonly MongoDbService _mongoService;
        private readonly OrderService _orderService;

        public OrderController(IMapper mapper, OrderService orderService)
        {
            //_mongoService = mongoService;
            _mapper = mapper;
            _orderService = orderService;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(int.TryParse(id, out int orderId) ? orderId : 0);
            if (orderId == 0) return NotFound();
            return Ok(order);
        }

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> PlaceOrder([FromBody] OrderRequestDto request)
        {
            // The Controller only handles the "Result"
            var result = await _orderService.CreateOrderAsync(request);
            
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var order = await _orderService.DeleteOrderAsync(int.TryParse(id, out int orderId) ? orderId : 0);
            if (order == null) return NotFound();
            return NoContent();
        }


        //GET: api/Order/ByCustomer/5
        [HttpGet("ByCustomer/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrdersByCustomer(string customerId)
        {
            var orders = await _orderService.GetCustomerOrders(int.TryParse(customerId, out int customerIdInt) ? customerIdInt : 0);
            if (orders == null) return NotFound();
            return Ok(orders);
        }

        //PATCH: api/Order/Status/5
        [HttpPatch("Status/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] string newStatus)
        {
            var order = await _orderService.UpdateOrderStatusAsync(int.TryParse(id, out int orderId) ? orderId : 0, newStatus);
            if (order == null) return NotFound();   
            return NoContent();
        }
    }
}