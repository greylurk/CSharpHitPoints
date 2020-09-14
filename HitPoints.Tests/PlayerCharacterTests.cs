using System.Collections.Generic;
using HitPoints.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HitPoints.Tests
{
    [TestClass]
    public class PlayerCharacterTests
    {
        [TestMethod]
        public void TestHitPoints() {
            var pc = new PlayerCharacter {
                Stats = new Stats {
                    Constitution = 16
                },
                Classes = new List<CharacterClass> {
                    new CharacterClass {
                        ClassLevel = 10,
                        HitDiceValue = 8,
                        Name = "Cleric"
                    }
                }
            };
            Assert.AreEqual(80, pc.BaseHitPoints);
        }

        [TestMethod]
        public void TestSimpleDamageEvent() {
            var pc = basicPlayerCharacter();
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.Damage,
                Amount = 10,
                DamageType=DamageType.Fire,
            });
            Assert.AreEqual(pc.CurrentHitPoints, pc.BaseHitPoints-10);
        }

        [TestMethod]
        public void TestTempHPAfterDamage() {
            var pc = basicPlayerCharacter();
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.Damage,
                Amount = 10,
                DamageType=DamageType.Fire,
            });
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.TempHitPoints,
                Amount=15,
            });
            Assert.AreEqual(pc.CurrentHitPoints, pc.BaseHitPoints+5);
        }

        [TestMethod]
        public void TestHealAfterTempHP() {
            var pc = basicPlayerCharacter();
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.TempHitPoints,
                Amount = 10,
            });
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.Heal,
                Amount=15,
            });
            Assert.AreEqual(pc.CurrentHitPoints, pc.BaseHitPoints+10);
        }

        [TestMethod]
        public void TestResistedDamageEvent() {
            var pc = basicPlayerCharacter();
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.Damage,
                Amount = 10,
                DamageType=DamageType.Fire,
            });
            pc.Defenses.Add(new Defense{
                DamageType=DamageType.Fire,
                DefenseType = DefenseType.Resistance,
            });
            Assert.AreEqual(pc.CurrentHitPoints, pc.BaseHitPoints-5);
        }

       
        [TestMethod]
        public void TestImmuneDamageEvent() {
            var pc = basicPlayerCharacter();
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.Damage,
                Amount = 10,
                DamageType=DamageType.Fire,
            });
            pc.Defenses.Add(new Defense{
                DamageType=DamageType.Fire,
                DefenseType = DefenseType.Immunity,
            });
            Assert.AreEqual(pc.CurrentHitPoints, pc.BaseHitPoints);
        }

        
        [TestMethod]
        public void TestHealingDamageEvent() {
            var pc = basicPlayerCharacter();
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.Damage,
                Amount = 10,
                DamageType=DamageType.Fire,
            });
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.Heal,
                Amount = 15,
            });
            Assert.AreEqual(pc.CurrentHitPoints, pc.BaseHitPoints);
        }

          
        [TestMethod]
        public void TestTempHPHealing() {
            var pc = basicPlayerCharacter();
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.TempHitPoints,
                Amount = 5,
            });
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.Damage,
                Amount = 10,
                DamageType=DamageType.Fire,
            });
            pc.HPEvents.Add(new HPEvent{
                HPEventType = HPEventType.Heal,
                Amount = 15,
            });
            Assert.AreEqual(pc.CurrentHitPoints, pc.BaseHitPoints);
        } 

        [TestMethod]
        public void TestEffectiveCon()
        {
            var pc = basicPlayerCharacter();
            Assert.AreEqual(14, pc.EffectiveConstitution);
        }

        private PlayerCharacter basicPlayerCharacter() {
            return new PlayerCharacter{
                Stats = new Stats {
                    Strength = 10,
                    Dexterity = 11,
                    Constitution = 12,
                    Intelligence = 13,
                    Wisdom = 14,
                    Charisma = 15
                },
                Items = new List<Item> {
                    new Item {
                        Modifier = new Modifier {
                            AffectedObject = "stats",
                            AffectedValue = "constitution",
                            Value = 2
                        },
                    },
                    new Item {
                        Modifier = new Modifier {
                            AffectedObject = "saves",
                            AffectedValue = "constitution",
                            Value = 4
                        }
                    },
                    new Item {
                        Modifier = new Modifier {
                            AffectedObject = "stats",
                            AffectedValue = "strength",
                            Value = 2
                        }
                    }
                }
            };
        }
    }
}
