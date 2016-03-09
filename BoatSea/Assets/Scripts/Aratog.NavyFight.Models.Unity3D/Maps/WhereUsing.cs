using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;

namespace Aratog.NavyFight.Models.Unity3D.Maps {
	[Flags]
	public enum WhereUsing {

		SingleGameUnlock = 0x001,
		MultiplayerUnlock = 0x002,
		BattleModeGameUnlock = 0x004,
		CampaignGameUnock = 0x008,
		SingleGameLock = 0x010,
		MultiplayerLock = 0x020,
		BattleModeGameLock = 0x040,
		CampaignGameLock = 0x080,

	}
}
