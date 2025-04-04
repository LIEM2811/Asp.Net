using lengocsiliem_2122110324.Data;
using lengocsiliem_2122110324.Dto;
using Microsoft.AspNetCore.Mvc;
using lengocsiliem_2122110324.Model;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace lengocsiliem_2122110324.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        // GET: api/<ProductController>
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            return await _context.Products.ToListAsync();
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return "Product not found";
            }
            return product.Name;
        }

        // POST api/<ProductController>
        [HttpPost]
        public IActionResult Post([FromBody] ProductDTO dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Image = dto.Image,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                Category = _context.Categories.FirstOrDefault(c => c.Id == dto.CategoryId),
                Description = dto.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(new { message = "Product created", product });
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
