using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Battles;
using Aratog.NavyFight.Models.Unity3D.Players;

namespace Assets.Scripts.Common.GameLogic {
	public interface IGameController
	{
		GameType CurrentGameType { get; set; }

		List<Player> Players { get; set; }

		Player Human { get; set; }

		List<Player> BlueTeamPlayers { get; set; }

		List<Player> RedTeamPlayers { get; set; }

		Battle CurrentBattle { get; set; }

		bool IsBattleStarted { get; set; }

		bool IsPause { get; set; }

		MechanicsType Mechanics { get; set; }

		Player GetPlayer(int playerId);
	}
}
