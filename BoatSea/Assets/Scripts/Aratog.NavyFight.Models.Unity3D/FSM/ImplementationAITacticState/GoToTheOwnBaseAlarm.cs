using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState
{
	internal class GoToTheOwnBaseAlarm : FSMState<AIPlayer>
	{
		static readonly GoToTheOwnBaseAlarm instance = new GoToTheOwnBaseAlarm();

		public static GoToTheOwnBaseAlarm Instance
		{
			get { return instance; }
		}

		static GoToTheOwnBaseAlarm()
		{
			
		}

		private GoToTheOwnBaseAlarm()
		{
			
		}

		public override void Enter(AIPlayer entity)
		{
		//	Debug.Log(string.Format("Player: {0}-{1}, START go to the own base alarm", entity.Id, entity.Team));

			entity.State = AIStates.GoToTheOwnBaseAlarm;

			entity.SetTargetPosition(entity.AroundMyBasePoint);
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
				if (entity.Tactic == AITactic.CaptureEnemy)
				{
					entity.ChangeState(GoToTheEnemyBaseToTakeTheFlag.Instance);
					return;
				}

				if (entity.Tactic == AITactic.BaseDefence)
				{
					entity.ChangeState(TreverseDefensePoint.Instance);
				}
			}

			// Follow the enemy ship to destroy it

			bool isEnemyFound = entity.IsShipInDirection(entity.Forward) || entity.IsShipInDirection(entity.Backward) || entity.IsShipInDirection(entity.Left) ||
								entity.IsShipInDirection(entity.Right);

			if (isEnemyFound && entity.EnemyPosition != default (Vector3))
			{
				entity.ChangeState(FollowToTheEnemy.Instance);
			}
		}

		public override void Exit(AIPlayer entity)
		{
		//	Debug.Log(string.Format("Player: {0}-{1}, STOP go to the own base alarm", entity.Id, entity.Team));
		}
	}
}
