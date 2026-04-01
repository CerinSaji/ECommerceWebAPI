using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using ECommerceWebAPI.DTOs;
using AutoMapper;
using MongoDB.Driver;
using ECommerceWebAPI.Data;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        //private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly MongoDbService _mongoService;

        public OrderItemController(MongoDbService mongoService, IMapper mapper)
        {
            _mongoService = mongoService;
            _mapper = mapper;
        }

        // GET: api/OrderItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemResponseDto>>> GetOrderItems()
        {
            var items = await _mongoService.OrderItems
                .Find(_ => true)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<OrderItemResponseDto>>(items));
        }

        // GET: api/OrderItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemResponseDto>> GetOrderItem(string id)
        {
            var orderItem = await _mongoService.OrderItems
                .Find(oi => oi.Id == id)
                .FirstOrDefaultAsync();

            if (orderItem == null) return NotFound();

            return _mapper.Map<OrderItemResponseDto>(orderItem);
        }

        // GET: api/OrderItem/ByOrder/5
        [HttpGet("ByOrder/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemResponseDto>>> GetItemsByOrder(string orderId)
        {
            var items = await _mongoService.OrderItems
                .Find(oi => oi.OrderId == orderId)
                .ToListAsync();

            if (!items.Any()) return NotFound($"No items found for Order ID {orderId}");

            return Ok(_mapper.Map<IEnumerable<OrderItemResponseDto>>(items));
        }
    }
}