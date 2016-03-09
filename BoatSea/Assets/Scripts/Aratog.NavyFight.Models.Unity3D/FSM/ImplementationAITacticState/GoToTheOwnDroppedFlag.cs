using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState {
	class GoToTheOwnDroppedFlag:FSMState<AIPlayer>
	{
		static readonly GoToTheOwnDroppedFlag instance = new GoToTheOwnDroppedFlag();

		public static GoToTheOwnDroppedFlag Instance
		{
			get { return instance; }
		}

		static GoToTheOwnDroppedFlag()
		{
			
		}

		private GoToTheOwnDroppedFlag()
		{
			
		}

		public override void Enter(AIPlayer entity)
		{
		//	Debug.Log(string.Format("Player: {0}-{1}, START to go to the own dropped flag", entity.Id, entity.Team));

			entity.State = AIStates.GoToTheOwnDroppedFlag;

			entity.SetTargetPosition(entity.MyFlagPosition);
		}

		public override void Execute(AIPlayer entity)
		{
			entity.ShootIfNeed();

			if (!entity.PathWasFound)
				return;

			bool goToTheTarget = entity.GoToTheTarget();

			// We was returned our flag to own base
			if (goToTheTarget)
			{
				// We came to the flag, and it elsewhere
				entity.ChangeState(Instance);
				return;
			}

			if (entity.IsMyFlagOnBase)
			{
				// Need to deliver enemy flag to the our base
				if (entity.MyShip.IsFlagTaken)
				{
					entity.ChangeState(GoToTheOwnBaseToDeliverTheEnemyFlag.Instance);
					return;
				}

				// Go to the Base tactic - Capture the enemy
				if (entity.Tactic == AITactic.CaptureEnemy)
				{
					entity.ChangeState(GoToTheEnemyBaseToTakeTheFlag.Instance);
					return;
				}

				// Go to the Base tactic - Base Defence
				if (entity.Tactic == AITactic.BaseDefence)
				{
					entity.ChangeState(TreverseDefensePoint.Instance);
				}
			}
		}

		public override void Exit(AIPlayer entity)
		{
	//		Debug.Log(string.Format("Player: {0}-{1}, STOP to go to the own dropped flag", entity.Id, entity.Team));
		}
	}
}
