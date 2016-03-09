using System.Collections.Generic;
using LinqTools;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Maps;
using Aratog.NavyFight.Models.Unity3D.Base;
using Aratog.NavyFight.Models.Unity3D.Battles;
using Aratog.NavyFight.Models.Unity3D.Flags;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;
using System;

public class BattleController : MonoBehaviour
{

	#region Constants

	private const string YourMsg = "Your";
	private const string EnemyMsg = "Enemy";

	private const string FlagStolenMsg = "{0} flag is stolen";
	private const string FlagDeliveredMsg = "{0} flag is delivered";
	private const string FlagDroppedMsg = "{0} flag is dropped";
	private const string FlagReturnedMsg = "{0} flag is returned";

	#endregion

	#region Variables

	private int flagCountPerDeliver = 1;

	public static BattleController Instance { get; private set; }

	public Battle ActiveBattle { get; private set; }


	/// <summary>
	/// Все точки спавна
	/// </summary>
	[HideInInspector] public List<Point> SpawnPoints;

	/// <summary>
	/// Точки спавна для оранжевой команды
	/// </summary>
	[HideInInspector] public List<Point> OrangeSpawnPoints;

	/// <summary>
	/// Точки спавна для синей команды
	/// </summary>
	[HideInInspector] public List<Point> BlueSpawnPoints;

	/// <summary>
	/// Место локации базы синей команды
	/// </summary>
	[HideInInspector] public Point BlueBasePoint;

	/// <summary>
	/// Место локации базы оранжевой команды
	/// </summary>
	[HideInInspector] public Point OrangeBasePoint;


	[HideInInspector] public FlagSpotBehaviour BlueFlagSpot;

	[HideInInspector] public FlagSpotBehaviour OrangeFlagSpot;

	[HideInInspector] public FlagsBehaviour BlueFlag;

	[HideInInspector] public FlagsBehaviour OrangeFlag;

	 public List<ShipBehaviour> ships;

	[HideInInspector] public List<IPositionable> PositionablesObject;

	[HideInInspector] public List<SpawnPointBehaviour> SpawnPointsBehaviours;

	//for battle 
	public int BlueFlagCounter { get; private set; }
	public int OrangeFlagCounter { get; private set; }

	public int BlueFragCounter { get; private set; }
	public int OrangeFragCounter { get; private set; }


	public int BlueSurvivlWins;
	public int RedSurvivlWins;

	public TeamColor CurrFlagCollor = TeamColor.BlueTeam;

	public bool IsWin = false;

	public float TimeSpentCounter
	{
		get { return Time.time - timeStartBattle; }
	}

	private float timeStartBattle;

	#endregion

	#region MonoBehaviour events

	private void Awake()
	{
		Instance = this;
		ships = new List<ShipBehaviour>();
		SpawnPointsBehaviours = new List<SpawnPointBehaviour>();

		PositionablesObject = new List<IPositionable>();
	}

	private void Start()
	{
		//GameController.Instance.OnStartBattle += OnBattleStart;

		BlueFlagSpot = null;
		OrangeFlagSpot = null;

		BlueFlag = null;
		OrangeFlag = null;

		someInt = UnityEngine.Random.Range(1, 6);

		Player.GetRandomSpawnPointEvent += GetRandomSpawnPoint;
		Player.GetIPositionableObjectEvent += GetIPositionableObject;
		Player.GetIPositionableListEvent += GetIPositionableList;

	}

	private void Update()
	{
		if (!GameSetObserver.Instance.IsBattleStarted || GameSetObserver.Instance.IsPause)
			return;

		ActiveBattle.TimeSpent = Time.time - timeStartBattle;
	}

	private int someInt = -1;

	#endregion

	#region Events

	#region Common events

	public float correctGeodataX = 5f;
	public float correctGeodataZ = 5f;

