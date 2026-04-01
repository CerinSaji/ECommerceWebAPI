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
    public class CustomerController : ControllerBase
    {
        //private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly MongoDbService _mongoService;

        public CustomerController(MongoDbService mongoService, IMapper mapper)
        {
            _mongoService = mongoService;
            _mapper = mapper;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _mongoService.Customers
                .Find(_ => true)
                .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customers));
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(string id)
        {
            var customer = await _mongoService.Customers
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (customer == null) return NotFound();

            return _mapper.Map<CustomerDto>(customer);
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(string id, CustomerDto customerDto)
        {
            if (id != customerDto.Id) return BadRequest();

            var customerInDb = await _mongoService.Customers
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (customerInDb == null) return NotFound();

            // Maps DTO properties onto the existing tracked entity
            _mapper.Map(customerDto, customerInDb);

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!CustomerExists(id)) return NotFound();
            //     else throw;
            // }

            return NoContent();
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> PostCustomer(CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);

            await _mongoService.Customers.InsertOneAsync(customer);

            var responseDto = _mapper.Map<CustomerDto>(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = responseDto.Id }, responseDto);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            var customer = await _mongoService.Customers
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (customer == null) return NotFound();

            //cannot delete a customer if they have existing orders
            var existingOrders = await _mongoService.Orders
                .Find(o => o.CustomerId == id)
                .AnyAsync();

            if (existingOrders)            
                return BadRequest("Cannot delete customer with existing orders. Please delete the orders first.");

            await _mongoService.Customers.DeleteOneAsync(c => c.Id == id);
            // await _mongoService.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> CustomerExistsAsync(string id) => await _mongoService.Customers.Find(e => e.Id == id).AnyAsync();
    }
}