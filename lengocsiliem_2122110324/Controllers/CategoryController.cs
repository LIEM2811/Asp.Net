using lengocsiliem_2122110324.Data;
using lengocsiliem_2122110324.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // POST api/<CategoryController>
        [HttpPost]
        public IActionResult Post([FromBody] CategoryDTO dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Image = dto.Image,
                CreatedAt = DateTime.Now,
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return Ok(new { message = "Category created", category });
        }


        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] CategoryDTO dto)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            category.Name = dto.Name;
            category.Image = dto.Image;
            category.UpdatedAt = DateTime.Now; 

            _context.Categories.Update(category);
            _context.SaveChanges();

            return Ok(new { message = "Category updated", category });
        }


        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(new { message = "Category deleted", category });
        }

    }
}
