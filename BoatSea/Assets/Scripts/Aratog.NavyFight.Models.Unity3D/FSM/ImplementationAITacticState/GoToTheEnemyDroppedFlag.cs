using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState
{
	internal class GoToTheEnemyDroppedFlag : FSMState<AIPlayer>
	{
		private static readonly GoToTheEnemyDroppedFlag instance = new GoToTheEnemyDroppedFlag();

		public static GoToTheEnemyDroppedFlag Instance
		{
			get { return instance; }
		}

		static GoToTheEnemyDroppedFlag()
		{

		}

		private GoToTheEnemyDroppedFlag()
		{

		}

		public override void Enter(AIPlayer entity)
		{
	//		Debug.Log(string.Format("Player: {0}-{1}, START go to the enemy dropped flag", entity.Id, entity.Team));

			entity.State = AIStates.GoToTheEnemyDroppedFlag;

			entity.SetTargetPosition(entity.EnemyFlagPosition);
		}

		public override void Execute(AIPlayer entity)
		{
			entity.ShootIfNeed();

			if (!entity.PathWasFound)
			{
				return;
			}

			bool goToTheTarget = entity.GoToTheTarget();

			if (goToTheTarget)
			{
				entity.ChangeState(GoToTheOwnBaseToDeliverTheEnemyFlag.Instance);
			}
		}

		public override void Exit(AIPlayer entity)
		{
		//	Debug.Log(string.Format("Player: {0}-{1}, STOP go to the enemy dropped flag", entity.Id, entity.Team));
		}
	}
}
