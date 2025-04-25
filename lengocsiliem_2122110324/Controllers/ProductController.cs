using lengocsiliem_2122110324.Data;
using lengocsiliem_2122110324.Dto;
using Microsoft.AspNetCore.Mvc;
using lengocsiliem_2122110324.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IEnumerable<object>> Get()
        {
            return await _context.Products
                .Include(p => p.Category) // lấy cả category
                .Select(p => new
                {
                    p.Id,
                    p.CategoryId,
                    CategoryName = p.Category.Name, // lấy categoryName
                    p.Name,
                    p.Image,
                    p.Discount,
                    p.SpecialPrice,
                    p.Quantity,
                    p.Price,
                    p.Description,
                    p.CreatedAt,
                    p.UpdatedAt
                })
                .ToListAsync();
        }


        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category) // Include category
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.CategoryId,
                    CategoryName = p.Category.Name, // Include category name
                    p.Name,
                    p.Image,
                    p.Discount,
                    p.SpecialPrice,
                    p.Quantity,
                    p.Price,
                    p.Description,
                    p.CreatedAt,
                    p.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound("Product not found");
            }

            return Ok(product);
        }
        // GET api/Product/category/3
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new
                {
                    p.Id,
                    p.CategoryId,
                    CategoryName = p.Category.Name,
                    p.Name,
                    p.Image,
                    p.Price,
                    p.Discount,
                    p.SpecialPrice,
                    p.Quantity,
                    p.Description,
                    p.CreatedAt,
                    p.UpdatedAt
                })
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No products found for this category.");
            }

            return Ok(products);
        }

        // POST api/<ProductController>
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Post([FromBody] Product pro)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == pro.CategoryId);
            if (category == null)
            {
                return BadRequest("Invalid category ID");
            }

            var product = new Product
            {
                Name = pro.Name,
                Image = pro.Image,
                Price = pro.Price,
                Discount = pro.Discount,
                SpecialPrice = pro.SpecialPrice,
                Quantity = pro.Quantity,
                CategoryId = pro.CategoryId,
                Description = pro.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(new { message = "Product created", product });
        }


        // PUT api/<ProductController>/5
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product pro)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            product.Name = pro.Name;
            product.Image = pro.Image;
            product.Price = pro.Price;
            product.Discount = pro.Discount;
            product.SpecialPrice = pro.SpecialPrice;
            product.Quantity = pro.Quantity;
            product.CategoryId = pro.CategoryId;
            product.Description = pro.Description;
            product.UpdatedAt = DateTime.Now;
            _context.Products.Update(product);
            _context.SaveChanges();

            return Ok(new { message = "Product updated", product });
        }


        // DELETE api/<ProductController>/5
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                NotFound(new { message = "Product not found" });
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        // Upload hình ảnh cho sản phẩm
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateProductImage(int id,IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id ==id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Save file into wwwroot/images
            var fileName = Path.GetFileName(image.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            Directory.CreateDirectory(uploadPath); // Create directory if it doesn't exist

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Update image path in the database
            product.Image = "/images/" + fileName;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            // Return DTO
            var productDto = new ProductDTO
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Image = product.Image
            };

            return Ok(productDto);
        }
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetProductImage(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || string.IsNullOrEmpty(product.Image))
            {
                return NotFound("Product or image not found");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.Image.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Image file not found");
            }

            var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = "image/" + Path.GetExtension(filePath).Trim('.'); // ví dụ: image/jpg, image/png

            return File(imageBytes, contentType);
        }


    }
}