	/// <summary>
	/// Задаем переменные BattleConttrller'а
	/// </summary>
	public void OnBattleStart()
	{
		ResetBattleData();

		ActiveBattle = GameSetObserver.Instance.CurrentBattle;

		if (ActiveBattle == null)
		{
			Debug.LogError("BattleCntroller.OnBattleStart: ActiveBattle is null");
			return;
		}

		Map map = ActiveBattle.Map;

		Map.TopLeftBorder = new Vector3(- map.FieldWidth*0.5f*map.CellSize - correctGeodataX, 0, -map.FieldHeight*0.5f*map.CellSize - correctGeodataZ);
		Map.BottomRightBorder = new Vector3(map.FieldWidth*0.5f*map.CellSize - correctGeodataX, 0, map.FieldHeight*0.5f*map.CellSize - correctGeodataZ);

		map.InitWaypoints();

		map.InitAccessWayPoints();

		SetSpawnPoint(ActiveBattle.Map);

		timeStartBattle = Time.time;

		
		SetScoreLabelData();

		SetBaseDataOnStartBattle();

		//if (ActiveBattle.Mode == GameMode.CaptureTheFlag) {
						InitCaptureTheFlagGame ();
			//	}

		SetShipPositionOnStartBattle();

		if (GameController.Instance != null)
		{
			UpdateBombCount(GameController.Instance.Human);
			UpdateSpecialCount(GameController.Instance.Human);
		}

		Destructable.OnBuildingDestructionEvent += OnBuildingDestruction;
	}

	public void OnEndBattle(GameMode mode, bool fromServer, TeamColor winner)
	{
		if (mode == GameMode.CaptureTheFlag)
		{
			if (UIController.Instance != null && Toasts.Instance != null)
			{
				//TODO:: show Capture The Flag Battle win interface

				Toasts.Instance.Add(string.Format("{0} is winner", winner));
				if (winner == TeamColor.BlueTeam) {

					if (GameController.Instance.IsAdmiralComplet == true) {
						PlayerInfo.Instance.AdmiralQuestComplet = true;
					}
				}
				ActiveBattle.TimeSpent = Time.time - timeStartBattle;

				int minutes = (int) ActiveBattle.TimeSpent/60;
				int seconds = (int) ActiveBattle.TimeSpent%60;

				UIController.Instance.timeSpentForBattle = string.Format("Time spent {0}:{1}", minutes, seconds);
				UIController.Instance.SetupDialogData();

				Toasts.Instance.Add(UIController.Instance.timeSpentForBattle);
			}
		}
		if (winner == TeamColor.BlueTeam) {
			IsWin = true;
		}
		if (!fromServer)
		{
			Debug.Log(string.Format("Send End of Battle to player"));
			if (MultiplayerManager.Instance != null)
				MultiplayerManager.Instance.NeedEndBattle(mode, winner);
		}


		if (GameController.Instance != null)
			GameController.Instance.BattleEnd();

		if (fromServer)
		{
			Debug.Log(string.Format("recieve End of Battle to player"));
			return;
		}
		
	}


	public void OnEndBattle(TeamColor winner)
	{

		Toasts.Instance.Add(string.Format("{0} is winner", winner));

		if (winner == TeamColor.BlueTeam) {
			IsWin = true;
			if (GameController.Instance.IsAdmiralComplet == true) {
				
				PlayerInfo.Instance.AdmiralQuestComplet = true;
				Debug.Log("ADMIRAL COMPLET");
			}
		}

		if (GameController.Instance != null)
			GameController.Instance.BattleEnd();
		

		
	}
	private void InitCaptureTheFlagGame()
	{

		BlueFlagSpot = ResourceBehaviourController.Instance.InitFlagSpot(ActiveBattle.Map.Environment, TeamColor.BlueTeam,
		                                                                 ActiveBattle.BlueBase);
		OrangeFlagSpot = ResourceBehaviourController.Instance.InitFlagSpot(ActiveBattle.Map.Environment, TeamColor.OrangeTeam,
		                                                                   ActiveBattle.OrangeBase);

		BlueFlagSpot.Flag =
			BlueFlag =
			ResourceBehaviourController.Instance.InitFlags(ActiveBattle.Map.Environment, TeamColor.BlueTeam,
			                                               ActiveBattle.BlueFlag);
		OrangeFlagSpot.Flag =
			OrangeFlag =
			ResourceBehaviourController.Instance.InitFlags(ActiveBattle.Map.Environment, TeamColor.OrangeTeam,
			                                               ActiveBattle.OrangeFlag);

		BlueFlag.Base = BlueFlagSpot;

		OrangeFlag.Base = OrangeFlagSpot;


		if(ActiveBattle.Mode== GameMode.Survival)
		{
			OrangeFlag.gameObject.SetActive(false);

			BlueFlag.gameObject.SetActive(true);
		}

	}

