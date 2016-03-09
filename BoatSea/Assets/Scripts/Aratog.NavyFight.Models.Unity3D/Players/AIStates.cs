using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;

namespace Aratog.NavyFight.Models.Unity3D.Players {
	public enum AIStates
	{
		None,
		Walk,
		GoToRandomPoint,
		UnstuckPath,
		ReturnToMainPath,
		GoToTheEnemyBaseToTakeTheFlag,
		GoToTheOwnBaseToDeliverTheEnemyFlag,
		GoToTheOwnDroppedFlag,
		GoToTheEnemyDroppedFlag,
		SupportShipWithFlag,
		TreverseDefensePoint,
		GoToTheOwnBaseAlarm,
		PursuitOfTheEnemyWithMyFlag,
		FollowToTheEnemy,
		PauseOnRandomTime,

	}
}
