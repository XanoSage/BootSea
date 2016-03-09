using System;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Extensions;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Battles;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Ship;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Players
{
	public abstract class Player : IInitable, ISerializable, IPositionable
	{

		#region constants

		public const int PreviousCount = 20;

		#endregion


		#region Variables



        public int Statistic_shoot;
        public int Statistic_shootAdvance;
        public int Statistic_death;
        public int Statistic_flag;
        public int Statistic_kills;
        public int Statistic_mines;

		public static MechanicsType Mechanics;

		public int Id = 0;

		public PlayerType Type;


		/// <summary>
		/// Цвет команды за который играет игрок
		/// </summary>
		public TeamColor Team;

		/// <summary>
		/// Является ли игрок капитаном
		/// </summary>
		public bool IsCaptain { get; protected set; }


        // Параметры доп.Оружия
        public WeaponsType AdvanceWeapon;
        public int AdvanceWeaponNumber;

        // Параметры Доп Модификацый
        public UpgradesType[] upgrades;


		/// <summary>
		/// Ссылка на корабль которым управляет игрок
		/// </summary>
		protected Ship.Ship myShip;

		public Ship.Ship MyShip
		{
			get { return myShip; }
			set { myShip = value; }
		}

		/// <summary>
		/// Список кораблей под одним флотом
		/// </summary>
		public List<Ship.Ship> Fleet;

		public List<Player> PlayersFleet;

		public List<Player> AllPlayerInMyTeam;

		protected Map currentMap;

		public Battle CurrentBattle { get; protected set; }

		public float CameraFOVFrom
		{
			get { return MyShip.CameraFOVFrom; }
		}

		public float CameraFOVTo
		{
			get { return MyShip.CameraFOVTo; }
		}

		#region Movement

		public bool IsMoving;

		public Action OnMovementStateChanged;

		private bool _lastIsInMovement;
		private bool _isInMovementFree;

		public void SetIsInMovementFree(bool isIsMovement)
		{
			if (Mechanics == MechanicsType.NewWave)
			{
				_isInMovementFree = isIsMovement;
			}
		}

		// Находится ли корабль в движении?
		public bool IsInMovement
		{
			get
			{
				if (Mechanics == MechanicsType.NewWave)
					return _isInMovementFree; //_lastIsInMovement;

				// Определяем, двигается/поворачивается ли корабль
				bool result = IsMoving;
				if (Mechanics == MechanicsType.Classic && !IsRotated)
					result = false;

				// Определяем, изменилось ли состояние с предыдущего вызова
				if (result != _lastIsInMovement && OnMovementStateChanged != null)
					OnMovementStateChanged();

				// Сохраняем последнее состояние
				_lastIsInMovement = result;
				return result;
			}

			set
			{
				if (Mechanics == MechanicsType.NewWave)
					_lastIsInMovement = value;
			}
		}

		public Quaternion Rotation, RotationFrom, RotationTo;

		public bool IsRotated
		{
			get
			{
				if (Mechanics == MechanicsType.NewWave)
					return true;

				if (Direction == Vector3.zero)
					return true;

				return RotationEquals(Rotation, RotationTo);
			}
		}

		public bool IsPlayerIsInCollision;

		private bool RotationEquals(Quaternion r1, Quaternion r2)
		{
			float abs = Mathf.Abs(Quaternion.Dot(r1, r2));
			if (abs >= 0.999f)
				return true;

			return false;
		}

		// Считается и задаётся автоматом в старте джойстика (нужно для правильного рассчёта нанотехнологического перемещения)
		public static float VisibleMagnitude;

		public Vector3 Direction
		{
			get { return MyShip == null ? Vector3.forward : MyShip.DirectionNW; }
			set
			{
				if (MyShip != null)
					MyShip.DirectionNW = value;
				else
					Debug.LogError("My ship is null");
			}
		}

		public Vector3 Forward;

		public Vector3 Backward
		{
			get { return Forward.RotateY(Mathf.PI); }
		}

		public Vector3 Right
		{
			get { return Forward.RotateY(Mathf.PI*0.5f); }
		}

		public Vector3 Left
		{
			get { return Forward.RotateY(-(Mathf.PI*0.5f)); }
		}


		public float Acceleration;

		public Vector3 Position
		{
			get { return MyShip != null ? MyShip.Position : new Vector3(0, 0, 0); }
			set
			{
				if (MyShip != null)
					MyShip.Position = value;
				else
					Debug.LogError("My ship is null");
			}
		}

		// Временные магические числа
		public float eps = 0.5f, //0.25f
		             coeffSpeed = 1.75f,
		             coeffRotationSpeed = 1.5f;

		//need to correct enter to FSM for ai
		public bool IsEventWasInitiated;

		#endregion

		#endregion

		#region Constructor

		protected Player(bool isCaptain = false, TeamColor team = TeamColor.BlueTeam)
		{
			Fleet = new List<Ship.Ship>();
			Team = team;
			IsCaptain = isCaptain;
			CurrentBattle = null;
			currentMap = null;
			IsPlayerIsInCollision = false;

          	}


     

		public static Player CreatePlayer(PlayerType type, bool isCaptain, TeamColor team, bool isLocal = true)
		{
			Player player = null;

			switch (type)
			{
				case PlayerType.HumanPlayer:
					player = new HumanPlayer(team);
					break;
				case PlayerType.AIPlayer:
					player = new AIPlayer(isCaptain, team, isLocal);
					break;
				case PlayerType.MultiplayerPlayer:
					//player = new MultiplayerPlayer(team);
					break;
			}
			return player;
		}

		public static Player CreatePlayer(PlayerType type, TeamColor team = TeamColor.BlueTeam, bool isLocal = true)
		{
			return CreatePlayer(type, false, team, isLocal);
		}

		#endregion

		#region Events

		public delegate Point GetRandomSpawnPointHandler(TeamColor team);

		private static GetRandomSpawnPointHandler getRandomSpawnPointHandler;

		public static event GetRandomSpawnPointHandler GetRandomSpawnPointEvent
		{
			add { getRandomSpawnPointHandler += value; }
			remove { getRandomSpawnPointHandler -= value; }
		}


		public delegate IPositionable GetIPositionableObjectHandler(Vector3 position);

		protected static GetIPositionableObjectHandler getIPositionableObject;

		public static event GetIPositionableObjectHandler GetIPositionableObjectEvent
		{
			add { getIPositionableObject += value; }
			remove { getIPositionableObject -= value; }
		}

		public delegate List<IPositionable> GetIPositionableListHandler();

		protected static GetIPositionableListHandler GetIPositionableList;

		public static event GetIPositionableListHandler GetIPositionableListEvent
		{
			add { GetIPositionableList += value; }
			remove { GetIPositionableList -= value; }
		}

		public void SetCaptain(bool isCaptain = true)
		{
			IsCaptain = isCaptain;
		}

		public void SetMyShip()
		{
			myShip = Fleet.FirstOrDefault(ship => ship.Owner == this);
		}

		// Пока обработчик ивентов делаю как void функцию
		public virtual void OnFireBasic()
		{

		}

		public virtual void OnFireBomb()
		{

		}

		public virtual void OnFireSpecial()
		{

		}

		/// <summary>
		/// Cобытия и действия происходящие в случае, если корабль затонул (нас подбили)
		/// </summary>
		public virtual void OnShipSunked()
		{
			Debug.Log("OnShipSunked() - parent");
			Acceleration = 0f;
		}

		/// <summary>
		/// События и действия происходящие в случае, еслы мы подбили корабль
		/// </summary>
		public virtual void OnShipDestroyed()
		{
			Debug.Log("OnShipDestroyed() - parent");
		}

		private int spawnPointRadius = 3;

		protected virtual void OnShipRespawn(bool invulnerability = false)
		{
			//Debug.Log("OnShipRespawn() - parent");
			Point spawnPoint = GetRandomSpawnPoint();

			while (IsSpawnPointNotFreeFromShip(spawnPoint) || !Map.IsPointAccessableToNavigate(currentMap, spawnPoint, this))
			{
				spawnPoint = Map.GetRandomPointOnRadius(currentMap, spawnPoint, this, spawnPointRadius);
			}

			Position = Map.GetWorldPosition(currentMap, spawnPoint, MyShip);
		}

		private bool IsSpawnPointNotFreeFromShip(Point point)
		{

			List<IPositionable> iPositionables;
			if (GetIPositionableList == null)
				return false;

			iPositionables = GetIPositionableList();

			//Debugger.Debugger.Log(string.Format("Check for playerId: {0}, is spawn point free...", Id));

			for (int i = 0; i < iPositionables.Count; i++)
			{
				if (!(iPositionables[i] is Player))
					continue;

				Point shipPoint = Map.GetMapPosition(currentMap, (iPositionables[i] as Player).Position);

				if (point == shipPoint)
				{
					//Debugger.Debugger.Log(string.Format("PlayerId: {0}, Spawn point is not free...", Id));
					return true;
				}
			}

			return false;
		}

		private float _correctSpeedCoeff = 1f;

		private float defaultVisibleMagnitude = 0.25f;

		public virtual void OnMove(Vector3 direction)
		{

			Direction = direction;

			if (Mechanics == MechanicsType.NewWave)
			{

				if (Math.Abs(VisibleMagnitude - 0) < 0.1)
				{
					VisibleMagnitude = defaultVisibleMagnitude;
				}

				_isInMovementFree = true;
				Acceleration = Mathf.Lerp(0,
				                          MyShip.MaxSpeed*coeffSpeed*Time.deltaTime,
				                          Mathf.Lerp(0, 1, Mathf.InverseLerp(0, VisibleMagnitude, direction.magnitude)));
			}

			Vector3 newPosition = Mechanics == MechanicsType.Classic
				                      ? Position + Direction*MyShip.MaxSpeed*coeffSpeed*Time.deltaTime
				                      : Position + Forward*Acceleration;


			if (!IsCanMove(newPosition) || IsColliderEnter(newPosition))
			{
				IsInMovement = false;
				return;
			}

			//Debug.Log(string.Format("Id:{0}, My position is old:{1} - new:{2}, dir:{3}, Acceleration:{4}, visibleMagn: {5}, dir.magn: {6}", Id, Position,
			//						newPosition, direction, Acceleration, VisibleMagnitude, direction.magnitude));

			Position = newPosition;

			IsInMovement = true;
		}

		public bool IsCanMove(Vector3 position)
		{
			float coeffCellSize = 0.60f;
			if ((position.x <= Map.TopLeftBorder.x - currentMap.CellSize*coeffCellSize
			     || position.z <= Map.TopLeftBorder.z - currentMap.CellSize*coeffCellSize
			     || position.x >= Map.BottomRightBorder.x + currentMap.CellSize*coeffCellSize
			     || position.z >= Map.BottomRightBorder.z + currentMap.CellSize*coeffCellSize))
			{
				return false;
			}

			if (Mechanics == MechanicsType.Classic && !IsRotated)
				return false;

			return true;
		}


		private float colliderCoeff = 0.65f;

		public bool IsColliderEnter(Vector3 position)
		{
			colliderCoeff = this is HumanPlayer ? 0.45f : 0.25f;

			RaycastHit[] hits = Physics.RaycastAll(position, Direction, currentMap.CellSize * MyShip.Size * colliderCoeff);

			//Debug.DrawLine(Position, Position + Direction*currentMap.CellSize*MyShip.Size * colliderCoeff, Color.yellow);

			foreach (RaycastHit raycastHit in hits)
			{
				//if (this is HumanPlayer)
				//{
				//	Debug.Log(string.Format("Checking collider: {0}, {1}", raycastHit.transform.tag, raycastHit.transform.name));
				//}

				if (raycastHit.transform.CompareTag("Untouchable") || raycastHit.transform.CompareTag("Ship") && !(this is HumanPlayer))
				{
					return false;
				}

				if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Mine"))
				{
					return false;
				}

			}

			if (hits.Length > 0)
				return true;

			return false;
		}

		/// <summary>
		/// Смена цвета команды
		/// </summary>
		/// <param name="newColor"></param>
		public virtual void OnChangeTeamColor(TeamColor newColor)
		{
			foreach (Player player in PlayersFleet)
			{
				player.Team = newColor;
			}
		}

		public void SetPlayerId(int idCaptain)
		{
			if (!IsCaptain)
				return;

			foreach (Player player in PlayersFleet)
			{
				player.Id = idCaptain++;
			}
		}

		public static void SetMapBorder(Vector3 topLeftBorder, Vector3 bottomRightBorder)
		{
			Map.TopLeftBorder = topLeftBorder;
			Map.BottomRightBorder = bottomRightBorder;
		}

		public void SetMap(Map map)
		{
			currentMap = map;
		}

		public void SetActiveBattle(Battle battle)
		{
			CurrentBattle = battle;
		}


		private void SetEventHandler()
		{
			MyShip.OnShipDestroyedEvent += OnShipDestroyed;
			MyShip.OnShipSunkEvent += OnShipSunked;
			MyShip.OnRespawnEvent += OnShipRespawn;
		}

		protected Point GetRandomSpawnPoint()
		{
			Point point = new Point(0, 0);

			if (getRandomSpawnPointHandler != null)
			{
				point = getRandomSpawnPointHandler(Team);
			}

			return point;
		}

		protected bool IsReachingTargetPos(float eps, Vector3 targetPosition)
		{
			eps = Mechanics == MechanicsType.Classic ? eps*1.25f : eps*2.75f;

			Vector2 pos = new Vector2(Position.x, Position.z);
			Vector2 targetPos = new Vector2(targetPosition.x, targetPosition.z);

			float distance = Vector2.Distance(pos, targetPos);

			//Debug.Log(string.Format("Position {0}, Target:{1}, distance: {2}, eps: {3}", Position, targetPosition, distance, eps));
			//return Position.x <= targetPosition.x + eps && Position.x >= targetPosition.x - eps &&
			//	   Position.z <= targetPosition.z + eps && Position.z >= targetPosition.z - eps;

			return distance <= eps;
		}

		#endregion

		#region Inerface implementation

		public virtual void Init()
		{
			SetEventHandler();
		}

		public virtual void Serialize(DataBuffer buffer)
		{
			buffer.Write((int) Type);
			buffer.Write((int) Team);
			buffer.Write(IsCaptain);
		}

		public virtual void Deserialize(DataBuffer buffer)
		{
			Type = (PlayerType) buffer.ReadInt();
			Team = (TeamColor) buffer.ReadInt();
			IsCaptain = buffer.ReadBool();
		}

		#endregion
	}
}
