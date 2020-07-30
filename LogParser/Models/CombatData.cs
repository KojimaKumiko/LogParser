using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models
{
    public class CombatData
    {
        public CombatData()
        {
        }

        public ulong Time { get; set; }
        public ulong SrcAgent { get; set; }
        public ulong DstAgent { get; set; }
        public int Value { get; set; }
        public int BuffDmg { get; set; }
        public uint OverstackValue { get; set; }
        public uint SkillID { get; set; }
        public ushort SrcInstID { get; set; }
        public ushort DstInstID { get; set; }
        public ushort SrcMasterInstID { get; set; }
        public ushort DstMasterInstID { get; set; }
        public byte IFF { get; set; }
        public byte Buff { get; set; }
        public byte Result { get; set; }
        public byte IsActivation { get; set; }
        public byte IsBuffRemove { get; set; }
        public byte IsNinety { get; set; }
        public byte IsFifty { get; set; }
        public byte IsMoving { get; set; }
        public byte IsStateChange { get; set; }
        public byte IsFlanking { get; set; }
        public byte IsShields { get; set; }
        public byte IsOffcycle { get; set; }
        public uint Pad { get; set; }
    }
}
