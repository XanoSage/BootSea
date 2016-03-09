using Aratog.NavyFight.Models.Unity3D.Base;
using Aratog.NavyFight.Models.Unity3D.FSM.ImplementationAITacticState;
using Aratog.NavyFight.Models.Unity3D.Flags;
using Aratog.NavyFight.Models.Unity3D.TaskManager;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Battles;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Extensions;
using Random = System.Random;
using Aratog.NavyFight.Models.Unity3D.Weapons;

namespace Aratog.NavyFight.Models.Unity3D.Players
{
	public class AIPlayer : Player {
		#region Constant

		public float MinPauseTime = 0.5f;
		public float MaxPauseTime = 5f;

		#endregion

		#region Variables

		/// <summary>
		/// Основная тактика ИИ, по которому будет определятся его поведение
		/// </summary>
		public AITactic Tactic { get; set; }

		/// <summary>
		/// Состояние ИИ
		/// </summary>
		private AIStates state;

		public AIStates State
		{
			get { return state; }
			set { state = value; }
		}

		/// <summary>
		/// Диспетчер задач, для каждого юнита, управляемого компьютером будет свой ДЗ
		/// </summary>
		public readonly TaskManager.TaskManager taskManager;

		public readonly FiniteStateMachine<AIPlayer> FSM;

		public Vector3 CurrentTargetPosition;

		public Vector3 GoalPosition;
		private Vector3 _mainGoalPosition;

		public List<Point> TargetPath;

		public List<Vector3> TargetPathVector;

		private int _currentWaypoint = 0;

		private List<Point> _mainTargetPath;

		private List<Vector3> _mainTargetPathVector;

		public bool IsLocal { get; private set; }

		// TODO: Temporary to test AI beaviour

		private List<Vector3> previousPosition;

		//private const int PreviousCount = 15;

		public static Point BlueBasePoint;
		public static Point OrangeBasePoint;

		public static BaseParent BlueBase;
		public static BaseParent OrangeBase;


		private int countOfAttemps;
		private const int NumberOfAttemps = 5;

		private AIPathTask _previousPathTask = AIPathTask.None;

		public float PauseTime;
		public float PauseCounter;



        // Параметры доп.Оружия
        public WeaponsType upgradeWeapon;
        public int upgradeWeaponNumber;

        // Параметры Доп Модификацый
        public UpgradesType[] upgrades;


		#endregion

		#region Constructor

		public AIPlayer(bool isCaptain = false, TeamColor team = TeamColor.BlueTeam, bool isLocal = true)
			: base(isCaptain, team)
		{
			Type = PlayerType.AIPlayer;

			taskManager = new TaskManager.TaskManager(true);

			previousPosition = new List<Vector3>();
			state = AIStates.Walk;
			Tactic = AITactic.BaseDefence;
			CurrentTargetPosition = Vector3.zero;


            upgradeWeapon = new WeaponsType();
            upgradeWeaponNumber =1;
           

			TargetPath = new List<Point>();

			IsLocal = isLocal;

			countOfAttemps = 0;

			FSM = new FiniteStateMachine<AIPlayer>();
		}

		#endregion

		#region Events

		public delegate void OnPathFindDelegate(List<Vector3> wayPoints);

		public delegate void FindPathHandler(Vector3 start, Vector3 goal, OnPathFindDelegate onPathFind);

		private FindPathHandler findPathHandler;

		public event FindPathHandler FindPathEvent
		{
			add { findPathHandler += value; }
			remove { findPathHandler -= value; }
		}


		public void ChangeState(FSMState<AIPlayer> machineState)
		{
			FSM.ChangeState(machineState);
		}

		/// <summary>
		/// Выбор тактики ИИ в зависисмости от условий
		/// 1. 3 охраняют флаг – 1 нападает
		/// 2. 2 охраняют флаг – 2 нападают
		/// 3. 1 охраняет – 3 нападают
		/// </summary>
		/// <param name="players"> список игроков под управлением ИИ</param>
		/// <param name="choise">выбор тактики один из трех</param>
		public static void AiTacticPreset(List<AIPlayer> players, int choise)
		{
			int playersSelect = players.Count;
			if (playersSelect > 2)
			{
				for (int i = 0; i != players.Count; i++)
				{

					switch (choise)
					{
						case 1:
							players[i].Tactic = i <= (playersSelect/2f) ? AITactic.BaseDefence : AITactic.CaptureEnemy;
							break;
						case 2:
							players[i].Tactic = i < playersSelect/2 ? AITactic.BaseDefence : AITactic.CaptureEnemy;
							break;
						case 3:
							players[i].Tactic = i <= playersSelect/2 ? AITactic.CaptureEnemy : AITactic.BaseDefence;
							break;
					}

				}
			}
		}

		
		public static ShipType TypeShipPresetByAiTactic(AITactic tactic, float chance)
		{
			ShipType shipType = ShipType.Boat;

			switch (tactic)
			{
				case AITactic.BaseDefence:
					if (chance >= 0f && chance <= 0.1f)
						shipType = ShipType.Boat;
					else if (chance > 0.1f && chance <= 0.5f)
						shipType = ShipType.Submarine;
					else if (chance > 0.5f && chance <= 1f)
						shipType = ShipType.BigShip;
					break;
				case AITactic.CaptureEnemy:
					if (chance >= 0f && chance < 0.5f)
						shipType = ShipType.Boat;
					else if (chance > 0.5f && chance <= 0.9f)
						shipType = ShipType.Submarine;
					else if (chance > 0.9f && chance <= 1f)
						shipType = ShipType.BigShip;
					break;
			}
			return shipType;
		}

