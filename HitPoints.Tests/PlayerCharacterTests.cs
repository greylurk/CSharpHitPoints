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
        public void TestEffectiveCon()
        {
            var pc = new PlayerCharacter{
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
            Assert.AreEqual(14, pc.EffectiveConstitution);
        }
    }
}
