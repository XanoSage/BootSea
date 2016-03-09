using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState
{
	internal class SupportShipWithFlag : FSMState<AIPlayer>
	{
		private static readonly SupportShipWithFlag instance = new SupportShipWithFlag();

		public static SupportShipWithFlag Instance
		{
			get { return instance; }
		}

		static SupportShipWithFlag ()
		{
			
		}

		private SupportShipWithFlag()
		{
			
		}

		public override void Enter(AIPlayer entity)
		{

			// Set the target to the ship with enemy flag
	//		Debug.Log(string.Format("Player: {0}-{1},  Go to the own ship with the enemy flag to support him", entity.Id, entity.Team));

			entity.State = AIStates.SupportShipWithFlag;

			entity.SetTargetPosition(entity.ShipPositionWithFlagInMyFleet);
		}

		public override void Execute(AIPlayer entity)
		{
			entity.ShootIfNeed();

			if (!entity.PathWasFound)
				return;

			bool goToTheTarget = entity.GoToTheTarget();

			if (goToTheTarget)
			{

				// When achieve the target and enemy flag was not deliver change state to this state
				if (entity.IsAnyShipInMyFleetTakeEnemyFlag)
				{
					entity.ChangeState(Instance);
				}
			}

			// Enemy flag was delivered and change state to base state
			if (entity.IsEnemyFlagOnBase)
			{
				entity.ChangeState(GoToTheEnemyBaseToTakeTheFlag.Instance);
				return;
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
				return;
			}

			// We are taken enemy flag
			if (entity.MyShip.IsFlagTaken)
			{
				entity.ChangeState(GoToTheOwnBaseToDeliverTheEnemyFlag.Instance);
			}
		}

		public override void Exit(AIPlayer entity)
		{
	//		Debug.Log(string.Format("Player: {0}-{1}, STOP Go to the own ship with the enemy flag to support him", entity.Id, entity.Team));
		}
	}
}
