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
    [Route("[controller]")]
    [ApiController]
    public class PlayerCharacterController : ControllerBase
    {
        private readonly Context _context;

        public PlayerCharacterController(Context context)
        {
            _context = context;
        }

        // GET: a list of player characters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerCharacterSummary>>> GetPlayerCharacter()
        {
            return await _context.PlayerCharacter.Select(p => new PlayerCharacterSummary{
                Id = p.Id,
                Name = p.Name
            }).ToListAsync();
        }

        // GET: api/PlayerCharacter/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerCharacter>> GetPlayerCharacter(long id)
        {
            var playerCharacter = await _context.PlayerCharacter
                .Include(i => i.Classes)
                .Include(i => i.Stats)
                .Include(i => i.Items)
                .ThenInclude(it => it.Modifier)
                .Include(i => i.Defenses)
                .Include(pc => pc.HPEvents)
                .Where(character=>character.Id == id)
                .FirstAsync();

            if (playerCharacter == null)
            {
                return NotFound();
            }

            return playerCharacter;
        }


        [HttpPost("{id}/damage")]
        public async Task<ActionResult> DamagePlayerCharacter(long id, Damage damage) {
            var playerCharacter = await _context.PlayerCharacter.FindAsync(id);
            playerCharacter.HPEvents.Add(new HPEvent{
                DamageType = damage.Type,
                HPEventType = HPEventType.Damage,
                Amount = damage.Amount,
            });
            
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

        [HttpPost("{id}/tempHitPoint")]
        public async Task<ActionResult> TempHitPointPlayerCharacter(long id, TempHitpoint tempHitPoints) {
            var playerCharacter = await _context.PlayerCharacter.FindAsync(id);
            playerCharacter.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.TempHitPoints,
                Amount = tempHitPoints.Amount,
            });
            
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
        
        [HttpPost("{id}/heal")]
        public async Task<ActionResult> HealPlayerCharacter(long id, Heal heal) {
            var playerCharacter = await _context.PlayerCharacter.FindAsync(id);
            playerCharacter.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.Heal,
                Amount = heal.Amount,
            });
            
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
        private bool PlayerCharacterExists(long id)
        {
            return _context.PlayerCharacter.Any(e => e.Id == id);
        }
    }

    public class PlayerCharacterSummary
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
