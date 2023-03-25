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

        //ECONOMY
        public Part<double> EXP_CHISEL_USE = new Part<double>(5);
        public Part<double> EXP_CHOPPED_LOG = new Part<double>(0.1);
        public Part<double> EXP_FORGED_TOOL = new Part<double>(18);
        public Part<double> EXP_FROM_KILLED_PLAYER_MIN_LAST_REVIVE_TOTAL_HOURS = new Part<double>(24);
        public Part<bool> DROP_PLAYER_ARMOR = new Part<bool>(true);
        public Part<bool> CHANCE_NOT_TO_DROP_ARMOR_PLAYER = new Part<bool>(false);
        public Part<int> CHANCE_NOT_TO_DROP = new Part<int>(35);


        public Part<float> grindTimeOneTick = new Part<float>(3);
    }
}
