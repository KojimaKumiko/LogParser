using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models.Enums
{
	public enum Activation
	{
		None = 0,			// not used - not this kind of event
		Start = 1,			// started skill/animation activation
		Quickness = 2,		// unused as of nov 5 2019
		CancelFire = 3,		// stopped skill activation with reaching tooltip time
		CancelCancel = 4,	// stopped skill activation without reaching tooltip time
		Reset = 5			// animation completed fully
	}
}