	/// <summary>
	/// Сброс параметров сражения
	/// </summary>
	public void ResetBattleData(bool needRemove = false)
	{

		timeStartBattle = 0;

		BlueFlagCounter = 0;
		BlueFragCounter = 0;


		OrangeFlagCounter = 0;
		OrangeFragCounter = 0;

		if (needRemove)
		{
			Player.GetRandomSpawnPointEvent -= GetRandomSpawnPoint;
			Player.GetIPositionableObjectEvent -= GetIPositionableObject;
			Player.GetIPositionableListEvent -= GetIPositionableList;
		}

		if (ActiveBattle != null)
			ActiveBattle.ClearData();

		BlueBasePoint = null;

		if (BlueFlag != null)
			Destroy(BlueFlag.gameObject);

		BlueFlag = null;

		if (BlueFlagSpot != null)
			Destroy(BlueFlagSpot.gameObject);

		BlueFlagSpot = null;

		BlueSpawnPoints = new List<Point>();

		OrangeBasePoint = null;

		if (OrangeFlag != null)
			Destroy(OrangeFlag.gameObject);

		OrangeFlag = null;

		if (OrangeFlagSpot != null)
			Destroy(OrangeFlagSpot.gameObject);

		OrangeFlagSpot = null;

		OrangeSpawnPoints = new List<Point>();

		if (ships != null && needRemove)
		{
			for (int i = 0; i != ships.Count; i++)
			{
				ShipBehaviour ship = ships[i];
				PositionablesObject.RemoveAt(i);
				ship.Reset();
				ships.Remove(ship);
				if (ship != null)
				{
					Destroy(ship.gameObject);
				}
				i--;
			}

			ships = new List<ShipBehaviour>();
			
			PositionablesObject = new List<IPositionable>();

		}

		if (SpawnPointsBehaviours != null && needRemove)
		{
			for (int i = 0; i != SpawnPointsBehaviours.Count; i++)
			{
				SpawnPointBehaviour point = SpawnPointsBehaviours[i];
				SpawnPointsBehaviours.Remove(point);
				Destroy(point.gameObject);
				i--;
			}

			SpawnPointsBehaviours = new List<SpawnPointBehaviour>();
		}

		Destructable.OnBuildingDestructionEvent -= OnBuildingDestruction;

		RemoveAndDestroyAllShell();

	}


	private void OnBuildingDestruction(Vector3 position)
	{
		Map.OnDestruction(ActiveBattle.Map, position);
	}



	public void SurvivalRaundEnd(TeamColor color)
	{
		Toasts.Instance.Add(string.Format("{0} is round win", color));

		if (color == TeamColor.BlueTeam) {
			BlueSurvivlWins++;

			if(BlueSurvivlWins>2)
			{
				OnEndBattle(color);
			}
			OrangeFlag.gameObject.SetActive(true);
			BlueFlag.gameObject.SetActive(false);

			CurrFlagCollor = TeamColor.OrangeTeam;
		} else {
			RedSurvivlWins++;

			if(RedSurvivlWins>2)
			{
				OnEndBattle(color);
			}
			OrangeFlag.gameObject.SetActive(false);

			BlueFlag.gameObject.SetActive(true);

			CurrFlagCollor = TeamColor.BlueTeam;
		}
		StartCoroutine("SurvivalTimer");
	}

	IEnumerator SurvivalTimer()
	{
		for(float i = 300;i>=0;i-= Time.deltaTime)
		{
			HUDButtons.Instance.SetTeamScoreLabel((i/60),TeamColor.BlueTeam);
			if(i<=Time.deltaTime)
			{
				Debug.Log("time OUT");
				SurvivalRaundEnd(CurrFlagCollor);
			}
			yield return null;
		}
	}


