using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.TaskManager;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState
{
	internal class UnstuckTask : FSMState<AIPlayer>
	{
		private static readonly UnstuckTask instance = new UnstuckTask();

		public static UnstuckTask Instance
		{
			get { return instance; }
		}

		static UnstuckTask()
		{

		}

		private UnstuckTask()
		{

		}

		public override void Enter(AIPlayer entity)
		{
			Debug.Log(string.Format("Player: {0}-{1}, START UnstuckTaskh", entity.Id, entity.Team));
			entity.Task = AIPlayer.AIPathTask.UnstuckPath;

			entity.SetTargetPosition(Map.GetRandomPointOnRadius(entity.CurrentBattle.Map, entity.Position, entity, 3));
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
					entity.ReturnToTaskBeforeStuck();
				}
				return;
			}

			bool goToTheTarget = entity.GoToTheTarget();

			if (goToTheTarget)
			{
				entity.ReturnToTaskBeforeStuck();
			}

			// Go to the our dropped flag to take it
			if (entity.IsMyFlagDropped || !entity.IsMyFlagOnBase || entity.IsThereEnemyInRadius(entity.MaxRadiusLocation))
			{
				entity.ReturnToTaskBeforeStuck();
			}
		}

		public override void Exit(AIPlayer entity)
		{
			Debug.Log(string.Format("Player: {0}-{1}, STOP UnstuckTask", entity.Id, entity.Team));
		}
	}
}
