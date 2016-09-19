using System;

namespace DS1Counter.Models
{
    public class CharacterInfo
    {
        public string Name { get; set; }
        public uint Deaths { get; set; }
        public uint SessionStartDeaths { get; set; }
        public uint SessionDeaths { get; set; }
        public DateTime LastDeath { get; set; }

        public CharacterInfo()
        {
            LastDeath = DateTime.UtcNow;
        }
    }
}