	private void SetScoreLabelData()
	{
		if (HUDButtons.Instance == null)
		{
			Debug.LogError("BattleController.SetScoreLabelData: HUDButtons not setted or not initialized");
			return;
		}
		HUDButtons.Instance.HudIconsActivation();
		switch (ActiveBattle.Mode)
		{	
			case GameMode.CaptureTheFlag:
				HUDButtons.Instance.SetTeamScoreLabel(BlueFlagCounter, TeamColor.BlueTeam);
				HUDButtons.Instance.SetTeamScoreLabel(OrangeFlagCounter, TeamColor.OrangeTeam);

				break;
			case GameMode.Deathmatch:
				HUDButtons.Instance.SetTeamScoreLabel(BlueFragCounter, TeamColor.BlueTeam);
				HUDButtons.Instance.SetTeamScoreLabel(OrangeFragCounter, TeamColor.OrangeTeam);
				break;
			case GameMode.BaseDefense:
				HUDButtons.Instance.SetTeamScoreLabel(BlueFragCounter, TeamColor.BlueTeam);
				HUDButtons.Instance.SetTeamScoreLabel(OrangeFragCounter, TeamColor.OrangeTeam);
				break;
			case GameMode.Survival:
				StartCoroutine("SurvivalTimer");
		
				HUDButtons.Instance.HideRedScore();
				break;
		
		}
	}

	/// <summary>
	/// Устанавливаем точки спавна кораблей
	/// </summary>
	/// <param name="map">Выбранная карта на которой происходит сражение</param>
	private void SetSpawnPoint(Map map)
	{
		SpawnPoints = new List<Point>();

		OrangeSpawnPoints = new List<Point>();
		BlueSpawnPoints = new List<Point>();
		Map.AvailableNavigatePoint = new List<Point>();

		for (int i = 0; i != map.Cells.Length; i++)
		{
			int x = i%map.FieldWidth;
			int y = i/map.FieldWidth;

			Point point = new Point(x, y);

			if (Map.IsPointAvailableToNavigate(map.Cells[i].Type))
			{
				Map.AvailableNavigatePoint.Add(point);
			}

			if (map.Cells[i].Type == CellType.SpawnPoint)
			{

				SpawnPoints.Add(point);

				if (map.Cells[i].Color == CellColor.Blue)
				{
					BlueSpawnPoints.Add(point);

					SpawnPointBehaviour spawnPoint = ResourceBehaviourController.Instance.InitSpawnPoints(
						ActiveBattle.Map.Environment, TeamColor.BlueTeam);

					if (UIControllerForNGUI.Instance != null)
					{
						if (UIControllerForNGUI.Instance.IsTutorial)
						{
							spawnPoint.transform.position = new Vector3(0, 0, 0);
						}
						else
						{
							spawnPoint.transform.position = Map.GetWorldPosition(ActiveBattle.Map, point);
						}
					}
					else
					{
						spawnPoint.transform.position = Map.GetWorldPosition(ActiveBattle.Map, point);
					}

					SpawnPointsBehaviours.Add(spawnPoint);

				}
				else
				{
					OrangeSpawnPoints.Add(point);

					SpawnPointBehaviour spawnPoint = ResourceBehaviourController.Instance.InitSpawnPoints(
						ActiveBattle.Map.Environment, TeamColor.OrangeTeam);
					spawnPoint.transform.position = Map.GetWorldPosition(ActiveBattle.Map, point);

					SpawnPointsBehaviours.Add(spawnPoint);

				}

			}
			else if (map.Cells[i].Type == CellType.FlagPoint)
			{
				if (map.Cells[i].Color == CellColor.Blue)
					BlueBasePoint = point;
				else
					OrangeBasePoint = point;
			}
		}
	}

	/// <summary>
	/// Получить произвольную точку спавна из списка всех спавн точек
	/// </summary>
	/// <returns></returns>

	public Point GetRandomSpawnPoint()
	{
		int index = UnityEngine.Random.Range(0, SpawnPoints.Count);
		return SpawnPoints[index];
	}

	/// <summary>
	/// Получить произвольную точку спавна по цвету команды
	/// </summary>
	/// <param name="team">Цвет команды</param>
	/// <returns></returns>
	public Point GetRandomSpawnPoint(TeamColor team)
	{
		int index = UnityEngine.Random.Range(0, Mathf.Min(BlueSpawnPoints.Count, OrangeSpawnPoints.Count));
		return team == TeamColor.BlueTeam ? BlueSpawnPoints[index] : OrangeSpawnPoints[index];
	}

