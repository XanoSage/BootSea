using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aratog.NavyFight.Models.UI {
	[Flags]
	public enum MenuID {
		None = 0x000,
		MainMenu = 0x001,
		OptionMenu = 0x002,
		CampaignMenu = 0x004,
		BattleModeMenu = 0x008,
	}
}
