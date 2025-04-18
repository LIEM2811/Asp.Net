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
    public class OrderDetailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetail
        [HttpGet]
        public async Task<IEnumerable<OrderDetail>> Get()
        {
            return await _context.OrderDetails
                .ToListAsync();
        }

        // GET: api/OrderDetail/Order/5
        [HttpGet("Order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            if (!orderDetails.Any())
            {
                return NotFound(new { message = "No order details found for this order." });
            }

            return Ok(orderDetails);
        }

        // POST: api/OrderDetail
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Post([FromBody] OrderDetail detail)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == detail.OrderId);
            if (order == null)
            {
                return BadRequest(new { message = "Order not found." });
            }

            _context.OrderDetails.Add(detail);
            _context.SaveChanges();

            return Ok(new { message = "Order detail added", detail });
        }

        // PUT: api/OrderDetail/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Put(int id, [FromBody] OrderDetail updatedDetail)
        {
            var detail = _context.OrderDetails.FirstOrDefault(d => d.OrderDetailId == id);

            if (detail == null)
            {
                return NotFound(new { message = "Order detail not found" });
            }

            detail.ProductId = updatedDetail.ProductId;
            detail.Quantity = updatedDetail.Quantity;
            detail.UnitPrice = updatedDetail.UnitPrice;
            detail.Discount = updatedDetail.Discount;

            // Recalculate the Total property based on other fields
            _context.Entry(detail).Property(d => d.Total).IsModified = false;

            _context.OrderDetails.Update(detail);
            _context.SaveChanges();

            return Ok(new { message = "Order detail updated", detail });
        }


        // DELETE: api/OrderDetail/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Delete(int id)
        {
            var detail = _context.OrderDetails.FirstOrDefault(d => d.OrderDetailId == id);

            if (detail == null)
            {
                return NotFound(new { message = "Order detail not found" });
            }

            _context.OrderDetails.Remove(detail);
            _context.SaveChanges();

            return Ok(new { message = "Order detail deleted", detail });
        }
    }
}
