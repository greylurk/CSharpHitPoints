using Microsoft.EntityFrameworkCore;

namespace HitPoints.Models {

public enum DamageType {
    slashing,
    piercing,
    bludgeoning,
    cold,
    poison,
    acid,
    psychic,
    fire,
    necrotic,
    radiant, 
    force,
    thunder,
    lighting,
}

public class Damage {
    public long Id { get; set;}
    public long Amount {get; set;}
    public DamageType Type {get; set;}
    public bool IsMagic {get; set;}
}

}
