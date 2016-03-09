using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;

namespace Aratog.NavyFight.Models.Unity3D.Battles {
	public class BattleConfigurator : IBattleConfigurator {

		#region Constants

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

		public const int FlagNeed = 5;
		public const int FragNeed = 10;
		public const int TimeNeeded = 10;

		public const int PlayersCountPlaying = 1;
		public const int SupportCountPlaying = 3;
		public const DifficultyType Type = DifficultyType.Normal;
		public const GameMode gameMode = GameMode.CaptureTheFlag;
		#endregion

		#region Variables

		public GameMode Mode { get; set; }
		public Map Map { get; set; }
		public List<string> Maps { get; set; }
		public int BlueTeamPlayersCount { get; set; }
		public int OrangeTeamPlayersCount { get; set; }
		public TeamColor Team { get; set; }
		public int SupportCount { get; set; }
		public DifficultyType Difficulty { get; set; }
		public int TimeNeed { get; set; }
		public float TimeSpent { get; set; }
		public int CountFlagNeed { get; set; }
		public int CountFragNeed { get; set; }
		public bool IsPrivateGame { get; set; }
		public MechanicsType Mechanics { get; set; }

		public void ClearData () {
			Mode = gameMode;
			Map = null;
			Maps = new List<string>();
			BlueTeamPlayersCount = PlayersCountPlaying;
			OrangeTeamPlayersCount = PlayersCountPlaying;
			SupportCount = SupportCountPlaying;
			Difficulty = Type;
			TimeNeed = TimeNeeded;
			TimeSpent = 0;
			CountFlagNeed = FlagNeed;
			CountFragNeed = FragNeed;
			IsPrivateGame = false;

			Mechanics = MechanicsType.Classic;
		}

		#endregion

		#region Constructor

		public BattleConfigurator () {
			Mode = gameMode;
			Map = null;
			Maps = new List<string>();
			BlueTeamPlayersCount = PlayersCountPlaying;
			OrangeTeamPlayersCount = PlayersCountPlaying;
			SupportCount = SupportCountPlaying;
			Difficulty = Type;
			TimeNeed = TimeNeeded;
			TimeSpent = 0;
			CountFlagNeed = FlagNeed;
			CountFragNeed = FragNeed;
			IsPrivateGame = false;
			Mechanics = MechanicsType.Classic;
		}

		public void UpdateData (IBattleConfigurator battle) {
			Map = battle.Map;
			Mode = battle.Mode;
			Maps = battle.Maps;
			BlueTeamPlayersCount = battle.BlueTeamPlayersCount;
			OrangeTeamPlayersCount = battle.OrangeTeamPlayersCount;
			SupportCount = battle.SupportCount;
			Difficulty = battle.Difficulty;
			TimeNeed = battle.TimeNeed;
			TimeSpent = battle.TimeSpent;
			CountFlagNeed = battle.CountFlagNeed;
			CountFragNeed = battle.CountFragNeed;
			Team = battle.Team;
			IsPrivateGame = battle.IsPrivateGame;

			Mechanics = battle.Mechanics;
		}

		#endregion

		public void Serialize (DataBuffer buffer) {
			buffer.Write(BlueTeamPlayersCount);
			buffer.Write(OrangeTeamPlayersCount);
			buffer.Write(SupportCount);

			buffer.Write(CountFlagNeed);
			buffer.Write(CountFragNeed);

			buffer.Write(IsPrivateGame);

			buffer.Write((int) Mode);

			buffer.Write(TimeNeed);
			buffer.Write(TimeSpent);

			buffer.Write((int) Mechanics);
		}

		public void Deserialize (DataBuffer buffer) {
			BlueTeamPlayersCount = buffer.ReadInt();
			OrangeTeamPlayersCount = buffer.ReadInt();
			SupportCount = buffer.ReadInt();

			CountFlagNeed = buffer.ReadInt();
			CountFragNeed = buffer.ReadInt();

			IsPrivateGame = buffer.ReadBool();

			Mode = (GameMode) buffer.ReadInt();

			TimeNeed = buffer.ReadInt();
			TimeSpent = buffer.ReadFloat();

			Mechanics = (MechanicsType) buffer.ReadInt();

		}

	}
}
