using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState
{
	internal class FollowToTheEnemy : FSMState<AIPlayer>
	{
		private static readonly FollowToTheEnemy instance = new FollowToTheEnemy();

		private float _researchTime = 0.5f;

		private float _researchTimeCounter;

		public static FollowToTheEnemy Instance
		{
			get { return instance; }
		}

		static FollowToTheEnemy()
		{

		}

		private FollowToTheEnemy()
		{

		}

		public override void Enter(AIPlayer entity)
		{
			Debug.Log(string.Format("Player: {0}-{1}, START follow the enemy ship", entity.Id, entity.Team));

			entity.State = AIStates.FollowToTheEnemy;

			entity.SetStartFollowPoint(entity.Position);

			entity.SetTargetPosition(entity.EnemyPosition);

			_researchTimeCounter = 0f;

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

			bool isEnemyFound = entity.IsShipInDirection(entity.Forward) || entity.IsShipInDirection(entity.Backward) ||
			                    entity.IsShipInDirection(entity.Left) ||
			                    entity.IsShipInDirection(entity.Right);

			bool goToTheTarget = entity.GoToTheTarget();

			//if (/*entity.EnemyPosition == default (Vector3) || */!entity.IsThereEnemyInRadius(entity.MaxFollowDistance))
			//{
			//	entity.FSM.RevertToMainState();
			//	return;
			//}

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
					//if we are destroyed the ship return to the main goal
					if (entity.EnemyPlayer.MyShip.StateOfShipBehaviour != Ship.Ship.ShipBehaviourState.Normal)
					{
						entity.RestartMainGoal();
						return;
					}

					entity.SetTargetPosition(entity.EnemyPlayer.Position);
					return;
				}

				if (!isEnemyFound)
				{
					entity.RestartMainGoal();
					return;
				}

				if (entity.IsThereEnemyInRadius(entity.MaxRadiusLocation))
				{
					entity.ChangeState(FollowToTheEnemy.Instance);
					return;
				}
			}

			if (entity.CurrentDistanceFollow >= entity.MaxFollowDistance)
			{
				entity.RestartMainGoal();
				return;
			}

			if (goToTheTarget)
			{
				if (isEnemyFound)
					entity.ChangeState(Instance);
			}

			//Debug.Log(string.Format("Player: {0}-{1}, I'am here", entity.Id, entity.Team));
		}

		public override void Exit(AIPlayer entity)
		{
			Debug.Log(string.Format("Player: {0}-{1}, STOP follow the enemy ship", entity.Id, entity.Team));
		}
	}
}