	/// <summary>
	/// Получить произвольный вектор из точки спавна
	/// </summary>
	/// <returns></returns>
	public Vector3 GetRandomSpawnPointVector()
	{
		return Map.GetWorldPosition(ActiveBattle.Map, GetRandomSpawnPoint());
	}

	public IPositionable GetIPositionableObject(Vector3 pos)
	{
		float eps = 0.2f;

		IPositionable positionableObject = null;

		for (int i = 0; i < PositionablesObject.Count; i++)
		{
			if (Mathf.Abs(PositionablesObject[i].Position.x - pos.x) < eps &&
			    Mathf.Abs(PositionablesObject[i].Position.z - pos.z) < eps)
			{
				positionableObject = PositionablesObject[i];
				break;
			}
		}

		return positionableObject;
	}

	public List<IPositionable> GetIPositionableList()
	{
		return PositionablesObject;
	}

	/// <summary>
	/// Получить произвольный вектор из точки спавна по цвету команды
	/// </summary>
	/// <param name="team"></param>
	/// <returns></returns>
	public Vector3 GetRandomSpawnPointVector(TeamColor team)
	{
		return Map.GetWorldPosition(ActiveBattle.Map, GetRandomSpawnPoint(team));
	}


	/// <summary>
	/// Установить начальные позиции для всех корабликов
	/// </summary>
	public void SetShipPositionOnStartBattle()
	{
		List<Player> players = GameSetObserver.Instance.Players;
		AIPlayer.BlueBasePoint = BlueBasePoint;
		AIPlayer.OrangeBasePoint = OrangeBasePoint;

		if (BlueFlag != null && BlueFlag.Base != null)
		{
			AIPlayer.BlueBase = BlueFlag.Base.Base;
		}

		if (OrangeFlag != null && OrangeFlag.Base != null)
		{
			AIPlayer.OrangeBase = OrangeFlag.Base.Base;
		}

		if (BlueFlag == null)
		{
		//	Debug.LogError(string.Format("BluFlag.Base is null"));
		}

		if (OrangeFlag.Base.Base == null )
		{
		//	Debug.LogError(string.Format("OrangeFlag.Base is null"));
		}

		if (players == null)
		{
			Debug.LogError("BattleController.SetShipPositionOnStartBattle: players not initialized");
			return;
		}

		foreach (Player player in players)
		{
			SetBattleDataToPlayer(player);
		}
	}

	public void SetBattleDataToPlayer(Player player)
	{
		player.SetMap(ActiveBattle.Map);
		player.SetActiveBattle(ActiveBattle);
	
		player.MyShip.OnRespawn(true);

		if (UIControllerForNGUI.Instance != null)

		if (UIControllerForNGUI.Instance.IsTutorial) {
			player.Position = new Vector3 (0,0,-120);
		}
	
	}

	/// <summary>
	/// Задать значения флагов, и базы где будут распологаться флаги
	/// </summary>
	public void SetBaseDataOnStartBattle()
	{

		Vector3 blueBasePoint = Map.GetWorldPosition(ActiveBattle.Map, BlueBasePoint);
		Vector3 orangeBasePoint = Map.GetWorldPosition(ActiveBattle.Map, OrangeBasePoint);

		ActiveBattle.BlueFlag = FlagParent.Create(TeamColor.BlueTeam, blueBasePoint);

		ActiveBattle.OrangeFlag = FlagParent.Create(TeamColor.OrangeTeam, orangeBasePoint);

		ActiveBattle.BlueBase = BaseParent.Create(blueBasePoint, ActiveBattle.BlueFlag);

		ActiveBattle.OrangeBase = BaseParent.Create(orangeBasePoint, ActiveBattle.OrangeFlag, BaseType.FlagKeeper,
		                                            TeamColor.OrangeTeam);

	}


	public FlagsBehaviour GetFlagsBehaviour(TeamColor team)
	{
		return team == TeamColor.BlueTeam ? BlueFlag : OrangeFlag;
	}

	public FlagSpotBehaviour GetFlagSpotBehaviour(TeamColor team)
	{
		return team == TeamColor.BlueTeam ? BlueFlagSpot : OrangeFlagSpot;
	}

