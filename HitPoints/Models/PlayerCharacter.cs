using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace HitPoints.Models
{
    public enum DefenseType
    {
        Resistance,
        Immunity,
    }

    /** 
     * In Java, I'd put this in a different file. In C# this feels like 
     * an ok place to put it since it's all modelling the JSON data in 
     * one character file. I've never worked on a big enough C# project to 
     * learn the appropriate conventions for where to put related classes.
     **/
    public class Level
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Name { get; set; }
        public int HitDiceValue { get; set; }
        public int ClassLevel { get; set; }
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

    public class PlayerCharacter
    {
        public PlayerCharacter()
        {
            HPEvents = new List<HPEvent>();
            Defenses = new List<Defense>();
            Items = new List<Item>();
        }

        [JsonIgnore]
        public long Id { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("classes")]
        public List<Level> Levels { get; set; } = new List<Level>();
        public Stats Stats { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
        public List<Defense> Defenses { get; set; } = new List<Defense>();

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
                    .Aggregate(0, (x, y) => x + y);
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
                var hp = Levels
                    .Select(level => level.ClassLevel * (level.HitDiceValue / 2 + 1 + conModifier))
                    .Aggregate(0, (x, y) => x + y);
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
                    .ForEach(hpEvent => currentHitPoints = applyHPEvent(currentHitPoints, hpEvent));

                return currentHitPoints;
            }
        }

        private static bool IsImmune(Defense defense, DamageType? damageType) {
            return defense.DamageType == damageType && defense.DefenseType == DefenseType.Immunity;
        }

        private static bool IsResistant(Defense defense, DamageType? damageType) {
            return defense.DamageType == damageType && defense.DefenseType == DefenseType.Resistance;
        }

        private int applyHPEvent(int currentHitPoints, HPEvent hpEvent)
        {
            int newHitPoints = currentHitPoints;
            // Might switch this over to a Command Pattern? C in CQRS
            switch (hpEvent.HPEventType)
            {
                case HPEventType.Damage:
                    if (Defenses.Any(def => IsImmune(def, hpEvent.DamageType)))
                    {
                        break; // they're immune, don't change HP total
                    }
                    else if (Defenses.Any(def => IsResistant(def, hpEvent.DamageType)))
                    {
                        newHitPoints -= hpEvent.Amount / 2;
                    }
                    else
                    {
                        newHitPoints -= hpEvent.Amount;
                    }
                    break;

                case HPEventType.TempHitPoints:
                    // Interesting I'd never thought of it this way, but effectively Temp hit points are just healing without
                    // a cap. Normal healing caps out at your base hp, but temp hp doesn't cap out there. 
                    newHitPoints += hpEvent.Amount;
                    break;

                case HPEventType.Heal:
                    if (currentHitPoints > BaseHitPoints)
                    {
                        // If we've still got temporary hit points, don't erase them
                        break;
                    }
                    newHitPoints = Math.Min(BaseHitPoints, currentHitPoints + hpEvent.Amount);
                    break;
                case HPEventType.ShortRest:
                    // Short Rests erase temporary hit points, and heal a number of hit dice (outside of scope, but a good skeleton)
                    newHitPoints = Math.Min(BaseHitPoints, currentHitPoints + hpEvent.Amount);
                    break;

                case HPEventType.LongRest:
                    // 
                    newHitPoints = BaseHitPoints;
                    break;
            }
            return newHitPoints;
        }

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