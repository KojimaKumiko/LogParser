using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models
{
    public class AgentData
    {
        public AgentData()
        {
        }

        public ulong Agent { get; set; }
        public uint Prof { get; set; }
        public uint IsElite { get; set; }
        public ushort Toughness { get; set; }
        public ushort Concentration { get; set; }
        public ushort Healing { get; set; }
        public ushort HitboxWidth { get; set; }
        public ushort Condition { get; set; }
        public ushort HitboxHeight { get; set; }
        public string Name { get; set; }
    }
}
