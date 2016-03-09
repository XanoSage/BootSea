using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Base;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState {
	class TreverseDefensePoint: FSMState<AIPlayer>
	{
		static readonly TreverseDefensePoint instance = new TreverseDefensePoint();	

		public static TreverseDefensePoint Instance
		{
			get { return instance; }
		}

		static TreverseDefensePoint()
		{
			
		}

		private TreverseDefensePoint()
		{
			
		}

		public override void Enter(AIPlayer entity)
		{
			Debug.Log(string.Format("Player: {0}-{1}, START treverse defense path", entity.Id, entity.Team));

			entity.State = AIStates.TreverseDefensePoint;

			entity.SetNextTreversePoint();
		}

		public override void Execute(AIPlayer entity)
		{

			entity.ShootIfNeed();

			if (!entity.PathWasFound)
			{
				Debug.Log(string.Format("TreverseDefensePoint: Path for player: {0}-{1} has't yet found, pathfindState:{2}", entity.Id, entity.Team, entity.PathfindState));

				if (entity.PathfindState == AIPlayer.PathfinderState.ReturnPathError ||
				    entity.PathfindState == AIPlayer.PathfinderState.ReturnPathOk)
				{
					entity.ChangeState(Instance);
				}
				return;
			}

			bool goToTheTarget = entity.GoToTheTarget();

			if (goToTheTarget)
			{
				entity.ChangeState(Instance);
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
				return;
			}

			// Our base is under attack, need to go to it
			if (entity.MyBaseState == BaseState.Alarm && entity.IsMyShipPlacedNearMyBase)
			{
				//Debug.Break();
				entity.ChangeState(GoToTheOwnBaseAlarm.Instance);
				return;
			}

			// Follow the enemy ship to destroy it

			bool isEnemyFound = entity.IsShipInDirection(entity.Forward) || entity.IsShipInDirection(entity.Backward) || entity.IsShipInDirection(entity.Left) ||
								entity.IsShipInDirection(entity.Right);

			if (isEnemyFound && entity.EnemyPosition != default (Vector3))
			{
				entity.ChangeState(FollowToTheEnemy.Instance);
				return;
			}


			if (entity.IsThereEnemyInRadius(entity.MaxRadiusLocation))
			{
				entity.ChangeState(FollowToTheEnemy.Instance);
			}
		}

		public override void Exit(AIPlayer entity)
		{
			Debug.Log(string.Format("Player: {0}-{1}, STOP treverse defense path", entity.Id, entity.Team));
		}
	}
}
