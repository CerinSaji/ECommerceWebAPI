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
    public class CategoryController : ControllerBase
    {
        //private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly MongoDbService _mongoService;


        public CategoryController(MongoDbService mongoService, IMapper mapper)
        {
            _mongoService = mongoService;
            _mapper = mapper;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _mongoService.Categories
                .Find(_ => true)
                .ToListAsync();
            
            // Map the list of entities to a list of DTOs
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(string id)
        {
            var category = await _mongoService.Categories
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (category == null) return NotFound();

            return _mapper.Map<CategoryDto>(category);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(string id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id) return BadRequest();

            var categoryInDb = await _mongoService.Categories
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (categoryInDb == null) return NotFound();

            // Use AutoMapper to map the DTO onto the existing tracked entity
            _mapper.Map(categoryDto, categoryInDb);

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!CategoryExists(id)) return NotFound();
            //     else throw;
            // }

            return NoContent();
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> PostCategory(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            await _mongoService.Categories.InsertOneAsync(category);

            // Map back to DTO for the response
            var responseDto = _mapper.Map<CategoryDto>(category);
            
            return CreatedAtAction(nameof(GetCategory), new { id = responseDto.Id }, responseDto);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var category = await _mongoService.Categories
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (category == null) return NotFound();

            //cannot delete category if it is referenced by any products
            var productsWithCategory = await _mongoService.Products
                .Find(p => p.CategoryId == id)
                .AnyAsync();    
            
            if (productsWithCategory)
                return Conflict($"Cannot delete category with ID '{id}' because it is referenced by existing products. Please reassign or delete those products first.");

            await _mongoService.Categories.DeleteOneAsync(c => c.Id == id);
            // await _mongoService.SaveChangesAsync();

            return NoContent();
        }

        //private bool CategoryExists(string id) => _mongoService.Categories.Any(e => e.Id == id);
        private async Task<bool> CategoryExistsAsync(string id) => await _mongoService.Categories.Find(e => e.Id == id).AnyAsync();
    }
}