using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using ECommerceWebAPI.DTOs; // Ensure you have this
using AutoMapper;           // Add this

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper; // 1. Add Mapper field

        public ProductController(ApplicationContext context, IMapper mapper) // 2. Inject Mapper
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
        {
            var products = await _context.Products
                .ToListAsync();

            // 3. Map List of Entities to List of DTOs
            return Ok(_mapper.Map<IEnumerable<ProductResponseDto>>(products));
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            // 4. Map single Entity to DTO
            return _mapper.Map<ProductResponseDto>(product);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> PostProduct(ProductCreateDto productDto)
        {
            // 5. Map incoming DTO to Entity
            var product = _mapper.Map<Product>(productDto);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // 6. Map back to ResponseDto for the result
            var response = _mapper.Map<ProductResponseDto>(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, response);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductCreateDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // 7. Update existing entity with DTO values
            _mapper.Map(productDto, product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE stays mostly the same logic-wise
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id) => _context.Products.Any(e => e.Id == id);
    }
}