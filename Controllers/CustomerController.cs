using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using ECommerceWebAPI.DTOs;
using AutoMapper;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public CustomerController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customers));
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null) return NotFound();

            return _mapper.Map<CustomerDto>(customer);
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerDto customerDto)
        {
            if (id != customerDto.Id) return BadRequest();

            var customerInDb = await _context.Customers.FindAsync(id);
            if (customerInDb == null) return NotFound();

            // Maps DTO properties onto the existing tracked entity
            _mapper.Map(customerDto, customerInDb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> PostCustomer(CustomerDto customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<CustomerDto>(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = responseDto.Id }, responseDto);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id) => _context.Customers.Any(e => e.Id == id);
    }
}