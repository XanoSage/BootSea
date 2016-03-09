using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState
{
	internal class GoToRandomPoint : FSMState<AIPlayer>
	{
		private static readonly GoToRandomPoint instance = new GoToRandomPoint();

		public static GoToRandomPoint Instance
		{
			get { return instance; }
		}

		static GoToRandomPoint()
		{

		}

		private GoToRandomPoint()
		{

		}

		public override void Enter(AIPlayer entity)
		{
			entity.State = AIStates.GoToRandomPoint;

			Vector3 targetPosition = Map.GetRandomPositionOnMap(entity.CurrentBattle.Map);

		//	Debug.Log(string.Format("Player: {1}-{2}, go To The random point: {0}", targetPosition, entity.Id, entity.Team));

			entity.SetTargetPosition(targetPosition,
			                         string.Format("Player: {0}-{2}, go To The random point: {1}", targetPosition, entity.Id, entity.Team));
		}

		public override void Execute(AIPlayer entity)
		{
			entity.ShootIfNeed();

			if (!entity.PathWasFound)
				return;

			bool goToTheTarget = entity.GoToTheTarget();

			if (goToTheTarget)
			{
				entity.ChangeState(Instance);
			}
		}

		public override void Exit(AIPlayer entity)
		{
	//		Debug.Log(string.Format("Player: {0}-{1},  Stop to the random state", entity.Id, entity.Team));
		}


	}
}
