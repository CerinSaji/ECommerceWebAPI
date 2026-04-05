using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using ECommerceWebAPI.DTOs; 
using AutoMapper;
using ECommerceWebAPI.Data;
using MongoDB.Driver;

namespace ECommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        //private readonly ApplicationContext _context;
        private readonly MongoDbService _mongoService;
        private readonly IMapper _mapper; 

        public ProductController(MongoDbService mongoService, IMapper mapper) 
        {
            _mongoService = mongoService;
            _mapper = mapper;
        }

        // GET: api/Product
        [HttpGet]
        //product parameters extends pagination parameters, so it includes page number and page size for pagination, 
        //and filters for searching and filtering products
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts([FromQuery] ProductParameters filter)
        {
            var filterBuilder = Builders<Product>.Filter;
            var filterDef = filterBuilder.Empty; // Start with everything

            // 1. Filter by Name (Case-Insensitive Search)
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                filterDef &= filterBuilder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(filter.SearchTerm, "i"));
            }

            // 2. Filter by Category
            if (!string.IsNullOrEmpty(filter.CategoryId))
            {
                filterDef &= filterBuilder.Eq(p => p.CategoryId, filter.CategoryId);
            }

            // 3. Filter by Price Range (Greater Than / Less Than)
            if (filter.MinPrice.HasValue)
            {
                filterDef &= filterBuilder.Gte(p => p.Price, filter.MinPrice.Value);
            }
            if (filter.MaxPrice.HasValue)
            {
                filterDef &= filterBuilder.Lte(p => p.Price, filter.MaxPrice.Value);
            }

            //sort by price ascending or descending
            var sortBuilder = Builders<Product>.Sort;
            var sortDef = filter.SortBy?.ToLower() switch
            {
                "price_asc" => sortBuilder.Ascending(p => p.Price),
                "price_desc" => sortBuilder.Descending(p => p.Price),
                _ => null // No sorting
            };


            // finallly, execute Query with Pagination
            var products = await _mongoService.Products.Find(filterDef)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Limit(filter.PageSize)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProductResponseDto>>(products));
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct([FromRoute] string id)
        {
            if(string.IsNullOrEmpty(id))
                return BadRequest("Invalid product ID.");

            var product = await _mongoService.Products
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null) 
                return NotFound(new { message = $"Product with ID {id} was not found." });

            return _mapper.Map<ProductResponseDto>(product);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> PostProduct(ProductCreateDto productDto) //from ProductCreateDto instead of Product
        {
            var category = await _mongoService.Categories
                .Find(c => c.Id == productDto.CategoryId)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                //invalid category id
                return BadRequest($"Validation Error: Category with ID '{productDto.CategoryId}' does not exist.");
            }

            var existingProduct = await _mongoService.Products
                .Find(p => p.Name.ToLower() == productDto.Name.ToLower()) //case insensitive check
                .FirstOrDefaultAsync();
            
            if (existingProduct != null)
            {
                return Conflict($"A product with the name '{productDto.Name}' already exists. Please update the existing product or choose a different name.");
            }

            var product = _mapper.Map<Product>(productDto);
            
            await _mongoService.Products.InsertOneAsync(product);

            var response = _mapper.Map<ProductResponseDto>(product);
            return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, response);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(string id, ProductCreateDto productDto)
        {
            var product = await _mongoService.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null) return NotFound();

            //update existing entity with DTO values
            _mapper.Map(productDto, product);

            // try
            // {
            //     await _context.SaveChangesAsync();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!ProductExists(id)) return NotFound();
            //     else throw;
            // }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            // var product = await _context.Products.FindAsync(id);
            // if (product == null) return NotFound();

            // _context.Products.Remove(product);
            // await _context.SaveChangesAsync();

            // return NoContent();

            var result = await _mongoService.Products.DeleteOneAsync(x => x.Id == id);
    
            if (result.DeletedCount == 0) return NotFound();

            return NoContent();
        }

        private async Task<bool> ProductExists(string id) 
        {
            return await _mongoService.Products.Find(e => e.Id == id).AnyAsync();
        }
    }
}