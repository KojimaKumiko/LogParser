using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models.Enums
{
	public enum StateChange : byte
	{
		None = 0,				// not used - not this kind of event
		EnterCombat = 1,		// src_agent entered combat, dst_agent is subgroup
		ExitCombat = 2,			// src_agent left combat
		ChangeUp = 3,			// src_agent is now alive
		ChangeDead = 4,			// src_agent is now dead
		ChangeDown = 5,			// src_agent is now downed
		Spawn = 6,				// src_agent is now in game tracking range (not in realtime api)
		Despawn = 7,			// src_agent is no longer being tracked (not in realtime api)
		HealthUpdate = 8,		// src_agent has reached a health marker. dst_agent = percent * 10000 (eg. 99.5% will be 9950) (not in realtime api)
		LogStart = 9,			// log start. value = server unix timestamp **uint32**. buff_dmg = local unix timestamp. src_agent = 0x637261 (arcdps id) if evtc, npc id if realtime
		LogEnd = 10,			// log end. value = server unix timestamp **uint32**. buff_dmg = local unix timestamp. src_agent = 0x637261 (arcdps id)
		WeapSwap = 11,			// src_agent swapped weapon set. dst_agent = current set id (0/1 water, 4/5 land)
		MaxHealthUpdate = 12,	// src_agent has had it's maximum health changed. dst_agent = new max health (not in realtime api)
		PointOfView = 13,		// src_agent is agent of "recording" player
		Language = 14,			// src_agent is text language
		GwBuild = 15,			// src_agent is game build
		ShardID = 16,			// src_agent is sever shard id
		Reward = 17,			// src_agent is self, dst_agent is reward id, value is reward type. these are the wiggly boxes that you get
		BuffInitial = 18,		// combat event that will appear once per buff per agent on logging start (statechange==18, buff==18, normal cbtevent otherwise)
		Position = 19,			// src_agent changed, cast float* p = (float*)&dst_agent, access as x/y/z (float[3]) (not in realtime api)
		Velocity = 20,			// src_agent changed, cast float* v = (float*)&dst_agent, access as x/y/z (float[3]) (not in realtime api)
		Facing = 21,			// src_agent changed, cast float* f = (float*)&dst_agent, access as x/y (float[2]) (not in realtime api)
		TeamChange = 22,		// src_agent change, dst_agent new team id
		AttackTarget = 23,		// src_agent is an attacktarget, dst_agent is the parent agent (gadget type), value is the current targetable state (not in realtime api)
		TargetTable = 24,		// dst_agent is new target-able state (0 = no, 1 = yes. default yes) (not in realtime api)
		MapID = 25,				// src_agent is map id
		ReplInfo = 26,			// internal use, won't see anywhere
		StackActive = 27,		// src_agent is agent with buff, dst_agent is the stackid marked active
		StackReset = 28,		// src_agent is agent with buff, value is the duration to reset to (also marks inactive), pad61- is the stackid
		Guild = 29,				// src_agent is agent, dst_agent through buff_dmg is 16 byte guid (client form, needs minor rearrange for api form),
		BuffInfo = 30,			// is_flanking = probably invuln, is_shields = probably invert, is_offcycle = category, pad61 = stacking type, pad62 = probably resistance, src_master_instid = max stacks (not in realtime)
		BuffForumla = 31,		// (float*)&time[8]: type attr1 attr2 param1 param2 param3 trait_src trait_self, is_flanking = !npc, is_shields = !player, is_offcycle = break, overstack = value of type determined by pad61 (none/number/skill) (not in realtime, one per formula)
		SkillInfo = 32,			// (float*)&time[4]: recharge range0 range1 tooltiptime (not in realtime)
		SkillTiming = 33,		// src_agent = action, dst_agent = at millisecond (not in realtime, one per timing)
		BreakBarState = 34,		// src_agent is agent, value is u16 game enum (active, recover, immune, none) (not in realtime api)
		BreakBarPercen = 35,	// src_agent is agent, value is float with percent (not in realtime api)
		Error = 36,				// (char*)&time[32]: error string (not in realtime api)
		Tag = 37,				// src_agent is agent, value is the id (volatile, game build dependent) of the tag
		Unknown
	}
}
