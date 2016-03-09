using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aratog.NavyFight.Models.Maps
{
	[Flags]
	public enum WhereUsing
	{
		SingleGameUnlock = 0x001,
		MultiplayerUnlock = 0x002,
		BattleModeGameUnlock = 0x004,
		CampaignGameLock = 0x008,
		SingleGameLock = 0x010,
		MultiplayerLock = 0x020,
		BattleModeGameLock = 0x040
	}
}