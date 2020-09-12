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
    public class PlayerCharacterController : ControllerBase
    {
        private readonly Context _context;

        public PlayerCharacterController(Context context)
        {
            _context = context;
        }

        // GET: api/PlayerCharacter
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerCharacter>>> GetPlayerCharacter()
        {
            return await _context.PlayerCharacter.ToListAsync();
        }

        // GET: api/PlayerCharacter/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerCharacter>> GetPlayerCharacter(long id)
        {
            var playerCharacter = await _context.PlayerCharacter.FindAsync(id);

            if (playerCharacter == null)
            {
                return NotFound();
            }

            return playerCharacter;
        }

        // PUT: api/PlayerCharacter/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayerCharacter(long id, PlayerCharacter playerCharacter)
        {
            if (id != playerCharacter.Id)
            {
                return BadRequest();
            }

            _context.Entry(playerCharacter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerCharacterExists(id))
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

        // POST: api/PlayerCharacter
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PlayerCharacter>> PostPlayerCharacter(PlayerCharacter playerCharacter)
        {
            _context.PlayerCharacter.Add(playerCharacter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayerCharacter", new { id = playerCharacter.Id }, playerCharacter);
        }

        // DELETE: api/PlayerCharacter/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PlayerCharacter>> DeletePlayerCharacter(long id)
        {
            var playerCharacter = await _context.PlayerCharacter.FindAsync(id);
            if (playerCharacter == null)
            {
                return NotFound();
            }

            _context.PlayerCharacter.Remove(playerCharacter);
            await _context.SaveChangesAsync();

            return playerCharacter;
        }

        private bool PlayerCharacterExists(long id)
        {
            return _context.PlayerCharacter.Any(e => e.Id == id);
        }
    }
}
