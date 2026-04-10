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
        private readonly CategoryService _categoryService;


        public CategoryController(CategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(string id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(string id, CategoryDto categoryDto)
        {
            await _categoryService.UpdateCategoryAsync(int.TryParse(id, out int categoryId) ? categoryId : 0, categoryDto);
            return NoContent();
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> PostCategory(CategoryDto categoryDto)
        {
            var category = await _categoryService.AddCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _categoryService.DeleteCategoryAsync(int.TryParse(id, out int categoryId) ? categoryId : 0);
            return NoContent();
        }

        //private bool CategoryExists(string id) => _mongoService.Categories.Any(e => e.Id == id);
    }
}