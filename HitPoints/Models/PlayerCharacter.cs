using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HitPoints.Models {

    /** 
     * In Java, I'd put this in a different file. In C# this feels like 
     * an ok place to put it since it's all modelling the JSON data in 
     * one character file. I've never worked on a big enough C# project to 
     * learn the appropriate conventions for where to put related classes.
     **/
    public class CharacterClass {
        [JsonIgnore]
        public long Id {get; set;}
        public string Name {get; set;}
        public int HitDiceValue {get; set;}
        public int ClassLevel { get; set;}
    }
    

    public enum DefenseType {
        resistance,
        immunity,
    }

    public class Defense {
        [JsonIgnore]
        public long Id {get; set;}
        public DefenseType DefenseType {get; set;}
        public DamageType DamageType {get; set;}
    }

    public class PlayerCharacter {
        [JsonIgnore]
        public long Id {get; set;}
        public string Name {get; set;}
        public List<CharacterClass> Classes {get; set;}
        /**
        There have only been 6 D&D stats for 46 years. If this were my model,
        I don't think I'd leave this an open ended dictionary. Just spell out 
        strength, dexterity, constitution, wisdom, intelligence and charisma
        **/
        public Stats Stats {get; set;}
        public List<Item> Items{get; set;}
        public List<Defense> Defenses{get; set;}

        // Maybe memoize? C# getters are a bit heady. 
        public int EffectiveConstitution {
            get {
                var con = Stats.Constitution;
                con += Items.ConvertAll(item => item.Modifier)
                    .FindAll(mod => mod.AffectedObject.Equals("stats") && mod.AffectedValue == "constitution")
                    .ConvertAll(mod => mod.Value)
                    .Aggregate((x,y) => x + y);
                return con;
            }
        }

        public int BaseHitPoints{
            get {
                var conModifier = EffectiveConstitution - 10 / 2;
                var hp = Classes
                    .ConvertAll(level => level.HitDiceValue/2 + 1 + level.ClassLevel * conModifier)
                    .Aggregate((x,y) => x+y);
                return hp;
            }
        }
    }

    public class Stats
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int Strength {get; set;}
        public int Dexterity {get; set;}
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
    }

    public class Item
    {
        [JsonIgnore]
        public long Id {get; set;}
        public string Name {get; set;}
        public Modifier Modifier {get; set;}
    }

    // this is an interesting way of modelling D&D modifiers. I think it ends up needing reflection 
    // in the general case, which is probably not great? Hard to say.
    public class Modifier
    {
        [JsonIgnore]
        public long Id { get; set;}
        public string AffectedObject {get; set;}
        public string AffectedValue {get; set;}
        public int Value {get; set;}
    }
}