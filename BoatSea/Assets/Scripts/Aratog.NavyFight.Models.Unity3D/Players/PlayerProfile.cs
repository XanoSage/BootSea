using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;

namespace Aratog.NavyFight.Models.Unity3D.Players {
	public class PlayerProfile {
		#region Variables

		public bool IsProfileLinkedWithGameAccount { get; set; }

		public string Name { get; set; }

		public uint PIN { get; private set; }

		public uint PINConfirm { get; private set; }

		public bool IsPinSetted;
		
		public PlayerProfile (string name = "Player") {

			IsProfileLinkedWithGameAccount = false;
			if (name == "Player")
				name += Guid.NewGuid();
			Name = name;

			PIN = 0;
			PINConfirm = 0;
			IsPinSetted = false;
		}

		#endregion
	}
}
