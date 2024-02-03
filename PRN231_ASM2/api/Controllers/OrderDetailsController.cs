using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class OrderDetailsController : ODataController
    {
        private readonly eStoreContext _context;

        public OrderDetailsController(eStoreContext context)
        {
            _context = context;
        }

        // GET: api/Members
        [EnableQuery]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }
            return await _context.OrderDetails.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("odata/OrderDetails/{id}")]
        public async Task<ActionResult<OrderDetail>> GetProduct(int id)
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }
            var product = await _context.OrderDetails.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("odata/OrderDetails/{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] OrderDetail product)
        {
            if (id != product.OrderDetailId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("odata/OrderDetails")]
        public async Task<ActionResult<OrderDetail>> PostProduct([FromBody] OrderDetail orderDetail)
        {
            if (_context.OrderDetails == null)
            {
                return Problem("Entity set 'EStoreContext.Products'  is null.");
            }
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = orderDetail.ProductId }, orderDetail);
        }

        // DELETE: api/Products/5
        [HttpDelete("odata/OrderDetails/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }
            var product = await _context.OrderDetails.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.OrderDetails?.Any(e => e.OrderDetailId == id)).GetValueOrDefault();
        }
    }
}
