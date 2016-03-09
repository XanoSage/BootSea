using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;

namespace Aratog.NavyFight.Models.Unity3D.Battles {
	public interface IBattleConfigurator: ISerializable {
		#region Variables

		// Режим игры
		GameMode Mode { get; set; }

		//Карта
		Map Map { get; set; }
		List<string> Maps { get; set; }

		//Количество игроков принимающих участие в сражении
		int BlueTeamPlayersCount { get; set; }

		int OrangeTeamPlayersCount { get; set; }

		TeamColor Team { get; set; }

		//Количество кораблей поддержки для каждого игрока
		int SupportCount { get; set; }
		
		//Сложность игры для одиночной компании и баттлмода с предустановленными режимами
		DifficultyType Difficulty { get; set; }
		
		// Время нужное для прохождения сражения: 0 и больше игра с учетом времени, меньше 0 - без учета времени
		int TimeNeed { get; set; }

		// Время прошедшее в битве
		float TimeSpent { get; set; }

		// Количество флагов необходимых для победы: 0 и меньше - параметр не учитывается
		int CountFlagNeed { get; set; }

		// Количество фрагов необходимых для победы: 0 и меньше - параметр не учитывается
		int CountFragNeed { get; set; }

		// Приватная игра или нет, используется для мультиплеера
		bool IsPrivateGame { get; set; }

		MechanicsType Mechanics { get; set; }

		// Сброс параметров битвы
		void ClearData ();

		// TODO: добавить переменные, которые будут описывать режим Survival and NavalConvoy

		#endregion
	}
}
