using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState
{
	internal class GoToTheOwnBaseToDeliverTheEnemyFlag : FSMState<AIPlayer>
	{

		static readonly GoToTheOwnBaseToDeliverTheEnemyFlag instance = new GoToTheOwnBaseToDeliverTheEnemyFlag();

		public static GoToTheOwnBaseToDeliverTheEnemyFlag Instance
		{
			get { return instance; }
		}

		static GoToTheOwnBaseToDeliverTheEnemyFlag()
		{
			
		}

		private GoToTheOwnBaseToDeliverTheEnemyFlag()
		{
			
		}

		public override void Enter(AIPlayer entity)
		{
			Debug.Log(string.Format("Player: {0}-{1}, Go to the own base to deliver the enemy flag", entity.Id, entity.Team));

			entity.State = AIStates.GoToTheOwnBaseToDeliverTheEnemyFlag;

			entity.SetTargetPosition(entity.MyBasePosition);
		}

		public override void Execute(AIPlayer entity)
		{
			entity.ShootIfNeed();

			if (!entity.PathWasFound)
				return;

			bool goToTheTarget = entity.GoToTheTarget();

			if (goToTheTarget)
			{
				//
			}


			// We are delivered enemy flag to our base
			if (!entity.MyShip.IsFlagTaken)
			{
				// Go To the enemy base
				if (entity.Tactic == AITactic.CaptureEnemy)
				{
					entity.ChangeState(GoToTheEnemyBaseToTakeTheFlag.Instance);
					return;
				}

				// go to the base tactic - Base Defence
				if (entity.Tactic == AITactic.BaseDefence)
				{
					entity.ChangeState(TreverseDefensePoint.Instance);
				}

			}

			// Go to the our dropped flag to take it
			if (entity.IsMyFlagDropped)
			{
				entity.ChangeState(GoToTheOwnDroppedFlag.Instance);
				return;
			}

			// Enemy ship has taken my flag
			if (!entity.IsMyFlagOnBase)
			{
				entity.ChangeState(PursuitOfTheEnemyWithMyFlag.Instance);
			}
		}

		public override void Exit(AIPlayer entity)
		{
			Debug.Log(string.Format("Player: {0}-{1}, STOP Go to the own base to deliver the enemy flag", entity.Id, entity.Team));
		}
	}
}