	public ShipBehaviour GetShipBehaviour(int shipId)
	{
		return ships.FirstOrDefault(ship => ship.Player.Id == shipId);
	}


	public void UpdateBombCount(Player player)
	{
		if (HUDButtons.Instance == null || !Equals(GameSetObserver.Instance.Human, player))
			return;

		HUDButtons.Instance.SetBombCountLabel(player.MyShip.BombCount);
	}

	public void UpdateSpecialCount(Player player)
	{
		if (HUDButtons.Instance == null || !Equals(GameSetObserver.Instance.Human, player))
			return;

		//TODO:: when special weapon was added need correct set this data
		HUDButtons.Instance.SetSpecialCountLabel(0);
	}

	#endregion

	
	#region Capture The Flag events

	public void OnFlagTaken(TeamColor color)
	{
		//TODO:: add alert message, for ai change tactic

		if (color == TeamColor.BlueTeam)
		{
			ActiveBattle.BlueFlag.OnFlagTaken();
		}
		else
		{
			ActiveBattle.OrangeFlag.OnFlagTaken();
		}

		if (Toasts.Instance == null)
			return;

		Toasts.Instance.Add(string.Format(FlagStolenMsg, GameSetObserver.Instance.Human.Team != color ? EnemyMsg : YourMsg));

		if (HUDButtons.Instance == null)
			return;

		HUDButtons.Instance.StartFlagBlinking(color);
	}

	public void OnFlagDropped(TeamColor color)
	{
		//TODO:: add alert message, for ai change tactic

		if (color == TeamColor.BlueTeam)
		{
			ActiveBattle.BlueFlag.OnFlagDropped();

		}
		else
		{
			ActiveBattle.OrangeFlag.OnFlagDropped();
		}

		if (Toasts.Instance != null)
			Toasts.Instance.Add(string.Format(FlagDroppedMsg, GameSetObserver.Instance.Human.Team != color ? EnemyMsg : YourMsg));
	}

	public void OnFlagReturned(TeamColor color)
	{
		if (color == TeamColor.BlueTeam)
		{
			ActiveBattle.BlueFlag.OnFlagReturned();

		}
		else
		{
			ActiveBattle.OrangeFlag.OnFlagReturned();
		}

		if (HUDButtons.Instance != null) HUDButtons.Instance.StopFlagBlinking(color);

		if (Toasts.Instance != null)
			Toasts.Instance.Add(string.Format(FlagReturnedMsg, GameSetObserver.Instance.Human.Team != color ? EnemyMsg : YourMsg));
	}

