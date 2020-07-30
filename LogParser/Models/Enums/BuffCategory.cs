using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models.Enums
{
	public enum BuffCategory : byte
	{
		Boon = 0,
		Any = 1,
		Condition = 2,
		Food = 4,
		Upgrade = 6,
		Boost = 8,
		Trait = 11,
		Enhancement = 13,
		Stance = 16
	}
}
