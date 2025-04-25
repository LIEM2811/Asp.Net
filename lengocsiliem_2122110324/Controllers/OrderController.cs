using lengocsiliem_2122110324.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lengocsiliem_2122110324.Model;

namespace lengocsiliem_2122110324.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Select(o => new
                {
                    orderId = o.OrderId,
                    email = o.Email,
                    orderDate = o.OrderDate,
                    payment = o.Payment,
                    note = o.Note,
                    totalAmount = o.TotalAmount,
                    orderStatus = o.OrderStatus,
                    orderItems = o.OrderItems.Select(oi => new
                    {
                        orderItemId = oi.OrderDetailId,
                        product = oi.Product != null ? new
                        {
                            productId = oi.Product.Id,
                            productName = oi.Product.Name ?? "Unknown",
                            image = oi.Product.Image ?? "No Image",
                            description = oi.Product.Description ?? "No Description",
                            quantity = oi.Quantity,
                            price = oi.Product.Price,
                            discount = oi.Product.Discount,
                            specialPrice = oi.Product.SpecialPrice,
                            category = oi.Product.Category != null ? new
                            {
                                categoryId = oi.Product.Category.Id,
                                categoryName = oi.Product.Category.Name
                            } : null
                        } : null
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }


        // GET: api/Order/user/john@example.com/5
        [HttpGet("user/{email}/{orderId}")]
        public async Task<IActionResult> GetUserOrder(string email, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Where(o => o.Email == email && o.OrderId == orderId)
                .Select(o => new
                {
                    orderId = o.OrderId,
                    email = o.Email,
                    orderDate = o.OrderDate,
                    payment = o.Payment,
                    note = o.Note,
                    totalAmount = o.TotalAmount,
                    orderStatus = o.OrderStatus,
                    orderItems = o.OrderItems.Select(oi => new
                    {
                        orderItemId = oi.OrderDetailId,
                        product = oi.Product != null ? new
                        {
                            productId = oi.Product.Id,
                            productName = oi.Product.Name ?? "Unknown",
                            image = oi.Product.Image ?? "No Image",
                            description = oi.Product.Description ?? "No Description",
                            quantity = oi.Quantity,
                            price = oi.Product.Price,
                            discount = oi.Product.Discount,
                            specialPrice = oi.Product.SpecialPrice,
                            category = oi.Product.Category != null ? new
                            {
                                categoryId = oi.Product.Category.Id,
                                categoryName = oi.Product.Category.Name
                            } : null
                        } : null
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(order);
        }


        [HttpPost("user/{email}/carts/{cartId}/payment/{payment}/note/{note}")]
        public async Task<IActionResult> CreateOrderFromCart(string email, int cartId, string payment, string note)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId && c.User != null && c.User.Email == email);

            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                return BadRequest(new { message = "Cart is empty or not found." });
            }

            if (string.IsNullOrWhiteSpace(payment))
            {
                return BadRequest(new { message = "Payment method is required." });
            }

            if (note.Length > 500)
            {
                return BadRequest(new { message = "Note cannot exceed 500 characters." });
            }

            var order = new Order
            {
                Email = email,
                OrderDate = DateTime.UtcNow,
                Payment = payment,
                OrderStatus = "Pending",
                Note = note,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderDetail>()
            };

            decimal totalAmount = 0;

            foreach (var item in cart.Items)
            {
                var product = item.Product;
                if (product == null) continue;

                decimal price = product.SpecialPrice > 0 ? product.SpecialPrice : (decimal)product.Price;
                decimal discount = item.Discount ?? 0m;

                if (discount < 0 || discount > 100)
                {
                    return BadRequest(new { message = $"Invalid discount value for product {product.Name}." });
                }

                decimal orderedPrice = price * item.Quantity * (1 - discount / 100m);
                totalAmount += orderedPrice;

                order.OrderItems.Add(new OrderDetail
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    OrderedProductPrice = price,
                    Discount = discount
                });
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);

            // ❌ Xóa cart items
            _context.CartItems.RemoveRange(cart.Items);

            // ✅ Reset total price của cart
            cart.TotalPrice = 0;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Order created successfully",
                orderId = order.OrderId,
                total = totalAmount,
                payment = order.Payment,
                note = order.Note
            });
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order updatedOrder)
        {
            if (id != updatedOrder.OrderId)
            {
                return BadRequest(new { message = "Order ID mismatch." });
            }

            var existingOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (existingOrder == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            // Cập nhật thông tin cơ bản
            existingOrder.Payment = updatedOrder.Payment;
            existingOrder.Note = updatedOrder.Note;
            existingOrder.OrderStatus = updatedOrder.OrderStatus;

            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order updated successfully.", order = existingOrder });
        }
        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            // Xóa các OrderItems liên quan trước
            if (order.OrderItems != null && order.OrderItems.Any())
            {
                _context.OrderDetails.RemoveRange(order.OrderItems);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order and its items deleted successfully", orderId = id });
        }

    }
}
