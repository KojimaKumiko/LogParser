using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models.Enums
{
	public enum PhysicalResult
	{
		Normal = 0,			// good physical hit
		Crit = 1,			// physical hit was crit
		Glance = 2,			// physical hit was glance
		Block = 3,			// physical hit was blocked eg. mesmer shield 4
		Evade = 4,			// physical hit was evaded, eg. dodge or mesmer sword 2
		Interrupt = 5,		// physical hit interrupted something
		Absorb = 6,			// physical hit was "invlun" or absorbed eg. guardian elite
		Blind = 7,			// physical hit missed
		KillingBlow = 8,	// hit was killing hit
		Downed = 9,			// hit was downing hit
	}
}
