using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace canjewelry.src
{
    public class Config
    {
        public static Config Current { get; set; } = new Config();
        public class Part<Config>
        {
            public readonly string Comment;
            public readonly Config Default;
            private Config val;
            public Config Val
            {
                get => (val != null ? val : val = Default);
                set => val = (value != null ? value : Default);
            }
            public Part(Config Default, string Comment = null)
            {
                this.Default = Default;
                this.Val = Default;
                this.Comment = Comment;
            }
            public Part(Config Default, string prefix, string[] allowed, string postfix = null)
            {
                this.Default = Default;
                this.Val = Default;
                this.Comment = prefix;

                this.Comment += "[" + allowed[0];
                for (int i = 1; i < allowed.Length; i++)
                {
                    this.Comment += ", " + allowed[i];
                }
                this.Comment += "]" + postfix;
            }
        }
        public Part<float> grindTimeOneTick = new Part<float>(3);

        public Part<Dictionary<string, HashSet<string>>> buffNameToPossibleItem = new Part<Dictionary<string, HashSet<string>>>
        (new Dictionary<string, HashSet<string>> {
            {"diamond", new HashSet<string>{ "brigandine", "plate", "chain", "scale" } },
            {"corundum", new HashSet<string>{ "pickaxe" } },
            {"emerald", new HashSet<string>{ "brigandine", "plate", "chain", "scale" } },
            {"fluorite", new HashSet<string>{ "halberd", "mace", "spear", "rapier", "longsword", "zweihander", "messer", "falx" } },
            {"lapislazuli", new HashSet<string>{ "brigandine", "plate", "chain", "scale" } },
            {"malachite", new HashSet<string>{ "brigandine", "plate", "chain", "scale", "knife" } },
            {"olivine", new HashSet<string>{ "brigandine", "plate", "chain", "scale" } },
            {"uranium", new HashSet<string>{ "brigandine", "plate", "chain", "scale" } },
            {"quartz", new HashSet<string>{ "pickaxe"} }
        });
        public Part<Dictionary<string, Dictionary<string, float>>> gems_buffs = new Part<Dictionary<string, Dictionary<string, float>>>
            (new Dictionary<string, Dictionary<string, float>>  {
                { "walkspeed", new Dictionary<string, float>{
                    { "1", 0.02f },
                    { "2", 0.04f },
                    { "3", 0.08f }
                    }              
                },
                { "miningSpeedMul", new Dictionary<string, float>{
                    { "1", 0.03f },
                    { "2", 0.06f },
                    { "3", 0.09f }
                    }
                },
                { "maxhealthExtraPoints", new Dictionary<string, float>{
                    { "1", 1 },
                    { "2", 2 },
                    { "3", 4 }
                    }
                },
                { "meleeWeaponsDamage", new Dictionary<string, float>{
                    { "1", 0.03f },
                    { "2", 0.05f },
                    { "3", 0.08f }
                    }
                },
                { "hungerrate", new Dictionary<string, float>{
                    { "1", -0.03f },
                    { "2", -0.06f },
                    { "3", -0.1f }
                    }
                },
                { "wildCropDropRate", new Dictionary<string, float>{
                    { "1", 0.02f },
                    { "2", 0.05f },
                    { "3", 0.09f }
                    }
                },
                { "armorDurabilityLoss", new Dictionary<string, float>{
                    { "1", 0.05f },
                    { "2", 0.1f },
                    { "3", 0.15f }
                    }
                },
                { "oreDropRate", new Dictionary<string, float>{
                    { "1", 0.05f },
                    { "2", 0.08f },
                    { "3", 0.11f }
                    }
                },
                { "healingeffectivness", new Dictionary<string, float>{
                    { "1", 0.05f },
                    { "2", 0.1f },
                    { "3", 0.12f }
                    }
                }
            });



    }
}
