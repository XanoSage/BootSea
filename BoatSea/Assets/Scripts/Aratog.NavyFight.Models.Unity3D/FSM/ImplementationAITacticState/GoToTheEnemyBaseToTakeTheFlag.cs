using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState
{
	internal class GoToTheEnemyBaseToTakeTheFlag : FSMState<AIPlayer>
	{
		private static readonly GoToTheEnemyBaseToTakeTheFlag instance = new GoToTheEnemyBaseToTakeTheFlag();

		public static GoToTheEnemyBaseToTakeTheFlag Instance
		{
			get { return instance; }
		}

		static GoToTheEnemyBaseToTakeTheFlag()
		{
			
		}

		private GoToTheEnemyBaseToTakeTheFlag()
		{
			
		}

		public override void Enter(AIPlayer entity)
		{
			Debug.Log(string.Format("Player: {0}-{1}, START Go to the enemy base to take the flag", entity.Id, entity.Team));

			entity.State = AIStates.GoToTheEnemyBaseToTakeTheFlag;

			entity.SetTargetPosition(entity.EnemyFlagPosition);
		}

		public override void Execute(AIPlayer entity)
		{
			entity.ShootIfNeed();

			if (!entity.PathWasFound)
			{
				Debug.Log(string.Format("GoToTheEnemyBaseToTakeTheFlag: Path for player: {0}-{1} has't yet found", entity.Id, entity.Team));
				if (entity.PathfindState == AIPlayer.PathfinderState.ReturnPathError ||
				    entity.PathfindState == AIPlayer.PathfinderState.ReturnPathOk)
				{
					entity.ChangeState(Instance);
				}
				return;
			}

			if (entity.IsThereEnemyInRadius(entity.MaxRadiusLocation))
			{
				entity.ChangeState(FollowToTheEnemy.Instance);
				return;
			}

			bool goToTheTarget = entity.GoToTheTarget();

			if (goToTheTarget)
			{
				//entity.ChangeState(Instance);
			}


			// We are take the flag and go to the own base to deliver this flag
			if (entity.MyShip.IsFlagTaken)
			{
				entity.ChangeState(GoToTheOwnBaseToDeliverTheEnemyFlag.Instance);
				return;
			}

			// Some players on my team already take the flag
			if (entity.IsAnyShipInMyFleetTakeEnemyFlag)
			{
				// go to the player with the enemy flag
				if (entity.Tactic == AITactic.CaptureEnemy)
				{
					entity.ChangeState(SupportShipWithFlag.Instance);
					return;
				}
				
				// go to the base tactic - Base Defence
				if (entity.Tactic == AITactic.BaseDefence)
				{
					entity.ChangeState(TreverseDefensePoint.Instance);
					return;
				}
			}

			// My flag is dropped, need to go to take him
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
			Debug.Log(string.Format("Player: {0}-{1},  STOP Go to the enemy base to take the flag", entity.Id, entity.Team));
		}
	}
}
