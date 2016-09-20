using System;
using System.Collections.Generic;
using System.Linq;

namespace DS1Stats.Models
{
    public class CharacterInfo
    {
        public string Name { get; set; }
        public uint Deaths { get; set; }
        public uint SessionStartDeaths { get; set; }
        public uint SessionDeaths { get; set; }
        public DateTime LastDeath { get; set; }
        public List<TimeSpan> LifeDurations { get; set; }

        //Stats
        public long Vitality { get; set; }
        public long Attunement { get; set; }
        public long Endurance { get; set; }
        public long Strength { get; set; }
        public long Dexterity { get; set; }
        public long Intelligence { get; set; }
        public long Faith { get; set; }
        public long Humanity { get; set; }
        public long Resistance { get; set; }
        public long Level { get; set; }
        public long Souls { get; set; }
        public long EarnedSouls { get; set; }

        //Items
        //public int LeftArrows { get; set; }
        //public int LeftBolts { get; set; }
        //public int RightArrows { get; set; }
        //public int RightBolts { get; set; }
        //public int LeftRing { get; set; }
        //public int RightRing { get; set; }
        //public int FirstQuick { get; set; }
        //public int SecondQuick { get; set; }
        //public int ThirdqQick { get; set; }
        //public int FourthQuick { get; set; }
        //public int FifthQuick { get; set; }
        //public int Headwearing { get; set; }
        //public int Bodywearing { get; set; }
        //public int Handswearing { get; set; }
        //public int Legswearing { get; set; }

        public CharacterInfo()
        {
            LastDeath = DateTime.UtcNow;
            LifeDurations = new List<TimeSpan>();
        }

        public TimeSpan CalculateAverageLifeLength()
        {
            if (SessionDeaths == 0) return TimeSpan.Zero;

            var totalSpan = new TimeSpan();
            totalSpan = LifeDurations.Aggregate(totalSpan, (current, duration) => current.Add(duration));

            return new TimeSpan(totalSpan.Ticks / SessionDeaths);
        }
    }
}