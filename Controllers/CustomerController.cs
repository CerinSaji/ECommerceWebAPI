using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using ECommerceWebAPI.DTOs;
using AutoMapper;
using ECommerceWebAPI.Data;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        //private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly CustomerService _customerService;
        //private readonly MongoDbService _mongoService;

        public CustomerController(CustomerService customerService, IMapper mapper)
        {
            //_mongoService = mongoService;
            _mapper = mapper;
            _customerService = customerService;
        }
        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _customerService.GetCustomersAsync();
            return Ok(customers);
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerDto customerDto)
        {
            await _customerService.UpdateCustomerAsync(id, customerDto);
            return NoContent();
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> PostCustomer(CustomerDto customerDto)
        {
            var customer = await _customerService.AddCustomerAsync(customerDto);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _customerService.DeleteCustomerAsync(id);
            if (customer == null) return NotFound();
            return NoContent();
        }

    }
}