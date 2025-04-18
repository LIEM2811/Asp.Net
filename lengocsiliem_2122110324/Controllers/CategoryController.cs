using lengocsiliem_2122110324.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lengocsiliem_2122110324.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<IEnumerable<Category>> Get()
        {
            return await _context.Categories.ToListAsync();
        }

        // Get: api/<CategoryController>/id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }
            return Ok(category);
        }

        // POST api/<CategoryController>
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")] // Chỉ admin mới được tạo
        public IActionResult Post([FromBody] Category cat)
        {
            var category = new Category
            {
                Name = cat.Name,
                Image = cat.Image,
                CreatedAt = DateTime.Now,
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return Ok(new { message = "Category created", category });
        }

        // PUT: api/<CategoryController>/id
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")] // Chỉ admin mới được sửa
        public IActionResult Put(int id, [FromBody] Category cat)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            category.Name = cat.Name;
            category.Image = cat.Image;
            category.UpdatedAt = DateTime.Now;

            _context.Categories.Update(category);
            _context.SaveChanges();

            return Ok(new { message = "Category updated", category });
        }

        // DELETE: api/<CategoryController>/id
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")] // Chỉ admin mới được xóa
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(new { message = "Xóa Category thành công" });
        }
    }
}