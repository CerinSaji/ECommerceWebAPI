using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using ECommerceWebAPI.DTOs;
using AutoMapper;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public OrderItemController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/OrderItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemResponseDto>>> GetOrderItems()
        {
            var items = await _context.OrderItems
                .Include(oi => oi.Product)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<OrderItemResponseDto>>(items));
        }

        // GET: api/OrderItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemResponseDto>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.Id == id);

            if (orderItem == null) return NotFound();

            return _mapper.Map<OrderItemResponseDto>(orderItem);
        }

        // GET: api/OrderItem/ByOrder/5
        [HttpGet("ByOrder/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemResponseDto>>> GetItemsByOrder(int orderId)
        {
            var items = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                .ToListAsync();

            if (!items.Any()) return NotFound($"No items found for Order ID {orderId}");

            return Ok(_mapper.Map<IEnumerable<OrderItemResponseDto>>(items));
        }
    }
}