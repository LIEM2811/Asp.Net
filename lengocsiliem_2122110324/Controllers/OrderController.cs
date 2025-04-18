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
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            return await _context.Orders
                .ToListAsync();
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(order);
        }

        // POST: api/Order
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Post([FromBody] Order order)
        {
            order.OrderDate = DateTime.Now;
            order.CreatedAt = DateTime.Now;

            _context.Orders.Add(order);
            _context.SaveChanges();

            return Ok(new { message = "Order created", order });
        }


        // PUT: api/Order/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Put(int id, [FromBody] Order updatedOrder)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            // Cập nhật thông tin
            order.UserId = updatedOrder.UserId;
            order.TotalAmount = updatedOrder.TotalAmount;
            order.Status = updatedOrder.Status;
            order.ShippingAddress = updatedOrder.ShippingAddress;
            order.Payment = updatedOrder.Payment;
            order.Note = updatedOrder.Note;
            order.UpdatedAt = DateTime.Now;

            _context.Orders.Update(order);
            _context.SaveChanges();

            return Ok(new { message = "Order updated", order });
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return Ok(new { message = "Order deleted", order });
        }
    }
}
