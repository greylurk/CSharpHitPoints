using Microsoft.EntityFrameworkCore;

namespace HitPoints.Models
{

    public enum DamageType
    {
        Slashing,
        Piercing,
        Bludgeoning,
        Cold,
        Poison,
        Acid,
        Psychic,
        Fire,
        Necrotic,
        Radiant,
        Force,
        Thunder,
        Lighting,
    }

    public class Damage
    {
        public int Amount { get; set; }
        public DamageType Type { get; set; }
    }
}
