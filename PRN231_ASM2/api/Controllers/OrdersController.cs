using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
namespace api.Controllers
{
    public class OrdersController : ODataController
    {
        private readonly eStoreContext _context;
        public OrdersController(eStoreContext context)
        {
            _context = context;
        }
        // GET: api/<MembersController>
        [EnableQuery]
        [HttpGet]
        public IQueryable<Order> GetOrders()
        {
            return _context.Orders.AsQueryable();
        }

        // GET api/<MembersController>/5
        [HttpGet("odata/Orders/{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound(id);
            }
            var member = await _context.Orders.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return member;
        }

        // POST api/<MembersController>
        [HttpPost("odata/Orders")]
        public async Task<ActionResult<Order>> PostOrder([FromBody] RequestOrder order)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'EStoreContext.Members'  is null.");
            }
            var newOrder = new Order
            {
                MemberId = order.MemberId,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                Freight = order.Freight,
            };
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // PUT api/<MembersController>/5
        [HttpPut("odata/Orders/{id}")]
        public async Task<IActionResult> PutOrder(int id,[FromBody] RequestOrder order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }
            //nice
            Order newOrder = new Order
            {
                Freight = order.Freight,
                OrderId = order.OrderId,
                MemberId = order.MemberId,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
            };
            _context.Entry(newOrder).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // DELETE api/<MembersController>/5
        [HttpDelete("odata/Orders/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.MemberId == id)).GetValueOrDefault();
        }
    }
}
