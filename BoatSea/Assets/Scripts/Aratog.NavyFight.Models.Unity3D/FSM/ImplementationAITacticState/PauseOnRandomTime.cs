using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState {
	class PauseOnRandomTime: FSMState<AIPlayer>
	{
		private static readonly PauseOnRandomTime instance = new PauseOnRandomTime();

		public static PauseOnRandomTime Instance
		{
			get { return instance; }
		}

		static PauseOnRandomTime()
		{
			
		}

		private PauseOnRandomTime()
		{
			
		}

		public override void Enter(AIPlayer entity)
		{
			entity.PauseTime = Random.Range(entity.MinPauseTime, entity.MaxPauseTime);

			entity.PauseCounter = 0f;

			entity.State = AIStates.PauseOnRandomTime;

		//	Debug.Log(string.Format("Player: {0}-{1},  STOP take some pause: {2} second(s)", entity.Id, entity.Team,
			//                        entity.PauseTime));
		}

		public override void Execute(AIPlayer entity)
		{
			entity.ShootIfNeed();

			if (entity.PauseCounter < entity.PauseTime)
			{
				entity.PauseCounter += Time.deltaTime;
			} 

			if (entity.PauseCounter >= entity.PauseTime)
			{
				entity.FSM.RevertToPreviousState();
			}
		}

		public override void Exit(AIPlayer entity)
		{
	//		Debug.Log(string.Format("Player: {0}-{1},  STOP take some pause: {2} second(s)", entity.Id, entity.Team,
		//	                        entity.PauseTime));
		}
	}
}
