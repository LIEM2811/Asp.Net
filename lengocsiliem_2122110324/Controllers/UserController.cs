using lengocsiliem_2122110324.Data;
using lengocsiliem_2122110324.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace lengocsiliem_2122110324.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllUsers()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Select(u => new
                {
                    UserId = u.UserId,
                    UserName = u.Name, // Fixed property name
                    Email = u.Email,
                    Roles = u.UserRoles.Select(ur => new
                    {
                        RoleName = ur.Role.RoleName
                    }).ToList()
                })
                .ToListAsync();
        }


        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUser(long id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.UserId == id) // Filter by the provided ID
                .Select(u => new
                {
                    UserId = u.UserId,
                    UserName = u.Name, // Fixed property name
                    Email = u.Email,
                    Roles = u.UserRoles.Select(ur => new
                    {
                        RoleName = ur.Role.RoleName
                    }).ToList()
                })
                .FirstOrDefaultAsync(); // Retrieve a single user or null

            if (user == null)
                return NotFound();

            return Ok(user); // Return the user as an anonymous object
        }


        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, User user)
        {
            if (id != user.UserId)
                return BadRequest();

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.UserId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _context.Users
                .Include(u => u.Cart)
                    .ThenInclude(c => c.Items)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return NotFound(new { message = $"User with email {email} not found." });

            var result = new
            {
                userId = user.UserId,
                email = user.Email,
                name = user.Name,
                password = user.Password,
                roles = user.UserRoles.Select(r => new
                {
                    roleId = r.Role.RoleId,
                    roleName = r.Role.RoleName
                }),
                cart = user.Cart != null ? new
                {
                    cartId = user.Cart.Id,
                    totalPrice = user.Cart.TotalPrice,
                    products = user.Cart.Items.Select(i => new
                    {
                        productId = i.ProductId,
                        productName = i.Product?.Name ?? "Unknown",
                        image = i.Product?.Image ?? "No Image",
                        description = i.Product?.Description ?? "No Description",
                        quantity = i.Quantity,
                        price = i.Price,
                        discount = i.Discount ?? 0,
                        specialPrice = i.SpecialPrice ?? 0,
                        category = i.Product?.Category != null ? new
                        {
                            categoryId = i.Product.Category.Id,
                            categoryName = i.Product.Category.Name
                        } : null
                    })
                } : null
            };

            return Ok(result);
        }


    }

}