		#region Change AiTactit when flag is delivered
		
		private static List<AITactic> CreateAiTacticList(int playersCount, int choise)
		{
			List<AITactic> tactics = new List<AITactic>();

			//Debug.Log(string.Format("Choise: {0}", choise));


			for (int i = 0; i != playersCount; i++)
			{
				AITactic tactic = AITactic.BaseDefence;
				switch (choise)
				{
					case 1:
						tactic = i <= (playersCount/2f) ? AITactic.BaseDefence : AITactic.CaptureEnemy;
						break;
					case 2:
						tactic = i < playersCount/2 ? AITactic.BaseDefence : AITactic.CaptureEnemy;
						break;
					case 3:
						tactic = i <= playersCount/2 ? AITactic.CaptureEnemy : AITactic.BaseDefence;
						break;
				}

			//	Debug.Log(string.Format("Current tactic[{0}]: {1}", i, tactic));

				tactics.Add(tactic);
			}

			return tactics;
		}
		
		/// <summary>
		/// Смена тактики АИ у игроков одной команды.
		/// </summary>
		/// <param name="aiPlayers">Список АИ игроков одной команды</param>
		public static void ChangeAiPlayerTactic(List<AIPlayer> aiPlayers)
		{
			// выбираем общую тактику (смотри ГДД)
			int choise = UnityEngine.Random.Range(1, 3);

			// на основе выбранной общей тактики формируем список нужных тактик для каждого корабля
			List<AITactic> aiTactics = CreateAiTacticList(aiPlayers.Count, choise);

			foreach (AIPlayer aiPlayer in aiPlayers)
			{
				if (aiPlayer.Tactic != AITactic.CaptureEnemy && aiPlayer.Tactic != AITactic.BaseDefence)
					continue;

			//	Debug.Log(string.Format("Tactic was Id: {0}-{1}, tactic: {2}", aiPlayer.Id, aiPlayer.Team, aiPlayer.Tactic));

				if (IsExistDifferentAiTactic(aiTactics))
				{
					// задаём случайную вероятность для выбора тактики
					float chance = UnityEngine.Random.Range(0, 100)/100f;

					// получаем нужную тактику на основе вероятности и типу корабля
					AITactic tactic = AiTacticPresetByShipType(aiPlayer, chance);

					// получаем индекс такой тактики в списке тактик
					int i = aiTactics.IndexOf(aiTactics.FirstOrDefault(someTactic => someTactic == tactic));

					aiPlayer.Tactic = aiTactics[i];

					// удаляем её из списка. Повторяем пока не останется в списке только один вид тактики
					aiTactics.RemoveAt(i);
				}
				else
				{
					// если остаётся один вид тактик, то просто присваиваем эту тактику оставшимся кораблям
					aiPlayer.Tactic = aiTactics[0];
					aiTactics.RemoveAt(0);
				}

		//		Debug.Log(string.Format("Tactic were Id: {0}-{1}, tactic: {2}", aiPlayer.Id, aiPlayer.Team, aiPlayer.Tactic));

				aiPlayer.OnTacticChange();
			}
		}

