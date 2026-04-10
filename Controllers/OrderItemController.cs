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
        private readonly OrderItemService _orderItemService;
        private readonly OrderService _orderService;

        public OrderItemController(OrderItemService orderItemService, IMapper mapper, OrderService orderService)
        {
            _orderItemService = orderItemService;
            _mapper = mapper;
            _orderService = orderService;
        }

        // GET: api/OrderItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemResponseDto>>> GetOrderItems()
        {
            var orderItems = await _orderItemService.GetOrderItemsAsync();
            return Ok(orderItems);
        }

        // GET: api/OrderItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemResponseDto>> GetOrderItem(int id)
        {
            var orderItem = await _orderItemService.GetOrderItemByIdAsync(id);
            if (orderItem == null) return NotFound();
            return Ok(orderItem);
        }

        // GET: api/OrderItem/ByOrder/5
        [HttpGet("ByOrder/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemResponseDto>>> GetItemsByOrder(int orderId)
        {
            //first get the order
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound($"Order {orderId} not found.");
            //then get the items for that order
            var orderItems = order.Items.Select(oi => _mapper.Map<OrderItemResponseDto>(oi));
            return Ok(orderItems);
        }
    }
}