	public void OnFlagDelivered(TeamColor color)
	{
		Debug.Log ("Flag Deliver");
		if (Toasts.Instance != null)
			Toasts.Instance.Add(string.Format(FlagDeliveredMsg, GameSetObserver.Instance.Human.Team != color ? EnemyMsg : YourMsg));

		List<AIPlayer> blueAiPlayers = new List<AIPlayer>();
		List<AIPlayer> redAiPlayers = new List<AIPlayer>();

		
		//  Оповещаем Индикатор количиства флажков, увеличивая количества флагов +1 соответвствующей команде
		if (color == TeamColor.OrangeTeam)
		{
			ActiveBattle.BlueFlag.OnFlagDelivered();
			ActiveBattle.BlueFlagCounter = BlueFlagCounter += flagCountPerDeliver;
			if (HUDButtons.Instance != null) HUDButtons.Instance.SetTeamScoreLabel(BlueFlagCounter, TeamColor.BlueTeam);		
		}
		else
		{
			ActiveBattle.OrangeFlag.OnFlagDelivered();
			ActiveBattle.OrangeFlagCounter = OrangeFlagCounter += flagCountPerDeliver;

			if (HUDButtons.Instance != null) HUDButtons.Instance.SetTeamScoreLabel(OrangeFlagCounter, TeamColor.OrangeTeam);
		}


		if (ActiveBattle.Mode == GameMode.Survival) {
			SurvivalRaundEnd(CurrFlagCollor);
		}

		// Перестаем мигать флажком
		if (HUDButtons.Instance != null) HUDButtons.Instance.StopFlagBlinking(color);

		// Если собрали нужное количество флагов, то окончание битвы
		if (BlueFlagCounter >= ActiveBattle.CountFlagNeed || OrangeFlagCounter >= ActiveBattle.CountFlagNeed)
		{
			Debug.Log("Current Flag "+OrangeFlagCounter+" Flag needed "+ActiveBattle.CountFlagNeed);

			TeamColor teamWin = BlueFlagCounter >= ActiveBattle.CountFlagNeed ? TeamColor.BlueTeam : TeamColor.OrangeTeam;
			if(ActiveBattle.Mode!= GameMode.Survival ){
			OnEndBattle(ActiveBattle.Mode, false, teamWin);
			}
			return;
		}

		// Собираем количество игроков синей и красной команды для смены тактики
		if (GameSetObserver.Instance.BlueTeamPlayers != null)
		{

			foreach (Player blueTeamPlayer in GameSetObserver.Instance.BlueTeamPlayers)
			{
				if (blueTeamPlayer is AIPlayer)
				{
					blueAiPlayers.Add(blueTeamPlayer as AIPlayer);
				}
			}
		}
		
		if (GameSetObserver.Instance.RedTeamPlayers != null)
		{
			foreach (Player redTeamPlayer in GameSetObserver.Instance.RedTeamPlayers)
			{
				if (redTeamPlayer is AIPlayer)
				{
					redAiPlayers.Add(redTeamPlayer as AIPlayer);
				}
			}
		}

		if (blueAiPlayers.Count > 0) 
			AIPlayer.ChangeAiPlayerTactic(blueAiPlayers);

		if (redAiPlayers.Count > 0)
			AIPlayer.ChangeAiPlayerTactic(redAiPlayers);
	
	}

	#endregion

	#region TeamDeath Match Event

	public void OnShipDestroyed(TeamColor color)
	{
		Debug.Log ("ShipDestroyed team: "+color);
		if (color == TeamColor.BlueTeam) {
						BlueFragCounter++;
			HUDButtons.Instance.SetTeamScoreLabel(BlueFragCounter, TeamColor.OrangeTeam);
				}
				else {
						OrangeFragCounter++;
			HUDButtons.Instance.SetTeamScoreLabel(OrangeFragCounter, TeamColor.BlueTeam);
				}


		Debug.Log(ActiveBattle.CountFragNeed+" FRAGS neaded");


		if (BlueFragCounter >= ActiveBattle.CountFragNeed || OrangeFragCounter >= ActiveBattle.CountFragNeed)
		{
			Debug.Log("FRAG ALL DONE");
			TeamColor teamWin = BlueFragCounter >= ActiveBattle.CountFragNeed ? TeamColor.OrangeTeam : TeamColor.BlueTeam;


			OnEndBattle(teamWin);
		}

	}

	#endregion

	public void Reset()
	{
		ActiveBattle.ClearData();

		shells = new List<WeaponBehaviour>();

		BlueBasePoint = null;
		BlueFlag = null;
		BlueFlagSpot = null;

		BlueSpawnPoints = new List<Point>();

		OrangeBasePoint = null;
		OrangeFlag = null;
		OrangeFlagSpot = null;

		OrangeSpawnPoints = new List<Point>();
	}

	#endregion

	#region Shell function
	private List<WeaponBehaviour> shells;

	private int shellCounter = 0;

	public void AddShell(WeaponBehaviour shell)
	{
		if (shells == null)
			shells = new List<WeaponBehaviour>();

		shell.viewID = shellCounter++;

		if (shellCounter >= 1000)
		{
			shellCounter = 0;
		}
		
		shells.Add(shell);
	}

	public void RemoveShell(WeaponBehaviour shell)
	{
		shells.Remove(shell);
	}

	private void RemoveAndDestroyAllShell()
	{
		if (shells == null)
			return;

		for (int i = 0; i != shells.Count; i++)
		{
			WeaponBehaviour weapon = shells[i];
			shells.Remove(weapon);
			Destroy(weapon.gameObject);
			i--;
		}

		shells = null;
	}

	public WeaponBehaviour GetShellByViewID(int viewID)
	{
		if (shells == null)
			return null;

		return shells.FirstOrDefault(shell => shell.viewID == viewID);
	}
	#endregion
}
