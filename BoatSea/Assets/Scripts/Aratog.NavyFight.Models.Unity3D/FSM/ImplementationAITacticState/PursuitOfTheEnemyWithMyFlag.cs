using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState
{
	internal class PursuitOfTheEnemyWithMyFlag : FSMState<AIPlayer>
	{
		private static readonly PursuitOfTheEnemyWithMyFlag instance = new PursuitOfTheEnemyWithMyFlag();

		private float _researchTime = 1.5f;

		private float _researchTimeCounter = 0;

		public static PursuitOfTheEnemyWithMyFlag Instance
		{
			get { return instance; }
		}

		static PursuitOfTheEnemyWithMyFlag()
		{

		}

		private PursuitOfTheEnemyWithMyFlag()
		{

		}

		public override void Enter(AIPlayer entity)
		{
			Debug.Log(string.Format("Player: {0}-{1}, START pusuit of the enemy with our flag", entity.Id, entity.Team));

			entity.State = AIStates.PursuitOfTheEnemyWithMyFlag;

			entity.SetTargetPosition(entity.MyFlagPosition);
		}

		public override void Execute(AIPlayer entity)
		{
			entity.ShootIfNeed();

			if (!entity.PathWasFound)
				return;

			if (entity.IsMyFlagDropped)
			{
				entity.ChangeState(GoToTheOwnDroppedFlag.Instance);
				return;
			}

			bool goToTheTarget = entity.GoToTheTarget();

			if (goToTheTarget)
			{
				if (!entity.IsMyFlagOnBase && !entity.IsMyFlagDropped)
				{
					entity.ChangeState(Instance);
				}
			}

			if (_researchTimeCounter < _researchTime)
			{
				_researchTimeCounter += Time.deltaTime;
			}
			if (_researchTimeCounter >= _researchTime)
			{
				_researchTimeCounter = 0f;

				//Debug.Log(string.Format("Player: {0}-{1}, restart sсan waypoints, my flag position:{2}", entity.Id, entity.Team, entity.MyFlagPosition));

				if (entity.EnemyPlayer != null)
				{
					//entity.SetTargetPosition(entity.MyFlagPosition);
					entity.ChangeState(this);
				}

				if (!entity.IsMyFlagOnBase && !entity.IsMyFlagDropped)
				{
					//entity.ChangeState(Instance);
					entity.SetTargetPosition(entity.MyFlagPosition);
				}
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
			Debug.Log(string.Format("Player: {0}-{1}, STOP pursuit if the enemy with our flag", entity.Id, entity.Team));
		}
	}
}
