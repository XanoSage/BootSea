using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Base;
using Aratog.NavyFight.Models.Unity3D.Flags;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;

namespace Aratog.NavyFight.Models.Unity3D.Battles {
	public class Battle : IInitable, IBattleConfigurator
	{

		#region Variables

		// Режим игры

		public GameMode Mode { get; set; }

		//Карта

		public Map Map { get; set; }

		public List<string> Maps { get; set; }

		//Количество игроков принимающих участие в сражении

		public int BlueTeamPlayersCount { get; set; }

		public int OrangeTeamPlayersCount { get; set; }
		public TeamColor Team { get; set; }

		//Количество кораблей поддержки для каждого игрока

		public int SupportCount { get; set; }

		//Сложность игры для одиночной компании и баттлмода с предустановленными режимами

		public DifficultyType Difficulty { get; set; }

		// Время нужное для прохождения сражения: 0 и больше игра с учетом времени, меньше 0 - без учета времени

		public int TimeNeed { get; set; }

		// Время прошедшее в битве

		public float TimeSpent { get; set; }



		// Количество флагов необходимых для победы: 0 и меньше - параметр не учитывается
		public int CountFlagNeed { get; set; }

		// Количество фрагов необходимых для победы: 0 и меньше - параметр не учитывается

		public int CountFragNeed { get; set; }

		// Для мультиплеера является ли сражение приватным
		public bool IsPrivateGame { get; set; }
		public MechanicsType Mechanics { get; set; }

		//Для отслеживания переходов в главном меню, и если битва была уже создана, то вызывать методы по сбросу параметров
		public bool IsBattleCreated { get; set; }

		public FlagParent BlueFlag;

		public FlagParent OrangeFlag;

		public BaseParent BlueBase;

		public BaseParent OrangeBase;

		public int MaxCountPlayers
		{
			get { return BlueTeamPlayersCount*(SupportCount + 1) + OrangeTeamPlayersCount*(SupportCount + 1); }
		}


		public int BlueFlagCounter { get; set; }
		public int OrangeFlagCounter { get; set; }

		public int BlueFragCounter { get; set; }
		public int OrangeFragCounter { get; set; }

		#endregion

		public void ClearData()
		{
			Mode = BattleConfigurator.gameMode;
			Map = null;
			Maps = new List<string>();
			BlueTeamPlayersCount = BattleConfigurator.PlayersCountPlaying;
			OrangeTeamPlayersCount = BattleConfigurator.PlayersCountPlaying;
			SupportCount = BattleConfigurator.SupportCountPlaying;
			Difficulty = BattleConfigurator.Type;
			TimeNeed = BattleConfigurator.TimeNeeded;
			TimeSpent = 0;
			CountFlagNeed = BattleConfigurator.FlagNeed;
			CountFragNeed = BattleConfigurator.FragNeed;
			IsPrivateGame = false;
			Team = TeamColor.BlueTeam;

			Mechanics = MechanicsType.Classic;

			BlueFlagCounter = 0;
			BlueFragCounter = 0;

			OrangeFlagCounter = 0;
			OrangeFragCounter = 0;

			IsBattleCreated = false;

			BlueBase = null;
			BlueFlag = null;

			OrangeBase = null;
			OrangeFlag = null;
		}


		public Battle(IBattleConfigurator battle)
		{
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

			BlueFlag = null;
			OrangeFlag = null;

			BlueBase = null;
			OrangeBase = null;
		}

		#region Events

		public void OnGameModeChange()
		{
		}


		public void OnPlayersCountChange()
		{

		}

		/// <summary>
		/// Проверка на окончание битвы
		/// </summary>
		/// <returns></returns>
		public bool IsBattleEnd()
		{
			return false;
		}

		public void OnBattleEnd()
		{

		}

		public void Init()
		{
			throw new NotImplementedException();
		}

		public void UpdateData(IBattleConfigurator battle)
		{
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

		public void Serialize(DataBuffer buffer)
		{
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

		public void Deserialize(DataBuffer buffer)
		{
			BlueTeamPlayersCount = buffer.ReadInt();
			OrangeTeamPlayersCount = buffer.ReadInt();
			SupportCount = buffer.ReadInt();

			CountFlagNeed = buffer.ReadInt();
			CountFragNeed = buffer.ReadInt();

			IsPrivateGame = buffer.ReadBool();

			Mode = (GameMode) buffer.ReadInt();

			TimeNeed = buffer.ReadInt();
			TimeSpent = buffer.ReadInt();

			Mechanics = (MechanicsType) buffer.ReadInt();
		}

	}
}
