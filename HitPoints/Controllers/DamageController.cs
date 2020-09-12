using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HitPoints.Models;

namespace HitPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DamageController : ControllerBase
    {
        private readonly Context _context;

        public DamageController(Context context)
        {
            _context = context;
        }

        // GET: api/Damage
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Damage>>> GetDamage()
        {
            return await _context.Damage.ToListAsync();
        }

        // GET: api/Damage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Damage>> GetDamage(long id)
        {
            var damage = await _context.Damage.FindAsync(id);

            if (damage == null)
            {
                return NotFound();
            }

            return damage;
        }

        // PUT: api/Damage/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDamage(long id, Damage damage)
        {
            if (id != damage.Id)
            {
                return BadRequest();
            }

            _context.Entry(damage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DamageExists(id))
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

        // POST: api/Damage
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Damage>> PostDamage(Damage damage)
        {
            _context.Damage.Add(damage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDamage", new { id = damage.Id }, damage);
        }

        // DELETE: api/Damage/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Damage>> DeleteDamage(long id)
        {
            var damage = await _context.Damage.FindAsync(id);
            if (damage == null)
            {
                return NotFound();
            }

            _context.Damage.Remove(damage);
            await _context.SaveChangesAsync();

            return damage;
        }

        private bool DamageExists(long id)
        {
            return _context.Damage.Any(e => e.Id == id);
        }
    }
}
