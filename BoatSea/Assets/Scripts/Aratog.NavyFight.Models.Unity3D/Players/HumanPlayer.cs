using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Players {
	public class HumanPlayer: Player {
		#region Variables
		
		public PlayerProfile Profile;
		#endregion

		#region Constructor

		public HumanPlayer (TeamColor team = TeamColor.BlueTeam) : base(true, team) {
			Type = PlayerType.HumanPlayer;
			Profile = new PlayerProfile();
		}

		public void SetHumanPlayerName (string name) {
			if (Profile == null) {
				Debug.Log(string.Format("SetHumanPlayerName: Error when trying to set name {0}, Profile is Null", name));
				return;
			}

			Profile.Name = name;
		}

		#endregion
	}
}
