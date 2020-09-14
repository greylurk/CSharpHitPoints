using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace HitPoints.Models
{

    /** 
     * In Java, I'd put this in a different file. In C# this feels like 
     * an ok place to put it since it's all modelling the JSON data in 
     * one character file. I've never worked on a big enough C# project to 
     * learn the appropriate conventions for where to put related classes.
     **/
    public class CharacterClass
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Name { get; set; }
        public int HitDiceValue { get; set; }
        public int ClassLevel { get; set; }
    }


    public enum DefenseType
    {
        Resistance,
        Immunity,
    }

    public class Defense
    {
        [JsonIgnore]
        public long Id { get; set; }
        [JsonPropertyName("defense")]
        public DefenseType DefenseType { get; set; }
        [JsonPropertyName("type")]
        public DamageType DamageType { get; set; }
    }

    public class PlayerCharacter
    {
        public PlayerCharacter()
        {
            Classes = new List<CharacterClass>();
            HPEvents = new List<HPEvent>();
            Defenses = new List<Defense>();
            Items = new List<Item>();
        }

        [JsonIgnore]
        public long Id { get; set; }
        public string Name { get; set; }
        public List<CharacterClass> Classes { get; set; }
        public Stats Stats { get; set; }
        public List<Item> Items { get; set; }
        public List<Defense> Defenses { get; set; }

        // This is effectively a chronological event log. 
        public List<HPEvent> HPEvents { get; set; }

        // Maybe memoize? I trust Linq performance (for now)
        [NotMapped]
        public int EffectiveConstitution
        {
            get
            {
                var con = Stats != null ? Stats.Constitution : 10; // Maybe this isn't a valid assumption?
                con += Items.Select(item => item.Modifier)
                    .Where(mod => mod != null && mod.AffectedObject.Equals("stats") && mod.AffectedValue == "constitution")
                    .Select(mod => mod.Value)
                    .Aggregate(0,(x, y) => x + y) ;
                return con;
            }
        }

        // Memoize? 
        [NotMapped]
        public int BaseHitPoints
        {
            get
            {
                var conModifier = (EffectiveConstitution - 10) / 2;
                var hp = Classes
                    .Select(level => level.ClassLevel * (level.HitDiceValue / 2 + 1 + conModifier))
                    .Aggregate(0,(x, y) => x + y);
                return hp;
            }
        }


        [NotMapped]
        // Memoize? In a large scale prod app, I'd probably CQRS this, but for now this should be performant enough.
        public int CurrentHitPoints
        {
            get
            {
                var currentHitPoints = BaseHitPoints;
                if (HPEvents == null || HPEvents.Count == 0)
                {
                    return currentHitPoints;
                }
                var lastLongRest = HPEvents.FindLastIndex(evt => evt.HPEventType == HPEventType.LongRest);
                var interestingEvents = HPEvents.Count - 1 - lastLongRest;
                HPEvents.TakeLast(interestingEvents)
                    .ToList() // This can allocate a lot of memory, but should be a pretty small list most of the time.
                    .ForEach(hpEvent =>
                    {
                        // Might switch this over to a Command Pattern? C in CQRS
                        switch (hpEvent.HPEventType) 
                        {
                            case HPEventType.Damage:
                                if (Defenses.Any(def => def.DamageType == hpEvent.DamageType && def.DefenseType == DefenseType.Immunity))
                                {
                                    break; // they're immune, don't change HP total
                                }
                                else if (Defenses.Any(def => def.DamageType == hpEvent.DamageType && def.DefenseType == DefenseType.Resistance))
                                {
                                    currentHitPoints -= hpEvent.Amount / 2;
                                }
                                else
                                {
                                    currentHitPoints -= hpEvent.Amount;
                                }
                                break;
                            case HPEventType.Heal:
                                if( currentHitPoints > BaseHitPoints ) {
                                    // If we've still got temporary hit points, don't erase them
                                    break;
                                }
                                currentHitPoints = Math.Min(BaseHitPoints, currentHitPoints + hpEvent.Amount);
                                break;
                            case HPEventType.ShortRest:
                                // Short Rests erase temporary hit points, and heal a number of hit dice (outside of scope, but a good skeleton)
                                currentHitPoints = Math.Min(BaseHitPoints, currentHitPoints + hpEvent.Amount);
                                break;
                            case HPEventType.TempHitPoints:
                                // Interesting I'd never thought of it this way, but effectively Temp hit points are just healing without
                                // a cap. Normal healing caps out at your base hp, but temp hp doesn't cap out there. 
                                currentHitPoints = currentHitPoints + hpEvent.Amount;
                                break;

                            case HPEventType.LongRest:
                                // This should never happen, if it did, I got something wrong above.
                                break;
                        }
                    });

                return currentHitPoints;
            }
        }

    }

    public enum HPEventType
    {
        Damage,
        Heal,
        TempHitPoints,
        ShortRest,
        LongRest,
    }
    public class HPEvent
    {
        public int Id { get; set; }
        public HPEventType HPEventType { get; set; }
        public int Amount { get; set; }
        public DamageType? DamageType { get; set; }
    }

    public class Stats
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
    }

    public class Item
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Name { get; set; }
        public Modifier Modifier { get; set; }
    }

    // this is an interesting way of modelling D&D modifiers. I think it ends up needing reflection 
    // in the general case, which is probably not great? Hard to say.
    public class Modifier
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string AffectedObject { get; set; }
        public string AffectedValue { get; set; }
        public int Value { get; set; }
    }

    public class TempHitpoint
    {
        public int Amount { get; set; }
    }
    public class Heal
    {
        public int Amount { get; set; }
    }
}