		// Проверить, что в списке есть различные тактики
		private static bool IsExistDifferentAiTactic(List<AITactic> aiTactics)
		{
			if (aiTactics == null || aiTactics.Count < 1)
				return false;

			AITactic aiTactic = aiTactics[0];

			foreach (AITactic tactic in aiTactics)
			{
				if (aiTactic != tactic)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Получить тактику АИ по шансу и типу корабля
		/// </summary>
		/// <param name="player"> АИ игрок</param>
		/// <param name="chance">вероятность</param>
		/// <returns></returns>
		private static AITactic AiTacticPresetByShipType(AIPlayer player, float chance)
		{
			AITactic aiTactic = AITactic.CaptureEnemy;

			switch (player.MyShip.Type)
			{
				case ShipType.BigShip:
					
					if (chance > 0 && chance <= 0.8f)
						aiTactic = AITactic.BaseDefence;
					else if (chance > 0.8f && chance <= 1f)
					{
						aiTactic = AITactic.CaptureEnemy;
					}

					break;

				case ShipType.Boat:
					
					if (chance > 0 && chance <= 0.8f)
						aiTactic = AITactic.CaptureEnemy;
					else if (chance > 0.8f && chance <= 1f)
						aiTactic = AITactic.BaseDefence;

					break;

				case ShipType.Submarine:
					
					if (chance > 0 && chance <= 0.5f)
						aiTactic = AITactic.CaptureEnemy;
					else if (chance > 0.5f && chance < 1f)
						aiTactic = AITactic.BaseDefence;

					break;

				default:
					aiTactic = AITactic.CaptureEnemy;
					break;
			}

			return aiTactic;
		}

		#endregion

		/// <summary>
		/// Move to target position
		/// </summary>
		/// <param name="args">0 arg - </param>
		/// <returns></returns>
		public bool OnMove(params object[] args)
		{
			if (state != AIStates.Walk || IsReachingTargetPos(eps, CurrentTargetPosition))
				return true;

		//	Debug.Log("OnMove() from AiPlayer.cs");

			base.OnMove(Direction);

			return false;
		}

		private bool IsPlayerStuck()
		{
			if (previousPosition.Count < PreviousCount)
				return false;

			float epsilon = 0.15f;

			for (int i = 0; i < previousPosition.Count; i++)
			{
				if (Math.Abs(previousPosition[i].x - Position.x) > epsilon || Math.Abs(previousPosition[i].z - Position.z) > epsilon)
					return false;
			}

			//Debugger.Debugger.Log(string.Format("Player stuck : {0}", IsPlayerIsInCollision));
			//Debug.Break();
			return true;
		}

		public void StartAiBasic()
		{
			CurrentTargetPosition = Map.GetRandomPositionOnMap(currentMap);
			Direction = (CurrentTargetPosition - Position).normilizedWithoutY();
			taskManager.AddTask(OnMove);
			taskManager.AddPause(20);
		}

		public void StartAiAdvanced()
		{


			//Debug.Log(string.Format("AiPlayer ID: {0} StartAiAdvanced, Position = {1}", Id, Position));

			switch (Tactic)
			{
				case AITactic.CaptureEnemy:

					FSM.Configure(this, GoToTheEnemyBaseToTakeTheFlag.Instance);
					break;
				case AITactic.BaseDefence:

					FSM.Configure(this, TreverseDefensePoint.Instance);

					break;
				default:

					FSM.Configure(this, GoToTheEnemyBaseToTakeTheFlag.Instance);

					break;
			}

		}

		public void OnTacticChange()
		{
			switch (Tactic)
			{
				case AITactic.CaptureEnemy:
					//ChangeState(GoToTheEnemyBaseToTakeTheFlag.Instance);
					FSM.Configure(GoToTheOwnBaseToDeliverTheEnemyFlag.Instance);
					break;

				case AITactic.BaseDefence:
					//ChangeState(TreverseDefensePoint.Instance);
					FSM.Configure(TreverseDefensePoint.Instance);
					break;

				default:
					//ChangeState(GoToTheEnemyBaseToTakeTheFlag.Instance);
					FSM.Configure(GoToTheEnemyBaseToTakeTheFlag.Instance);
					break;
			}
		}

		#region Inerface implementation

		public override void Init()
		{
			base.Init();
		}

		public override void Serialize(DataBuffer buffer)
		{
			base.Serialize(buffer);

			buffer.Write(IsLocal);
		}

		public override void Deserialize(DataBuffer buffer)
		{
			base.Deserialize(buffer);

			IsLocal = buffer.ReadBool();
		}

		#endregion

		#endregion

		#region Ovverride function and events

		public override void OnShipSunked()
		{
			base.OnShipSunked();


			TargetPath = null;

			Task = AIPathTask.None;

		//	Debug.Log(string.Format("AiPlayer: OnShipSunked: Id {0}, Position {1}", Id, Position));

			taskManager.RemoveAllTask();

		}

		protected override void OnShipRespawn(bool invulnerability = false)
		{
			base.OnShipRespawn(invulnerability);

			StartAiAdvanced();
		}

		#endregion

		#region Unstuck event

		//public void UnstuckTask()
		//{
		//	if (Mechanics == MechanicsType.Classic)
		//	{

		//		if (TargetPath.Count > 1)
		//		{
		//			//		Debug.Log(string.Format("UnstuckTask:trying to unstuck, Id:{0}", Id));
		//			//SetTargetPosition(GoalPosition);

		//			if (_mainTargetPath == null)
		//			{
		//				_mainTargetPath = new List<Point>(TargetPath);
		//				_mainGoalPosition = GoalPosition;
		//				_previousPathTask = Task;
		//			}

		//			Task = AIPathTask.None;

		//			countOfAttemps ++;

		//			if (countOfAttemps >= NumberOfAttemps)
		//			{
		//				SetTargetPosition(_mainGoalPosition);
		//				Debugger.Debugger.Log(string.Format("Was trying more then {1} attemps calculate path again, Id:{0}", Id,
		//													countOfAttemps));
		//				ResetUnstuckData();
		//				return;
		//			}

		//			taskManager.AddUrgentInstantTask(Unstack, "Unstuck task");
		//		}
		//		else
		//		{
		//			TargetPath.Clear();
		//			Task = AIPathTask.None;

		//			ResetUnstuckData();
		//		}
		//	}
		//	else if (Mechanics == MechanicsType.NewWave)
		//	{
		//		if (TargetPathVector.Count > 1)
		//		{
		//			if (_mainTargetPathVector == null)
		//			{
		//				_mainTargetPathVector = new List<Vector3>(TargetPathVector);
		//				_mainGoalPosition = GoalPosition;
		//				_previousPathTask = Task;
		//			}
		//			Task = AIPathTask.None;

		//			countOfAttemps ++;

		//			if (countOfAttemps >= NumberOfAttemps)
		//			{
		//				SetTargetPosition(_mainGoalPosition);
		//				Debug.Log(string.Format("Was trying more then {1} attemps calculate path again, Id:{0}", Id,
		//										countOfAttemps));
		//				ResetUnstuckData();
		//				return;

		//			}
		//			taskManager.AddUrgentInstantTask(Unstack, "Unstuck task");
		//		}
		//		else
		//		{
		//			TargetPathVector.Clear();
		//			Task = AIPathTask.None;

		//			ResetUnstuckData();
		//		}
		//	}
		//}

		private void ResetUnstuckData()
		{
			_mainTargetPath = null;

			_mainTargetPathVector = null;
			
			_mainGoalPosition = Vector3.zero;
			
			countOfAttemps = 0;

			_previousPathTask = AIPathTask.None;
		}

		private int unstackRadius = 3;

		public bool Unstack(params object[] args)
		{
			if (Task == AIPathTask.None)
			{

				Debugger.Debugger.Log(string.Format("Trying to unstuck, Id:{0}", Id));
				Task = AIPathTask.UnstuckPath;
		
				SetTargetPosition(Map.GetRandomPointOnRadius(currentMap, Position, this, unstackRadius));
			}

			bool goToTheTarget = GoToTheTarget();

			if (goToTheTarget)
			{
				if (Task == AIPathTask.UnstuckPath)
				{
					Task = AIPathTask.ReturnToMainPath;
					//Task = AIPathTask.None;

					if (_mainTargetPath != null) 
						SetTargetPosition(Map.GetWorldPosition(currentMap, _mainTargetPath[0], MyShip));
					//SetTargetPosition(_mainGoalPosition);

					Debugger.Debugger.Log(string.Format("Unstuck successful, go to main path, Id:{0}", Id));
					return false;
				}

				if (Task == AIPathTask.ReturnToMainPath)
				{
					Task = AIPathTask.ReturnToMainPath;

					countOfAttemps --;

					if (countOfAttemps <= 0)
					{
						if (_mainTargetPath != null)
						{
							TargetPath = new List<Point>(_mainTargetPath);

							Task = _previousPathTask;

							Debugger.Debugger.Log(
								string.Format("We are come to the main path, Id:{0}, count of th attemp: {1}, previousTask: {2}", Id,
								              countOfAttemps, Task));

							ResetUnstuckData();

							SetCurrentTargetPosition();

							_pathWasFound = true;
						}
						else
						{
							Debugger.Debugger.Log(string.Format("We are lose the way to the main path, Id:{0}", Id));
							ResetUnstuckData();
						}
					}

					return true;
				}


			}

			return false;
		}

		#endregion

		#region AI Tactic Implementation

		public enum AIPathTask
		{
			None,
			GoToRandomPoint,
			UnstuckPath,
			ReturnToMainPath,
			GoToEnemyBase,
			GoToOwnBase,
			GoToOwnDroppedFlag,
			GoToEnemyDroppedFlag,
			SupportShipWithFlag,
			TreverseDefensePoint,
			GoToOwnBaseAlarm,
			PursuitOfTheEnemyWithMyFlag,
			FollowTheEnemy,
		}

		public AIPathTask Task = AIPathTask.None;

		private bool _pathWasFound = false;

		public bool PathWasFound
		{
			get { return _pathWasFound; }
		}

		//Useful variables

		public bool IsMyFlagOnBase
		{
			get
			{
				if (Team == TeamColor.BlueTeam && BlueBase != null && BlueBase.Flag != null)
				{
					return BlueBase.Flag.State == FlagState.OnBase;
				}

				if (Team == TeamColor.OrangeTeam && OrangeBase != null && OrangeBase.Flag != null)
				{
					return OrangeBase.Flag.State == FlagState.OnBase;
				}

				return false;
			}
		}

		public bool IsEnemyFlagOnBase
		{
			get
			{
				if (Team == TeamColor.BlueTeam && OrangeBase != null && OrangeBase.Flag != null)
				{
					return OrangeBase.Flag.State == FlagState.OnBase;
				}

				if (Team == TeamColor.OrangeTeam && BlueBase != null && BlueBase.Flag != null)
				{
					return BlueBase.Flag.State == FlagState.OnBase;
				}

				return false;
			}
		}

		public Vector3 MyFlagPosition
		{
			get
			{
				if (Team == TeamColor.BlueTeam && BlueBase != null && BlueBase.Flag != null)
				{
					return BlueBase.Flag.Position;
				}

				if (Team == TeamColor.OrangeTeam && OrangeBase != null && OrangeBase.Flag != null)
				{
					return OrangeBase.Flag.Position;
				}

				return Vector3.zero;
			}
		}


		public Vector3 EnemyFlagPosition
		{
			get
			{
				if (Team == TeamColor.BlueTeam && OrangeBase != null && OrangeBase.Flag != null)
				{
					return OrangeBase.Flag.Position;
				}

				if (Team == TeamColor.OrangeTeam && BlueBase != null && BlueBase.Flag != null)
				{
					return BlueBase.Flag.Position;
				}

				return Vector3.zero;
			}
		}

		public bool IsMyFlagDropped
		{
			get
			{
				if (Team == TeamColor.BlueTeam && BlueBase != null && BlueBase.Flag != null)
				{
					return BlueBase.Flag.State == FlagState.Dropped;
				}

				if (Team == TeamColor.OrangeTeam && OrangeBase != null && OrangeBase.Flag != null)
				{
					return OrangeBase.Flag.State == FlagState.Dropped;
				}
				return false;
			}
		}

		private bool IsEnemyFlagDropped
		{
			get
			{
				if (Team == TeamColor.BlueTeam && OrangeBase != null && OrangeBase.Flag != null)
				{
					return OrangeBase.Flag.State == FlagState.Dropped;
				}

				if (Team == TeamColor.OrangeTeam && BlueBase != null && BlueBase.Flag != null)
				{
					return BlueBase.Flag.State == FlagState.Dropped;
				}
 
				return false;
			}
		}

		public Vector3 MyBasePosition
		{
			get
			{
				if (Team == TeamColor.BlueTeam && BlueBase != null)
				{
					return BlueBase.Position;
				}

				if (Team == TeamColor.OrangeTeam && OrangeBase != null)
				{
					return OrangeBase.Position;
				}

				return Vector3.zero;
			}
		}

		private Vector3 EnemyBasePosition
		{
			get
			{
				if (Team == TeamColor.BlueTeam && OrangeBase != null)
				{
					return OrangeBase.Position;
				}

				if (Team == TeamColor.OrangeTeam && BlueBase != null)
				{
					return BlueBase.Position;
				}

				return Vector3.zero;
			}
		}

		public BaseState MyBaseState
		{
			get
			{
				if (Team == TeamColor.BlueTeam && BlueBase != null)
				{
					return BlueBase.State;
				}

				if (Team == TeamColor.OrangeTeam && OrangeBase != null)
				{
					return OrangeBase.State;
				}
				return BaseState.Normal;
			}
		}

		public Vector3 AroundMyBasePoint
		{
			get
			{
				return Map.GetRandomPointOnRadius(currentMap, MyBasePosition, this);
			}
		}

		public bool IsMyShipPlacedNearMyBase
		{
			get
			{
				if (Team == TeamColor.BlueTeam && BlueBase != null)
				{
					return BlueBase.IsAiShipPlacedNearBase(this);
				}

				if (Team == TeamColor.OrangeTeam && OrangeBase != null)
				{
					return OrangeBase.IsAiShipPlacedNearBase(this);
				}
				return false;
			}
		}

		public bool IsAnyShipInMyFleetTakeEnemyFlag
		{
			get
			{
				if (AllPlayerInMyTeam != null)
				{
					return !MyShip.IsFlagTaken && AllPlayerInMyTeam.Any(player => player.MyShip.IsFlagTaken);
				}

				return false;
			}
		}

		public Vector3 ShipPositionWithFlagInMyFleet
		{
			get
			{
				Vector3 shipPositionWithFlag = Vector3.zero;

				if (AllPlayerInMyTeam != null)
				{
					Player player = AllPlayerInMyTeam.FirstOrDefault(player1 => player1.MyShip.IsFlagTaken);

					if (player != null)
					{
						shipPositionWithFlag = player.Position;
					}
				}

				return shipPositionWithFlag;
			}
		}

		//variables to base defencse tactic

		private List<Point> _baseDefensePoints;

		private Vector3 _enemyPosition;

		public Vector3 EnemyPosition
		{
			set { _enemyPosition = value; }
			get { return _enemyPosition; }
		}

		private Player _enemyPlayer;

		private FSMState<AIPlayer> _beforeUnstackTask;

		public void ReturnToTaskBeforeStuck()
		{
			if (_beforeUnstackTask != null)
			{
				FSM.ChangeState(_beforeUnstackTask);
				Debug.Log(string.Format("Player: {0}-{1} return to task before stuck", Id, Team));
			}
		}

		/// <summary>
		/// Перемещение кораблика к заданной цели, возвращает true - если цель достигнута, false - иначе
		/// </summary>
		/// <returns></returns>
		public bool GoToTheTarget()
		{
			if (IsReachingTargetPos(eps, CurrentTargetPosition))
			{
				//Debug.Log(string.Format("Player: {0} is reached target position ({1}), current target pos ({2})", Id, Position, CurrentTargetPosition));
				return MoveNextInTargetPath();
			}

			Vector3 curTargetPos = new Vector3(CurrentTargetPosition.x, 0, CurrentTargetPosition.z);
			Vector3 curPos = new Vector3(Position.x, 0, Position.z);

			var dir = (curTargetPos - curPos).normalized;

			//Debug.Log(string.Format("Player: {0} position ({1}), dir ({2})", Id, Position, dir));

			//base.OnMove(dir);

			if (MoveShipEvent != null)
			{
				MoveShipEvent(dir);
			}

			//Debug.Log(string.Format("Player: {0} new position ({1}), dir ({2})", Id, Position, dir));

			if (previousPosition.Count >= PreviousCount)
			{
				previousPosition.RemoveAt(0);
			}

			_timeToStayCounter += Time.deltaTime;

			if (IsMovementOnCircle())
			{
				ChangeState(FSM.Current);
			}

			previousPosition.Add(Position);

			if (IsPlayerIsInCollision && IsPlayerStuck())
			{
				ShootBasic();
				IsPlayerIsInCollision = false;
				previousPosition.Clear();

				if (FSM.Current != UnstuckTask.Instance)
				{
					_beforeUnstackTask = FSM.Current;
				}
				ChangeState(UnstuckTask.Instance);
				//UnstuckTask();
				return false;
			}

			if (IsReachingTargetPos(eps, CurrentTargetPosition))
			{
				return MoveNextInTargetPath();
			}

			//Debug.Log(string.Format("Player ID {0}, {1}, target was not reached", Id, CurrentTargetPosition));
			return false;
		}

		public delegate void MoveShipDelegate(Vector3 direction);

		public event MoveShipDelegate MoveShipEvent;

		// to find and fix circle movement

		private float _timeToStay = 3.5f;

		private float _timeToStayCounter;

		private bool IsMovementOnCircle()
		{
			if ( myShip.Type == ShipType.BigShip)
			{
				_timeToStay = 5;
			}

			if (_timeToStayCounter >= _timeToStay)
			{
				ShootBasic();
				_timeToStayCounter = 0;
				Debug.Log(string.Format( "AiPlayer.GoToTheTarget: was detected that player {0} is movement on circle", Id));
				return true;
			}
			return false;
		}

		private bool MoveNextInTargetPath()
		{
			bool isPathTraversed = false;

			_timeToStayCounter = 0;

			if (Mechanics == MechanicsType.Classic)
			{
				_currentWaypoint++;
				if (_currentWaypoint < TargetPath.Count )
				{
					SetCurrentTargetPosition();
				}
				else
				{
					TargetPath.Clear();
					isPathTraversed = true;
					_pathWasFound = false;
				}
			}
			else
			{

				//Debug.Log("AiPlayer.MoveNextInTargetPath: mechanic is NewWave");

				_currentWaypoint++;

				if (_currentWaypoint < TargetPathVector.Count)
				{	
					SetCurrentTargetPosition();
				}
				else
				{
					TargetPathVector.Clear();

					isPathTraversed = true;
					_pathWasFound = false;
				}
			}
			return isPathTraversed;
		}

		/// <summary>
		/// Set new target
		/// </summary>
		/// <param name="targetPosition">position of new target</param>
		public void SetTargetPosition(Vector3 targetPosition, string message = "")
		{
			PathfindState = PathfinderState.SetTarget;
			if (message != string.Empty)
			{
				//Debug.Log(message);
			}

			_pathWasFound = false;

			Point mapPoint = Map.GetMapPosition(currentMap, targetPosition);
			if (!Map.IsPointAvailableToNavigate(currentMap[mapPoint.X, mapPoint.Y].Type))
			{
				mapPoint = Map.GetRandomPointOnRadius(currentMap, mapPoint, this, 1);
				targetPosition = Map.GetWorldPosition(currentMap, mapPoint, MyShip);
			}

			GoalPosition = targetPosition;

			if (findPathHandler != null)
			{
				findPathHandler(Position, GoalPosition, OnPathFind);
				PathfindState = PathfinderState.Calculating;
			}
			else
			{
				PathfindState = PathfinderState.ReturnPathError;
			}
		}

		private void SetCurrentTargetPosition()
		{
			if (Mechanics == MechanicsType.Classic)
			{
				if (TargetPath != null && TargetPath.Count > 0)
					CurrentTargetPosition = Map.GetWorldPosition(currentMap, TargetPath[_currentWaypoint], MyShip);
			}

			else
			{
				if (TargetPathVector != null && TargetPathVector.Count > 0)
				{
					CurrentTargetPosition = TargetPathVector[_currentWaypoint];
				}
			}
		}

		public void ShootIfNeed()
		{
			// If enemy ahead
			if (IsShipInDirection(Forward))
			{
				ShootBasic() ;
			}

            if (IsShipInDirection(Backward, SearchDistance * 0.5f))
            {
                // Place bomb
                PlaceBomb();
            }
		}

		private const float SearchDistance = 20f;

		public bool IsShipInDirection(Vector3 direction, float distance = SearchDistance)
		{
			Vector3 shiftPos = Vector3.zero;

			if (myShip.Type == ShipType.BigShip)
			{
				shiftPos = Left*currentMap.CellSize*0.5f;
				
			Debug.DrawLine(Position + shiftPos, Position + shiftPos + Forward*SearchDistance,
				               Color.yellow);

				Debug.DrawLine(Position - shiftPos, Position - shiftPos + Forward*SearchDistance,
				               Color.yellow);
			}

			RaycastHit[] hits = Physics.RaycastAll(Position + shiftPos, direction, SearchDistance);

			//Debug.Log(string.Format("raycastHits count: {0}", hits.Length));

			bool result = IsShipInRaycastHit(hits);

			if (result)
				return true;

			if (MyShip.Type == ShipType.BigShip)
			{
				hits = Physics.RaycastAll(Position - shiftPos, direction, SearchDistance);

				result = IsShipInRaycastHit(hits);
			}

			if (!result)
				_enemyPosition = default (Vector3);

			return result;
		}

		private bool IsShipInRaycastHit(RaycastHit[] hits)
		{
			foreach (RaycastHit raycastHit in hits)
			{
				if (raycastHit.transform.CompareTag("Ship"))
				{
					_enemyPosition = raycastHit.transform.position;
					if (getIPositionableObject != null)
					{
						IPositionable positionableObject = getIPositionableObject (_enemyPosition);

						if (positionableObject != null)
						{
							Player player = positionableObject as Player;
							if (player != null)
							{
								if (player.Team != Team && player.MyShip.StateOfShipBehaviour == Ship.Ship.ShipBehaviourState.Normal)
									return true;
							}
						}
					}
					//return true;
				}
			}
			return false;
		}

		private void ShootBasic()
		{
			MyShip.OnFireBasic();
		}

        private void PlaceBomb()
        {
            MyShip.OnFireBomb();
        }

		private int _traversePointCounter;

		private bool _baseDefensePathInit;

		private void InitBaseDefensePath()
		{
			//state = AIStates.Walk;

			Debug.Log(string.Format("AiPlayer.InitBaseDefensePath - OK, player:{0}-{1}", Id, Team));

			Point basePoint = Map.GetMapPosition(currentMap, MyBasePosition);
			
			_baseDefensePoints = Map.GetEdgePointOnRadius(currentMap, basePoint, 5);

			_pathWasFound = false;

			_traversePointCounter = 0;

			_baseDefensePathInit = true;
		}

		public void SetNextTreversePoint()
		{
			if (!_baseDefensePathInit)
				InitBaseDefensePath();

			Vector3 currentTreversePosition = Map.GetWorldPosition(currentMap, _baseDefensePoints[_traversePointCounter++], MyShip);

			Vector3 position = Map.GetRandomPointOnRadius(currentMap, currentTreversePosition, this, 2);

			if (_traversePointCounter >= _baseDefensePoints.Count)
			{
				_traversePointCounter = 0;
			}

			Task = AIPathTask.TreverseDefensePoint;

			SetTargetPosition(position, string.Format("ID: {0}, Set base defense target position", Id));

		}

		public void RestartMainGoal()
		{
			FSM.RevertToMainState();
		}

		#region Path helper

		public enum PathfinderState
		{
			None,
			SetTarget,
			Calculating,
			ReturnPathOk,
			ReturnPathError
		}

		public PathfinderState PathfindState = PathfinderState.None;

		public void PathSet(List<Point> newPath, int playerId, AIPathTask task)
		{
			if (Id != playerId)
			{
				//Debug.Log(string.Format("This is not our path: my id:{0}, path id: {1}", Id, playerId));
				return;
			}

			//Debug.Log(string.Format("Path was found for id:{0}", Id));

			PathfindState = PathfinderState.ReturnPathOk;

			if (newPath == null)
			{
				Task = AIPathTask.None;
				_pathWasFound = false;

				TargetPath.Clear();

				TargetPath = null;
				return;
			}


			_pathWasFound = true;

			TargetPath = newPath;

			if (TargetPath.Count > 1)
				TargetPath.RemoveAt(0);

			SetCurrentTargetPosition();
		}


		//Trying fix error if the ship is stuck and cann't find another way 

		private int _stuckCount = 5;
		private int _stuckCounter = 0;

		public void OnPathFind(List<Vector3> wayPoints)
		{
			if (wayPoints == null)
				return;

			PathfindState = PathfinderState.ReturnPathOk;
			
			if (wayPoints.Count == 0)
			{
				_stuckCounter++;

				if (_stuckCounter == _stuckCount)
				{
					_stuckCounter = 0;
					TryingFindAnotherStartPoint();
				}

				_pathWasFound = true;
				//Debug.Log(string.Format("AiPlayer.OnPathFind, id: {0}, way point count: {1}", Id, wayPoints.Count));
				return;
			}

			_currentWaypoint = 0;

			_stuckCounter = 0;

			//wayPoints.RemoveAt(0);

			TargetPathVector = wayPoints;

			if (TargetPath == null)
			{
				TargetPath = new List<Point>();
			}

			TargetPath.Clear();

			foreach (Vector3 wayPoint in wayPoints)
			{
				Point point = Map.GetMapPosition(currentMap, wayPoint);
				//Map.CorrectPoint(currentMap, ref point, this, true);
				TargetPath.Add(point);
			}


			_pathWasFound = true;

			SetCurrentTargetPosition();

			//Debug.Log(string.Format("AiPlayer, id: {0}, way point count: {1}", Id, wayPoints.Count));
		}

		private void TryingFindAnotherStartPoint()
		{
			Vector3 shipPos = Position;
			Point mapPoint = Map.GetMapPosition(currentMap, shipPos);
			if (!Map.IsPointAvailableToNavigate(currentMap[mapPoint.X, mapPoint.Y].Type))
			{
				mapPoint = Map.GetRandomPointOnRadius(currentMap, mapPoint, this, 3);
				shipPos = Map.GetWorldPosition(currentMap, mapPoint, MyShip);
			}

			Position = shipPos;

			Debug.Log(string.Format("AiPlayer.TryingFindAnotherStartPoint, id: {0}-{1}", Id, Team ));
		}

		#endregion

		#endregion

		#region Detection of The Enemy

		private Point _startFollowPoint;

		private int _currentDistanceToFollowFromStart; // in cells

		public int MaxFollowDistance = 5; // in cells
		public int MaxRadiusLocation = 2;

		private Player _enemy;
		public Player EnemyPlayer { get { return _enemy; }}

		public int CurrentDistanceFollow
		{
			get
			{
				CalculateCurrentDistance();
				return _currentDistanceToFollowFromStart;
			}
		}

		private void CalculateCurrentDistance()
		{
			Point currentPoint = Map.GetMapPosition(currentMap, Position);

			int x = Mathf.Abs(currentPoint.X - _startFollowPoint.X);
			int y = Mathf.Abs(currentPoint.Y - _startFollowPoint.Y);

			_currentDistanceToFollowFromStart = Mathf.RoundToInt(Mathf.Sqrt(x*x + y*y));
		}

		//

		public void SetStartFollowPoint(Vector3 currentPosition)
		{
			_startFollowPoint = Map.GetMapPosition(currentMap, currentPosition);
		}

		public bool IsThereEnemyInRadius(int radius)
		{
			_enemy = null;
			EnemyPosition = default (Vector3);

			List<IPositionable> ships = GetIPositionableList();

			if (ships != null)
			{
				Point currentPoint = Map.GetMapPosition(currentMap, Position);

				Point point = new Point(currentPoint.X - radius, currentPoint.Y - radius);
				Map.CorrectPoint(currentMap, ref point, this);

				Vector3 minPosition = Map.GetWorldPosition(currentMap, point);

				point = new Point(currentPoint.X + radius, currentPoint.Y + radius);
				Map.CorrectPoint(currentMap, ref point, this);

				Vector3 maxPosition = Map.GetWorldPosition(currentMap, point);

				float minimalDistance = float.MaxValue;

				foreach (IPositionable pos in ships)
				{
					if (!(pos is Player))
						continue;

					if ((pos as Player).Team == Team)
						continue;

					if ((pos as Player).MyShip.StateOfShipBehaviour != Ship.Ship.ShipBehaviourState.Normal)
						continue;

					if (pos.Position.x >= minPosition.x && pos.Position.z >= minPosition.z && pos.Position.x <= maxPosition.x &&
					    pos.Position.z <= maxPosition.z)
					{
						float distance = (Position - pos.Position).sqrMagnitude;

						if (distance < minimalDistance)
						{
							_enemy = pos as Player;
							minimalDistance = distance;
						}
					}
				}

				if (_enemy != null)
				{
					EnemyPosition = _enemy.Position;

					Debug.Log(string.Format("Player {0}-{1} Enemy was found: {2}-{3}", Id, Team, _enemy.Id, _enemy.Team));
					return true;
				}
			}

			return false;
		}

		#endregion
	}

}
