using lengocsiliem_2122110324.Data;
using lengocsiliem_2122110324.Dto;
using lengocsiliem_2122110324.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace lengocsiliem_2122110324.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllCarts()
        {
            var carts = await _context.Carts
                .Include(c => c.User)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .Select(c => new
                {
                    cartId = c.Id,
                    totalPrice = c.TotalPrice,
                    userName = c.User != null ? c.User.Name : "Unknown",
                    userId=c.UserId,
                    products = c.Items.Select(i => new
                    {
                        productId = i.Product != null ? i.Product.Id : 0,
                        productName = i.Product != null ? i.Product.Name : "Unknown",
                        image = i.Product != null ? i.Product.Image : "No Image",
                        description = i.Product != null ? i.Product.Description : "No Description",
                        quantity = i.Quantity,
                        price = i.Price,
                        discount = i.Discount ?? 0,
                        specialPrice = i.Product != null ? i.Product.SpecialPrice : 0,
                        category = i.Product != null && i.Product.Category != null
                            ? new
                            {
                                categoryId = i.Product.Category.Id,
                                categoryName = i.Product.Category.Name
                            }
                            : null
                    }).ToList()
                })
                .ToListAsync();

            return Ok(carts);
        }


        // GET api/<CartController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartDTO>> GetCartById(long cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
                return NotFound(new { message = $"Cart with ID {cartId} not found." });

            var cartDto = new CartDTO
            {
                CartId = cart.Id,
                TotalPrice = cart.TotalPrice,
                Products = cart.Items.Select(i => new ProductDTO
                {
                    ProductId = i.Product.Id,
                    ProductName = i.Product.Name,
                    Image = i.Product.Image,
                    Description = i.Product.Description,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Discount = i.Discount ?? 0,
                    SpecialPrice = i.Product.SpecialPrice,
                    Category = i.Product.Category != null ? new CategoryDTO
                    {
                        CategoryId = i.Product.Category.Id,
                        CategoryName = i.Product.Category.Name
                    } : null
                }).ToList()
            };

            return Ok(cartDto);
        }


        [HttpPost("{cartId}/product/{productId}/quantity/{quantity}")]
        public async Task<ActionResult<CartDTO>> AddProductToCartAsync(long cartId, long productId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
                return NotFound(new { message = $"Cart with ID {cartId} not found." });

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            //if (product == null)
            //    return NotFound(new { message = $"Product with ID {productId} not found." });

            //var existingCartItem = await _context.CartItems
            //    .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            //if (existingCartItem != null)
            //    return BadRequest(new { message = $"Product {product.Name} already exists in the cart." });

            //if (product.Quantity == 0)
            //    return BadRequest(new { message = $"{product.Name} is not available." });

            //if (product.Quantity < quantity)
            //    return BadRequest(new { message = $"Please, order {product.Name} in quantity less than or equal to {product.Quantity}." });

            var newCartItem = new CartItem
            {
                ProductId = product.Id,
                CartId = cart.Id,
                Quantity = quantity,
                Discount = product.Discount,
                Price = product.SpecialPrice
            };

            await _context.CartItems.AddAsync(newCartItem);

            // Update stock and cart price
            product.Quantity -= quantity;
            cart.TotalPrice += product.SpecialPrice * quantity;
            cart.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();

            // Return mapped DTO
            var cartDto = new CartDTO
            {
                CartId = cart.Id,
                TotalPrice = cart.TotalPrice,
                Products = cart.Items.Select(i => new ProductDTO
                {
                    ProductId = i.Product.Id,
                    ProductName = i.Product.Name,
                    Image = i.Product.Image,
                    Description = i.Product.Description,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Discount = i.Discount ?? 0,
                    SpecialPrice = i.Product.SpecialPrice,
                    Category = i.Product.Category != null ? new CategoryDTO
                    {
                        CategoryId = i.Product.Category.Id,
                        CategoryName = i.Product.Category.Name
                    } : null
                }).ToList()
            };

            return Ok(cartDto);
        }

        // PUT api/<CartController>/5
        [HttpPut("{cartId}/product/{productId}/quantity/{quantity}")]
        public async Task<IActionResult> UpdateProductQuantityInCart(long cartId, long productId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (cartItem == null)
                return NotFound(new { message = $"Product {productId} not found in cart {cartId}." });

            var product = cartItem.Product;

            int currentQuantity = cartItem.Quantity;

            if (product.Quantity + currentQuantity < quantity)
                return BadRequest(new { message = $"Only {product.Quantity + currentQuantity} of {product.Name} available in stock." });

            // Tính lại chênh lệch và cập nhật
            int difference = quantity - currentQuantity;

            product.Quantity -= difference;
            cartItem.Quantity = quantity;

            // Cập nhật lại giá trị totalPrice trong giỏ hàng
            var cart = cartItem.Cart;
            cart.TotalPrice += cartItem.Price * difference;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Updated quantity of {product.Name} to {quantity} in cart {cartId}." });
        }


        // DELETE api/<CartController>/5
        [HttpDelete("{cartId}/product/{productId}")]
        public async Task<IActionResult> RemoveProductFromCart(long cartId, long productId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (cartItem == null)
                return NotFound(new { message = $"Product {productId} not found in Cart {cartId}." });

            // Cập nhật lại tồn kho sản phẩm
            var product = cartItem.Product;
            product.Quantity += cartItem.Quantity;

            // Cập nhật lại tổng tiền giỏ hàng
            var cart = cartItem.Cart;
            cart.TotalPrice -= cartItem.Price * cartItem.Quantity;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Product {product.Name} removed from Cart {cartId}." });
        }
        [HttpPost]
        public IActionResult Post([FromBody] Cart C)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == C.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }
            var cart = new Cart
            {
                TotalPrice = C.TotalPrice,
                UserId = C.UserId
            };

            _context.Carts.Add(cart);
            _context.SaveChanges();

            return Ok(new
            {
                totalPrice = cart.TotalPrice,
                userId = cart.UserId
            });
        }
        [HttpGet("{id}/{email}")]
        public async Task<ActionResult<object>> GetCartByEmail(int id, string email)
        {
            var cart = await _context.Carts
                .Include(c => c.User)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .Where(c => c.Id == id && c.User != null && c.User.Email == email)
                .Select(c => new
                {
                    cartId = c.Id,
                    totalPrice = c.TotalPrice,
                    userName = c.User != null ? c.User.Name : "Unknown",
                    products = c.Items.Select(i => new
                    {
                        productId = i.Product != null ? i.Product.Id : 0,
                        productName = i.Product != null ? i.Product.Name : "Unknown",
                        image = i.Product != null ? i.Product.Image : "No Image",
                        description = i.Product != null ? i.Product.Description : "No Description",
                        quantity = i.Quantity,
                        price = i.Price,
                        discount = i.Discount ?? 0,
                        specialPrice = i.Product != null ? i.Product.SpecialPrice : 0,
                        category = i.Product != null && i.Product.Category != null
                            ? new
                            {
                                categoryId = i.Product.Category.Id,
                                categoryName = i.Product.Category.Name
                            }
                            : null
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (cart == null)
                return NotFound("Cart not found for the given id and email.");

            return Ok(cart);
        }

    }
}
