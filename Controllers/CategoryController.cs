using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using ECommerceWebAPI.DTOs; 
using AutoMapper;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public CategoryController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            
            // Map the list of entities to a list of DTOs
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) return NotFound();

            return _mapper.Map<CategoryDto>(category);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id) return BadRequest();

            var categoryInDb = await _context.Categories.FindAsync(id);
            if (categoryInDb == null) return NotFound();

            // Use AutoMapper to map the DTO onto the existing tracked entity
            _mapper.Map(categoryDto, categoryInDb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> PostCategory(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Map back to DTO for the response
            var responseDto = _mapper.Map<CategoryDto>(category);
            
            return CreatedAtAction(nameof(GetCategory), new { id = responseDto.Id }, responseDto);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id) => _context.Categories.Any(e => e.Id == id);
    }
}