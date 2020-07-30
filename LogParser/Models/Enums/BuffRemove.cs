using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models.Enums
{
    public enum BuffRemove
    {
        None = 0,   // not used - not this kind of event
        All = 1,    // last/all stacks removed (sent by server)
        Single = 2, // single stack removed (sent by server). will happen for each stack on cleanse
        Manual = 3, // single stack removed (auto by arc on ooc or all stack, ignore for strip/cleanse calc, use for in/out volume)
    }
}
