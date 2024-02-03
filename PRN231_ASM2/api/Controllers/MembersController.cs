using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    public class MembersController : ODataController
    {
        private readonly eStoreContext _context;
        public MembersController(eStoreContext context)
        {
            _context = context;
        }
        // GET: api/<MembersController>
        [EnableQuery]
        [HttpGet]
        public IQueryable<Member> GetMembers()
        {
            return _context.Members.AsQueryable();
        }

        // GET api/<MembersController>/5
        [HttpGet("odata/Members/{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            if(_context.Members == null)
            {
                return NotFound(id);
            }
            var member = await _context.Members.FindAsync(id);
            if(member == null)
            {
                return NotFound();
            }
            return member;
        }

        // POST api/<MembersController>
        [HttpPost("odata/Members")]
        public async Task<ActionResult<Member>> PostMember([FromBody] Member member)
        {
            if(_context.Members == null)
            {
                return Problem("Entity set 'EStoreContext.Members'  is null.");
            }
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetMember", new { id = member.MemberId }, member);
        }

        // PUT api/<MembersController>/5
        [HttpPut("odata/Members/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Member member)
        {
            if (id != member.MemberId)
            {
                return BadRequest();
            }
            _context.Entry(member).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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
        [HttpDelete("odata/Members/{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            if (_context.Members == null)
            {
                return NotFound();
            }
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool MemberExists(int id)
        {
            return (_context.Members?.Any(e => e.MemberId == id)).GetValueOrDefault();
        }
        [HttpPost("odata/Login")]
        public async Task<ActionResult<Member>> Login([FromBody] RequestMember member)
        {
            var email = member.Email;
            var password = member.Password;
            Member member1 = new Member();
            if (email != null&&password !=null) { 
                member1 = await _context.Members.Where(e => e.Email == email && e.Password == password).FirstOrDefaultAsync();
                if(member1 != null)
                {
                    return(Ok(member1));
                }
            }
            return BadRequest();
        }
    }
}
