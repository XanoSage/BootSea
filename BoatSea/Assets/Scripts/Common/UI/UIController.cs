using System;
using System.Collections.Generic;
using LinqTools;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Battles;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Ship;
//using Aratog.NavyFight.Models.Unity3D.Ships;
using Assets.Scripts.Common.GameLogic.Multiplayer;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using Aratog.NavyFight.Models.Unity3D.Extensions;
//using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIController : MonoBehaviour
{

	#region Variables


	/// <summary>
	/// Using menu and submenu
	/// </summary>
	public enum MenuUiControllsType
	{

		None,
		MainMenu,
		CampaignMenu,
		BattleMenu,
		MultiplayerMenu,
		OptionsMenu,
		TacticScreenMenu,
		BattleFreeMenu,
		CreateGameMultiplayerMenu,
		LobbyCreatingGameMultiplayerMenu,
		SetupTeamMenu

	}

	public enum MenuUIResponceType
	{

		CampaignMenuButtonClicked,
		BattleMenuButtonClicked,
		MultiplayerMenuButtonClicked,
		OptionsMenuClicked,
		BattleFreeButtonClicked,
		BattlePresetButtonClicked,
		TacticScreenButtonClicked,

	}

	public enum DialogUiControllsType
	{

		None,
		CaptureTheFlagVictoryDialog,
		DeathMatchVictoryDialog

	}

	public enum DialogUiResponceType
	{

		None,
		CaptureTheFlagOkButtonClicked,
		DeathMatchOkButtonClicked

	}

	[HideInInspector] public MenuUiControllsType CurrentMenu;

	[HideInInspector] public DialogUiControllsType CurrentDialog;

	// TODO:: For test only
	private Rect BasicRectStart = new Rect(10, 10, 120, 30);
	private float BasicYShiftPosition = 50f;
	private float BasicXShiftPosition = 150f;

	//For Battle Menu

	public BattleConfigurator battle;

	public int currentMapsIndex = 0;

	[HideInInspector] public string CurrentMapName;

	public static UIController Instance { get; private set; }

	public delegate void MenuEventHandler(MenuUIResponceType type);

	public event MenuEventHandler OnMainMenuButtonSelected;

	public void OnMainMenuButtonClicked(MenuUIResponceType type)
	{
		if (OnMainMenuButtonSelected != null)
			OnMainMenuButtonSelected(type);
	}

	#endregion

	#region MonoBehaviour events

	private void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	private void Start()
	{
		CurrentMenu = MenuUiControllsType.MainMenu;
		CurrentDialog = DialogUiControllsType.None;

		battle = new BattleConfigurator();
		battle.Maps.AddRange(GetAvailableMapList());

		roomInfoUpdateTimer = gameObject.AddComponent<Timer>();
		roomInfoUpdateTimer.OnEnd += UpdateRoomInfo;

	}

	#endregion

	public void setGameMode(string mode)
	{
		Debug.Log ("GAME mod" + mode);
			if (battle is BattleConfigurator) {
						switch (mode) {
						case "deathmatch":
								battle.Mode = Aratog.NavyFight.Models.Games.GameMode.Deathmatch;
								break;
						case"Capture the flag":
								battle.Mode = Aratog.NavyFight.Models.Games.GameMode.CaptureTheFlag;
								break;
						case"base defense":
								battle.Mode = Aratog.NavyFight.Models.Games.GameMode.BaseDefense;
								break;
						case"Survival":
								battle.Mode = Aratog.NavyFight.Models.Games.GameMode.Survival;
								break;
			
						}
				}
		
	}


	public void OnBattleEnd()
	{

		OnDialogButtonClicked(DialogUiResponceType.CaptureTheFlagOkButtonClicked);

		battle = new BattleConfigurator();
		battle.Maps.AddRange(GetAvailableMapList());
	}

	#region Dialog Components

	public void SetupDialogData()
	{
		BlueFlagCounter = BattleController.Instance.BlueFlagCounter;
		OrangeFlagCounter = BattleController.Instance.OrangeFlagCounter;
	}

	private int BlueFlagCounter, OrangeFlagCounter;
	public string timeSpentForBattle;

	public void CaptureTheFlagVictoryDialog()
	{
		GUI.Box(new Rect(Screen.width*0.5f - 120, Screen.height*0.5f - 60, 240, 120), "Victory Dialog");

		string labelText = string.Format("BlueFlag:{0}, OrangeFlag:{1}, Time: {2}",
		                                 BlueFlagCounter,
		                                 OrangeFlagCounter,
		                                 timeSpentForBattle);

		GUI.Label(new Rect(Screen.width*0.5f - 100, Screen.height*0.5f - 30, 200, 50), labelText);

		if (GUI.Button(new Rect(Screen.width*0.5f - 60, Screen.height*0.5f + 20, 120, 30), "OK"))
		{
			ShowDialog(DialogUiControllsType.None);
			OnDialogButtonClicked(DialogUiResponceType.CaptureTheFlagOkButtonClicked);
		}
	}

	public void OnDialogButtonClicked(DialogUiResponceType responce)
	{
		switch (responce)
		{
			case DialogUiResponceType.CaptureTheFlagOkButtonClicked:
			UiVictoryController.Instance.Show();
				GameController.Instance.RestartBasicData();

				if (GameController.Instance.CurrentGameType == GameType.Multiplayer)
				{
					MultiplayerManager.Instance.ResetMultiplayer();
					MultiplayerManager.Instance.LeaveActiveRoom();
				}

				SwitchMenuTo(MenuUiControllsType.MainMenu);

			//	StartCoroutine(LoadMainMenu());
				ShowDialog(DialogUiControllsType.None);

				break;

			default:
				break;
		}
	}

	private IEnumerator LoadMainMenu()
	{
		yield return new WaitForSeconds(2);
		Application.LoadLevel(GameController.MainSceneName);
	}

	public void ShowDialog(DialogUiControllsType dialog)
	{
		CurrentDialog = dialog;
	}

	#endregion

	#region Menu components

	private void MainMenu()
	{
		Rect basicRect = BasicRectStart;
		if (GUI.Button(BasicRectStart, "Campaign"))
		{
			SwitchMenuTo(MenuUiControllsType.CampaignMenu);

			OnMainMenuButtonClicked(MenuUIResponceType.CampaignMenuButtonClicked);
		}

		basicRect.y += BasicYShiftPosition;

		if (GUI.Button(basicRect, "Battle Mode"))
		{
			SwitchMenuTo(MenuUiControllsType.BattleMenu);

			OnMainMenuButtonClicked(MenuUIResponceType.BattleMenuButtonClicked);
		}

		basicRect.y += BasicYShiftPosition;

		if (GUI.Button(basicRect, "Multiplayer"))
		{
			SwitchMenuTo(MenuUiControllsType.MultiplayerMenu);

			MultiplayerManager.Instance.TryingConnectToPhoton();

			OnMultiplayerMenuClicked();
		}

		basicRect.y += BasicYShiftPosition;

		if (GUI.Button(basicRect, "Options"))
		{
			SwitchMenuTo(MenuUiControllsType.OptionsMenu);

			OnMainMenuButtonClicked(MenuUIResponceType.OptionsMenuClicked);
		}
	}

	private void CampaignMenu()
	{
		if (GUI.Button(BasicRectStart, "Back"))
		{
			SwitchMenuTo(MenuUiControllsType.MainMenu);
		}
	}


	#region BattleFree menu and Events

	private void BattleFreeMenu()
	{

		DisplayMapSelecting();

		DisplayGameModeSelecting();

		DisplayAdditionParameterSelecting();

		DisplayPlayerCountSelecting();

		DisplayDifficultySelecting();

		//Back to Main Menu
		if (GUI.Button(new Rect(10, 500, 120, 30), "Back"))
		{
			SwitchMenuTo(MenuUiControllsType.MainMenu);

			// TODO: for test only
			if (GameController.Instance.CurrentBattle != null && GameController.Instance.CurrentBattle.IsBattleCreated)
			{
				GameController.Instance.RestartBasicData();
			}
		}

		// Next Button (Tactic screen and equip panel)

		if (GUI.Button(new Rect(210, 500, 120, 30), "Tactic Screen"))
		{
			SwitchMenuTo(MenuUiControllsType.TacticScreenMenu);

			OnTacticScreenButtonClicked(battle, currentMapsIndex);
		}
	}

	public void OnTacticScreen()
	{
		OnTacticScreenButtonClicked(battle, currentMapsIndex);
	}
	public void LoadTutorial()
	{

		OnTacticScreenButtonClicked(battle, 1);
	}
	#region Battle Freee Menu Events

	public void OnTacticScreenButtonClicked(IBattleConfigurator iBattle, int mapIndex)
	{
		iBattle.Map = GameController.LoadMap(iBattle.Maps[mapIndex]);

		if (iBattle.Map == null)
		{
			Debug.LogError(string.Format("OnTacticScreenButtonClicked: can't load Map"));
		}

		GameController.Instance.OnTacticScreenSelected(iBattle);

		OnMainMenuButtonClicked(MenuUIResponceType.TacticScreenButtonClicked);

		SetTeamShipsPreset();
	}

	private void OnPrevMapName()
	{
		currentMapsIndex--;
		if (currentMapsIndex < 0)
			currentMapsIndex = 0;
	}

	private void OnNextMapName()
	{
		currentMapsIndex++;
		if (currentMapsIndex >= battle.Maps.Count)
			currentMapsIndex = battle.Maps.Count - 1;
	}

	private void OnPrevDifficulty()
	{
		battle.Difficulty = (DifficultyType) battle.Difficulty - 1;
		if ((int) battle.Difficulty < (int) DifficultyType.Easy)
			battle.Difficulty = DifficultyType.Easy;
	}

	private void OnNextDifficulty()
	{
		battle.Difficulty = (DifficultyType) battle.Difficulty + 1;
		if ((int) battle.Difficulty > (int) DifficultyType.Insane)
			battle.Difficulty = DifficultyType.Insane;
	}

	private void OnPrevGameMode()
	{
		battle.Mode = (GameMode) battle.Mode - 1;
		if ((int) battle.Mode < (int) GameMode.CaptureTheFlag)
			battle.Mode = GameMode.CaptureTheFlag;
	}

	private void OnNextGameMode()
	{
		Debug.Log ("GAme Mod Change");
		battle.Mode = (GameMode) battle.Mode + 1;
		if ((int) battle.Mode > (int) GameMode.NavalConvoys)
			battle.Mode = GameMode.NavalConvoys;
	}

	#endregion

	#endregion

	private void BattleMenu()
	{
		Rect basicRect = BasicRectStart;
		if (GUI.Button(basicRect, "Preset Battle"))
		{
		}

		basicRect.y += BasicYShiftPosition;

		if (GUI.Button(basicRect, "Free Battle"))
		{
			SwitchMenuTo(MenuUiControllsType.BattleFreeMenu);

			OnMainMenuButtonClicked(MenuUIResponceType.BattleFreeButtonClicked);
			GameController.Instance.OnMainMenuButtonClicked(MenuUIResponceType.BattleFreeButtonClicked);

		}

		basicRect.y += BasicYShiftPosition;

		if (GUI.Button(basicRect, "Back"))
		{
			SwitchMenuTo(MenuUiControllsType.MainMenu);
		}
	}

	private void OptionsMenu()
	{
		if (GUI.Button(BasicRectStart, "Back"))
		{
			SwitchMenuTo(MenuUiControllsType.MainMenu);
		}
	}


	#region MultiplayerMenu

	#region MultiplayerMenu constants

	/// <summary>
	/// Максимальное количество сражений отображаемых на одной странице
	/// </summary>
	private const int MaxRoomOnPage = 20;

	/// <summary>
	/// Время для обновления списка комнат
	/// </summary>
	private const int TimeToUpdateRoomInfoList = 10;

	#endregion

	#region MultiplayerMenu variables

	/// <summary>
	/// для корректного отображения скроллинга (только для OnGUI)
	/// </summary>
	private Vector2 scrollPos = Vector2.zero;

	/// <summary>
	/// список всех созданных комнат
	/// </summary>
	private List<RoomInfo> roomInfosTotal;

	/// <summary>
	/// список отфильтрованных комнат
	/// </summary>
	public List<RoomInfo> filteredRoomList;

	/// <summary>
	/// номер текущей отображаемой страницы
	/// </summary>
	private byte currentPageOfRoomList = 0;

	/// <summary>
	/// переменная для отображения кнопки Присоединиться, вслучае если пользователь выбрал какую
	/// либо комнату
	/// </summary>
	private bool isRoomSelected = false;

	/// <summary>
	/// индекс выбранной комнаты
	/// </summary>
	private int indexOfSelectingRoom = 0;

	/// <summary>
	/// Список параметров, которые будут использоваться при отборе комнат (сражений)
	/// </summary>
	public readonly string[] RoomPropertyList = {"Map Name", "Game Mode", "League", "Battle Data", "Controls"};

	public const byte MapNameIndex = 0;
	public const byte GameModeIndex = 1;
	public const byte LeagueIndex = 2;
	public const byte BattleDataIndex = 3;
	public const byte ControlsIndex = 4;

	/// <summary>
	/// Таймер для обновления списка комнат сражений
	/// </summary>
	private Timer roomInfoUpdateTimer;

	#endregion

	private void MultiplayerMenu()
	{

		GUILayout.BeginArea(new Rect((Screen.width - 400)*0.5f, (Screen.height - 300)*0.5f, 400, 300));

		GUILayout.Label("Main Menu");

		// Player name
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player name:", GUILayout.Width(150));
		MultiplayerManager.MyMultiplayerEntity.photonPlayer.name =
			GUILayout.TextField(MultiplayerManager.MyMultiplayerEntity.photonPlayer.name);
		if (GUI.changed)
		{
			PlayerPrefs.SetString("playerName" + Application.platform, MultiplayerManager.MyMultiplayerEntity.photonPlayer.name);
		}

		GUILayout.EndHorizontal();
		GUILayout.Space(15);

		// Join random room (there must be at least 1 room)
		GUILayout.BeginHorizontal();
		GUILayout.Label("JOIN RANDOM ROOM:", GUILayout.Width(150));
		if (filteredRoomList.Count == 0)
		{
			GUILayout.Label("..no games available...");
		}
		else
		{
			if (GUILayout.Button("GO"))
			{
				PhotonNetwork.JoinRandomRoom();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(30);

		//Show a list of all current rooms
		GUILayout.Label("ROOM LISTING:");
		if (filteredRoomList.Count == 0)
		{
			GUILayout.Label("..no games available..");
		}
		else
		{
			// Room listing: simply call GetRoomList: no need to fetch/poll whatever!
			this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);
			roomInfosTotal = PhotonNetwork.GetRoomList().ToList();
			foreach (RoomInfo game in filteredRoomList)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(game.name + " " + game.playerCount + "/" + game.maxPlayers);
				if (GUILayout.Button("JOIN"))
				{
					PhotonNetwork.JoinRoom(game.name);
				}

				if (game.customProperties.ContainsKey("Game Mode") && game.customProperties.ContainsKey("League"))
				{
					GUILayout.Label(string.Format("Game Type:{0}, League: {1}",
					                              game.customProperties["GameMode"],
					                              game.customProperties["League"]));
				}
				GUILayout.EndHorizontal();
			}

			GUILayout.EndScrollView();
		}

		GUILayout.EndArea();

		BasicRectStart.y = Screen.width - BasicRectStart.y - BasicRectStart.height;
		if (GUI.Button(BasicRectStart, "Back"))
		{
			if (PhotonNetwork.room != null)
			{
				PhotonNetwork.LeaveRoom();
			}

			OnBackButtonMultiplayeMenuClicked();

			//TODO:: temprorary for test
			if (GameController.Instance.CurrentBattle != null && GameController.Instance.CurrentBattle.IsBattleCreated)
			{
				GameController.Instance.RestartBasicData();
			}

			BattleDataReset();

			SwitchMenuTo(MenuUiControllsType.MainMenu);
		}

		if (GUI.Button(new Rect(10, Screen.height - 50, 120, 30), "Create Game"))
		{

			SwitchMenuTo(MenuUiControllsType.CreateGameMultiplayerMenu);

			roomInfoUpdateTimer.Pause();
			BattleDataReset();
		}
	}

	#region Multiplayer Menu events

	/// <summary>
	/// Функция вызывается при входе в мультиплеер меню
	/// </summary>

	public void OnMultiplayerMenuClicked()
	{
		OnMainMenuButtonClicked(MenuUIResponceType.MultiplayerMenuButtonClicked);

		roomInfoUpdateTimer.Launch(TimeToUpdateRoomInfoList);

		isRoomSelected = false;
		indexOfSelectingRoom = 0;

		UpdateRoomInfo();
	}

	/// <summary>
	/// Функция вызывается когда мы возвращаемся в главное меню
	/// </summary>
	public void OnBackButtonMultiplayeMenuClicked()
	{
		roomInfoUpdateTimer.Pause();
	}


	/// <summary>
	/// Обновление списков комнат
	/// </summary>
	public void UpdateRoomInfo()
	{

		MultiplayerManager.Instance.TryingConnectToPhoton();

		if (UIManager.Instance.CurrentMenu == UIMenuInterfaceControllsType.GameList)
		{
			roomInfoUpdateTimer.Launch(TimeToUpdateRoomInfoList);
		}
		else
		{
			return;
		}

		currentPageOfRoomList = 0;
		roomInfosTotal = PhotonNetwork.GetRoomList().ToList();

		FiltersApply();

		UIGameList.Instance.OnRoomInfoUpdate(filteredRoomList);

		Debug.Log(string.Format("Update room info, room count:{0}", filteredRoomList.Count));
	}

	/// <summary>
	/// TODO: need to do this function
	/// </summary>
	public void FiltersApply()
	{
		filteredRoomList = roomInfosTotal;
	}

	/// <summary>
	/// Получить список наименований игровых карт, для выполнения фильтрации
	/// </summary>
	/// <returns></returns>
	public List<string> GetListOfMapName()
	{
		List<string> names = new List<string>();
		foreach (RoomInfo roomInfo in filteredRoomList)
		{
			if (roomInfo.customProperties.ContainsKey(RoomPropertyList[MapNameIndex]))
			{
				string mapName = roomInfo.customProperties[RoomPropertyList[MapNameIndex]] as string;
				names.Add(mapName);
			}
		}
		return names.Unique();
	}

	public void OnCreateRoomButtonClicked()
	{
		DataBuffer buffer = new DataBuffer();

		battle.Serialize(buffer);

		buffer.Write(currentMapsIndex);

		Hashtable table = new Hashtable();
		table.Add(RoomPropertyList[MapNameIndex], battle.Maps[currentMapsIndex]);
		table.Add(RoomPropertyList[GameModeIndex], battle.Mode.ToString());
		table.Add(RoomPropertyList[LeagueIndex], 1);
		table.Add(RoomPropertyList[BattleDataIndex], buffer.StringFromBytes);
		table.Add(RoomPropertyList[ControlsIndex], battle.Mechanics.ToString());

		PhotonNetwork.CreateRoom(MultiplayerManager.Instance.roomName,
		                         true,
		                         true,
		                         battle.BlueTeamPlayersCount*2,
		                         table,
		                         RoomPropertyList);
	}

	#endregion

	#endregion

	#region CreateGame Multiplayer menu

	private void CreateGameMultiplayerMenu()
	{
		Rect someRect = BasicRectStart;
		someRect.y = Screen.height - BasicRectStart.y - BasicRectStart.height;
		if (GUI.Button(someRect, "Back"))
		{
			SwitchMenuTo(MenuUiControllsType.MultiplayerMenu);
			UpdateRoomInfo();
		}


		DisplayMapSelecting();

		DisplayGameModeSelecting();

		DisplayAdditionParameterSelecting();

		DisplayPlayerCountSelecting();

		DisplayPrivateModeSelecting();

		if (GUI.Button(new Rect(210, 500, 120, 30), "Create Game"))
		{
			battle.Map = GameController.LoadMap(battle.Maps[currentMapsIndex]);

			if (battle.Map == null)
			{
				Debug.LogError(string.Format("CreateGameMultiplayerMenu: can't load Map"));
				return;
			}

			GameController.Instance.OnTacticScreenSelected(battle, true);
			OnCreateRoomButtonClicked();
		}
	}

	/// <summary>
	/// Сброс параметров битвы в момент выхода в главное меню из создания битвы, из мультиплеера создания битвы
	/// TODO:: need to add parameters GameType to correct loading map name by game type (local, multiplayer)
	/// </summary>

	public void BattleDataReset()
	{
		battle.ClearData();

		battle.Maps.AddRange(GetAvailableMapList());
	}

	#endregion

	#region TacticScreen Menu and Events

	private const int BasicStartXPos = 10;
	private const int BasicStartYPos = 10;

	private const int BasicButtonWidth = 120;
	private const int BasicButtonHeight = 30;

	private const int BasicLabelWidth = 100;
	private const int BasicLabelHeight = 30;

	private const int BasicShiftXPos = 130;
	private const int BasicShiftYPos = 40;

	internal List<int> blueTeamShipsPreset = new List<int>();
	internal List<int> orangeTeamShipsPreset = new List<int>();

	private void SetTeamShipsPreset()
	{
		orangeTeamShipsPreset.Clear();
		blueTeamShipsPreset.Clear();


		List<Player> aiPlayerFleet = null;

		for (int i = 0; i != GameController.Instance.BlueTeamPlayers.Count; i++)
		{

			if (GameController.Instance.BlueTeamPlayers[i].PlayersFleet == GameController.Instance.HumanPlayerFleet)
			{
				blueTeamShipsPreset.Add(0);
				continue;
			}

			if (aiPlayerFleet == null)
			{
				aiPlayerFleet = GameController.Instance.BlueTeamPlayers[i].PlayersFleet;
				blueTeamShipsPreset.AddRange(GameController.AiPlayersShipTypePreset(aiPlayerFleet));
			}

			if (aiPlayerFleet != null && aiPlayerFleet != GameController.Instance.BlueTeamPlayers[i].PlayersFleet)
			{
				aiPlayerFleet = GameController.Instance.BlueTeamPlayers[i].PlayersFleet;
				blueTeamShipsPreset.AddRange(GameController.AiPlayersShipTypePreset(aiPlayerFleet));
			}

		}

		for (int i = 0; i != GameController.Instance.RedTeamPlayers.Count; i++)
		{
			//orangeTeamShipsPreset.Add(0);

			if (GameController.Instance.RedTeamPlayers[i].PlayersFleet == GameController.Instance.HumanPlayerFleet)
			{
				orangeTeamShipsPreset.Add(0);
				continue;
			}

			if (aiPlayerFleet == null)
			{
				aiPlayerFleet = GameController.Instance.RedTeamPlayers[i].PlayersFleet;
				orangeTeamShipsPreset.AddRange(GameController.AiPlayersShipTypePreset(aiPlayerFleet));
			}

			if (aiPlayerFleet != null && aiPlayerFleet != GameController.Instance.RedTeamPlayers[i].PlayersFleet)
			{
				aiPlayerFleet = GameController.Instance.RedTeamPlayers[i].PlayersFleet;
				orangeTeamShipsPreset.AddRange(GameController.AiPlayersShipTypePreset(aiPlayerFleet));
			}
		}
	}


	private void TacticScreenMenu()
	{

		#region BlueTeam Info and ship setting

		GUILayout.BeginArea(new Rect(10, 10, 600, 600), "Players info");

		List<Player> blueTeamPlayers = GameController.Instance.BlueTeamPlayers;

		for (int i = 0; i != blueTeamPlayers.Count; i++)
		{
			Player player = blueTeamPlayers[i];
			bool playerBelongHumanFleet = GameController.Instance.IsShipBelongHumanFleet(player);


			Ship ship = ShipsPool.Instance[(ShipType) blueTeamShipsPreset[i]];

			if (ship == null)
			{
				Debug.LogError(string.Format("Ship with type {0}, not found", (ShipType) blueTeamShipsPreset[i]));
				GUI.EndGroup();
				return;
			}

			string shipInfo = string.Format(
				"{0} blue team, ship data - Speed: {1}, Armor: {2}, Mines:{3}, Rate: {4}; Tactic:{5}",
				player.IsCaptain ? "Captain" : "Player",
				ship.MaxSpeed,
				ship.HealthPoint,
				ship.BombCount,
				ship.Rate,
				player is AIPlayer ? (player as AIPlayer).Tactic.ToString() : "None");

			GUI.Label(
				new Rect(BasicStartXPos,
				         BasicStartYPos + 2*BasicShiftYPos*i,
				         BasicLabelWidth*2 + BasicButtonWidth,
				         BasicLabelHeight*2),
				shipInfo);

			if (playerBelongHumanFleet &&
			    GUI.Button(
				    new Rect(BasicStartXPos, BasicStartYPos*5 + 2*BasicShiftYPos*i, BasicButtonWidth, BasicButtonHeight),
				    "Prev Ship"))
			{
				blueTeamShipsPreset[i] = OnPrevShip(blueTeamShipsPreset[i], 0);
			}

			GUI.Label(
				new Rect(BasicStartXPos + BasicShiftXPos,
				         BasicShiftYPos*1.3f + 2*BasicShiftYPos*i,
				         BasicLabelWidth,
				         BasicLabelHeight),
				ship.Type.ToString());

			if (playerBelongHumanFleet &&
			    GUI.Button(
				    new Rect(BasicStartXPos + BasicShiftXPos*2,
				             BasicStartYPos*5 + 2*BasicShiftYPos*i,
				             BasicButtonWidth,
				             BasicButtonHeight),
				    "Next Ship"))
			{
				blueTeamShipsPreset[i] = OnNextShip(blueTeamShipsPreset[i], Ship.PoolOfShipType.Count - 1);
			}

			// Players Ai tactic select

			if (playerBelongHumanFleet && (player is AIPlayer))
			{
				AIPlayer aiPlayer = player as AIPlayer;

				if (
					GUI.Button(
						new Rect(BasicStartXPos, BasicStartYPos*2 + 2*BasicShiftYPos*i, BasicButtonWidth, BasicButtonHeight),
						"Prev Tactic"))
				{
					aiPlayer.Tactic = (AITactic) OnPrevShip((int) aiPlayer.Tactic);
				}

				if (
					GUI.Button(
						new Rect(BasicStartXPos + BasicShiftXPos*2,
						         BasicStartYPos*2 + 2*BasicShiftYPos*i,
						         BasicButtonWidth,
						         BasicButtonHeight),
						"Next Tactic"))
				{
					aiPlayer.Tactic = (AITactic) OnNextShip((int) aiPlayer.Tactic, 3);
				}

			}

		}

		GUILayout.EndArea();

		#endregion

		#region OrangeTeam Info and ships setting

		GUI.BeginGroup(new Rect(620, 10, 600, 600), "Orange team Players info");

		List<Player> orangeTeamPlayers = GameController.Instance.RedTeamPlayers;

		for (int i = 0; i != orangeTeamPlayers.Count; i++)
		{
			Player player = orangeTeamPlayers[i];
			bool playerBelongHumanFleet = GameController.Instance.IsShipBelongHumanFleet(player);

			Ship ship = ShipsPool.Instance[(ShipType) orangeTeamShipsPreset[i]];

			if (ship == null)
			{
				Debug.LogError(string.Format("Ship with type {0}, not found", (ShipType) orangeTeamShipsPreset[i]));
				return;
			}

			string shipInfo = string.Format(
				"{0} {6}, ship data - Speed: {1}, Armor: {2}, Mines:{3}, Rate: {4}; Tactic:{5}",
				player.IsCaptain ? "Captain" : "Player",
				ship.Speed,
				ship.HealthPoint,
				ship.BombCount,
				ship.Rate,
				player is AIPlayer ? (player as AIPlayer).Tactic.ToString() : "None",
				player.Team);

			GUI.Label(
				new Rect(BasicStartXPos,
				         BasicStartYPos + 2*BasicShiftYPos*i,
				         BasicLabelWidth*2 + BasicButtonWidth,
				         BasicLabelHeight*2),
				shipInfo);

			if (playerBelongHumanFleet &&
			    GUI.Button(
				    new Rect(BasicStartXPos, BasicStartYPos*5 + 2*BasicShiftYPos*i, BasicButtonWidth, BasicButtonHeight),
				    "Prev Ship"))
			{
				orangeTeamShipsPreset[i] = OnPrevShip(orangeTeamShipsPreset[i], 0);
			}

			GUI.Label(
				new Rect(BasicStartXPos + BasicShiftXPos,
				         BasicShiftYPos*1.3f + 2*BasicShiftYPos*i,
				         BasicLabelWidth,
				         BasicLabelHeight),
				ship.Type.ToString());

			if (playerBelongHumanFleet &&
			    GUI.Button(
				    new Rect(BasicStartXPos + BasicShiftXPos*2,
				             BasicStartYPos*5 + 2*BasicShiftYPos*i,
				             BasicButtonWidth,
				             BasicButtonHeight),
				    "Next Ship"))
			{
				orangeTeamShipsPreset[i] = OnNextShip(orangeTeamShipsPreset[i], Ship.PoolOfShipType.Count - 1);
			}
		}

		GUI.EndGroup();

		#endregion


		if (GUI.Button(new Rect(10, Screen.height - 100, 120, 30), "Back"))
		{
			SwitchMenuTo(MenuUiControllsType.BattleMenu);
		}

		if (GUI.Button(new Rect(210, Screen.height - 100, 120, 30), "Start Battle"))
		{
			SwitchMenuTo(MenuUiControllsType.None);

			GameController.Instance.StartBattle();
		}
	}

	private int OnPrevShip(int current, int min = 0)
	{
		current--;

		if (current < min)
			current = min;

		return current;
	}

	private int OnNextShip(int current, int max = 10)
	{
		current++;

		if (current >= max)
			current = max;

		return current;
	}

	public void OnTacticScreenStartBattle()
	{
		GameController.Instance.StartBattle();
	}

	#endregion


	/// <summary>
	/// Переключить меню 
	/// </summary>
	/// <param name="type"></param>

	public void SwitchMenuTo(MenuUiControllsType type)
	{
		CurrentMenu = type;
	}

	#endregion

	#region Lobby Creating Game Multiplayer Menu

	// отображаем капитанов и их флот, в случае мультиплеера, сюда подтягивается иконка игрока (береться из profileclass)
	// TODO:: need to add Icon image to proile class, and add syncronize function to send icon image for other players
	private void LobbyCreatingGameMultiplayerMenu()
	{
		Rect someRect = BasicRectStart;
		someRect.y = Screen.height - BasicRectStart.y - BasicRectStart.height;

		if (GUI.Button(someRect, "Back"))
		{
			SwitchMenuTo(MenuUiControllsType.MultiplayerMenu);

			OnBackButtonInLobbyCreatingGameMenuClicked();
		}

		GUI.Label(new Rect(20, 30, 200, 60),
		          string.Format("Game mode: {0} \nMap: {1}", battle.Mode, battle.Maps[currentMapsIndex]));

		Rect labelInfoRect = new Rect(BasicShiftXPos*2, BasicShiftYPos*2.2f, 300, 30);
		switch (battle.Mode)
		{
			case GameMode.CaptureTheFlag:
				GUI.Label(labelInfoRect, string.Format("Flag count need: {0}", battle.CountFlagNeed));
				break;
			case GameMode.Deathmatch:
				GUI.Label(labelInfoRect, string.Format("Frag count need: {0}", battle.CountFragNeed));
				break;
			case GameMode.TimeCaptureTheFlag:
				GUI.Label(labelInfoRect,
				          string.Format("Flag count need: {0}, by the time: {1}", battle.CountFlagNeed, battle.TimeNeed));
				break;
		}

		someRect.x = BasicRectStart.x + 3*BasicShiftXPos;
		someRect.y = BasicRectStart.y + BasicShiftYPos;
		if (GUI.Button(someRect, "Change Team"))
		{
			MultiplayerManager.Instance.OnChangePlayerTeamColor(MultiplayerManager.MyMultiplayerEntity);
		}

		List<MultiplayerEntity> entities = MultiplayerManager.MultiplayerEntities;
		if (entities == null)
			return;

		GUI.Label(new Rect(20, 90, 200, 30),
		          string.Format("Player list: players count total:{0}", GameController.Instance.Players.Count));

		Rect rectForPlayerList = new Rect(20, 120, 120, 30);

		foreach (MultiplayerEntity multiplayerEntity in entities)
		{
			GUI.contentColor = MultiplayerManager.MyMultiplayerEntity == multiplayerEntity ? Color.yellow : Color.white;
			if (GUI.Button(rectForPlayerList,
			               string.Format("{0} {1}", multiplayerEntity.photonPlayer.name,
			                             multiplayerEntity.photonPlayer.isMasterClient ? "MC" : "")))
			{
				;
			}

			if (multiplayerEntity.player != null && multiplayerEntity.photonPlayer != null)
			{
				Rect rect = new Rect(rectForPlayerList.x + BasicShiftXPos, rectForPlayerList.y, 500, 30);
				GUI.contentColor = Color.white;
				string str = "" + multiplayerEntity.player.Team + "; ";

				foreach (Player player in multiplayerEntity.player.PlayersFleet)
				{
					if (player.MyShip == null)
						continue;

					if (player.IsCaptain)
						str += " Cap: ";
					else
						str += " AI: ";

					if (player.MyShip != null)
						str += string.Format(" Ship:{0}; ", player.MyShip.Type);

					if (player is AIPlayer)
					{
						AIPlayer ai = player as AIPlayer;
						str += string.Format(" Tactic:{0}; ", ai.Tactic);
					}
				}

				GUI.Label(rect, str);

				MultiplayerPlayer mpPlayer = multiplayerEntity.player as MultiplayerPlayer;
				if (mpPlayer != null)
				{
					Rect rectIsReady = rectForPlayerList;
					rectIsReady.x = rectForPlayerList.x + 520;
					if (GUI.Button(rectIsReady, string.Format("{0}", mpPlayer.IsReady ? "Ready" : "Not Ready")))
					{
						if (MultiplayerManager.MyMultiplayerEntity == multiplayerEntity)
						{
							mpPlayer.SetIsReady(!mpPlayer.IsReady);
						}
					}
				}
			}
			rectForPlayerList.y += BasicShiftYPos;
		}

		GUI.contentColor = Color.white;

		if (GUI.Button(new Rect(10, Screen.height - BasicRectStart.y - BasicRectStart.height - 50, 120, 30), "Setup Team"))
		{
			SwitchMenuTo(MenuUiControllsType.SetupTeamMenu);
			SetPlayerTeamShipsPreset();
			//TODO:: need add reset IsReady data when player entered on this menu
		}

		if (MultiplayerManager.MyMultiplayerEntity.photonPlayer.isMasterClient && IsPlayersReadyToStart(entities) &&
		    GUI.Button(new Rect(10 + BasicShiftXPos, Screen.height - BasicRectStart.y - BasicRectStart.height - 50, 120, 30),
		               "Start"))
		{
			GameController.Instance.StartBattle();
		}
	}


	/// <summary>
	/// Проверка игроков к готовности начать играть сражение
	/// </summary>
	/// <param name="entities">список мультиплеерных сущностей</param>
	/// <returns></returns>
	public static bool IsPlayersReadyToStart(List<MultiplayerEntity> entities)
	{
		bool blueOppositeExist = false;
		bool orangeOppositeExist = false;

		for (int i = 0; i != entities.Count(); i++)
		{
			MultiplayerPlayer mpPlayer = entities[i].player as MultiplayerPlayer;

			if (mpPlayer == null)
				continue;

			if (!blueOppositeExist && mpPlayer.Team == TeamColor.BlueTeam)
			{
				blueOppositeExist = true;
			}

			if (!orangeOppositeExist && mpPlayer.Team == TeamColor.OrangeTeam)
			{
				orangeOppositeExist = true;
			}

			if (!mpPlayer.IsReady)
				return false;
		}

		return blueOppositeExist && orangeOppositeExist;
	}

	/// <summary>
	/// При подсоединении игрока к существующей комнате, переносим его в меню лоби
	/// </summary>
	public void OnPlayerJoinedToGame()
	{
		if (CurrentMenu == MenuUiControllsType.LobbyCreatingGameMultiplayerMenu)
			return;

		SwitchMenuTo(MenuUiControllsType.LobbyCreatingGameMultiplayerMenu);
		roomInfoUpdateTimer.Pause();
	}

	public void OnBackButtonInLobbyCreatingGameMenuClicked()
	{
		Debug.Log("Back from lobby menu");
		PhotonNetwork.LeaveRoom();
		GameController.Instance.RestartBasicData();
		UpdateRoomInfo();
	}

	#endregion

	#region Setup Team on Multiplayer Game

	private List<int> playerTeamShipsPreset = new List<int>();

	private void SetPlayerTeamShipsPreset()
	{
		playerTeamShipsPreset.Clear();

		List<Player> humanTeamPlayers = GameController.Instance.HumanPlayerFleet;
		for (int i = 0; i != humanTeamPlayers.Count; i++)
		{
			playerTeamShipsPreset.Add(humanTeamPlayers[i].MyShip == null ? 0 : (int) humanTeamPlayers[i].MyShip.Type);
		}
		// TODO:: need this setting send by network for other players set
	}
	
	public void SetDefaultPlayerTeamShipsPreset()
	{
		SetPlayerTeamShipsPreset();
		playerTeamShipsPreset[0] = (int) ShipType.Boat;
		for (int i = 1; i < playerTeamShipsPreset.Count; i++)
		{
			playerTeamShipsPreset[i] = Random.Range(0, ShipsPool.PoolOfShips.Count);
		}

		OnSupportTeamSetup(GameController.Instance.Human, playerTeamShipsPreset);
	}

	public void SetPlayerTeamShipsPreset(List<UIShipItem> shipItems)
	{

		playerTeamShipsPreset.Clear();

		List<Player> humanTeamPlayers = GameController.Instance.HumanPlayerFleet;
		for (int i = 0; i < humanTeamPlayers.Count; i++)
		{
			int indexOfType = (int) UIControllerForNGUI.GetNewShipType(shipItems[i].Type);
			playerTeamShipsPreset.Add(/*humanTeamPlayers[i].MyShip == null ? 0 :*/ indexOfType);
		}

		OnSupportTeamSetup(GameController.Instance.Human, playerTeamShipsPreset);
		UILobbyPanel.Instance.UpdateBattleUiInfo();
	}

	private void SetupTeamMenu()
	{
		GUILayout.BeginArea(new Rect(10, 10, 600, 600), "Players info");

		List<Player> humanTeamPlayers = GameController.Instance.HumanPlayerFleet;

		for (int i = 0; i != humanTeamPlayers.Count; i++)
		{
			Player player = humanTeamPlayers[i];
			bool playerBelongHumanFleet = GameController.Instance.IsShipBelongHumanFleet(player);


			Ship ship = ShipsPool.Instance[(ShipType) playerTeamShipsPreset[i]];

			if (ship == null)
			{
				Debug.LogError(string.Format("Ship with type {0}, not found", (ShipType) playerTeamShipsPreset[i]));
				GUI.EndGroup();
				return;
			}

			string shipInfo = string.Format(
				"{0} blue team, ship data - Speed: {1}, Armor: {2}, Mines:{3}, Rate: {4}; Tactic:{5}",
				player.IsCaptain ? "Captain" : "Player",
				ship.MaxSpeed,
				ship.HealthPoint,
				ship.BombCount,
				ship.Rate,
				player is AIPlayer ? (player as AIPlayer).Tactic.ToString() : "None");

			GUI.Label(
				new Rect(BasicStartXPos,
				         BasicStartYPos + 2*BasicShiftYPos*i,
				         BasicLabelWidth*2 + BasicButtonWidth,
				         BasicLabelHeight*2),
				shipInfo);

			if (playerBelongHumanFleet &&
			    GUI.Button(
				    new Rect(BasicStartXPos, BasicStartYPos*5 + 2*BasicShiftYPos*i, BasicButtonWidth, BasicButtonHeight),
				    "Prev Ship"))
			{
				playerTeamShipsPreset[i] = OnPrevShip(playerTeamShipsPreset[i], 0);
			}

			GUI.Label(
				new Rect(BasicStartXPos + BasicShiftXPos,
				         BasicShiftYPos*1.3f + 2*BasicShiftYPos*i,
				         BasicLabelWidth,
				         BasicLabelHeight),
				ship.Type.ToString());

			if (playerBelongHumanFleet &&
			    GUI.Button(
				    new Rect(BasicStartXPos + BasicShiftXPos*2,
				             BasicStartYPos*5 + 2*BasicShiftYPos*i,
				             BasicButtonWidth,
				             BasicButtonHeight),
				    "Next Ship"))
			{
				playerTeamShipsPreset[i] = OnNextShip(playerTeamShipsPreset[i], Ship.PoolOfShipType.Count - 1);
			}

			// Players Ai tactic select

			if (playerBelongHumanFleet && (player is AIPlayer))
			{
				AIPlayer aiPlayer = player as AIPlayer;

				if (
					GUI.Button(
						new Rect(BasicStartXPos, BasicStartYPos*2 + 2*BasicShiftYPos*i, BasicButtonWidth, BasicButtonHeight),
						"Prev Tactic"))
				{
					aiPlayer.Tactic = (AITactic) OnPrevShip((int) aiPlayer.Tactic);
				}

				if (
					GUI.Button(
						new Rect(BasicStartXPos + BasicShiftXPos*2,
						         BasicStartYPos*2 + 2*BasicShiftYPos*i,
						         BasicButtonWidth,
						         BasicButtonHeight),
						"Next Tactic"))
				{
					aiPlayer.Tactic = (AITactic) OnNextShip((int) aiPlayer.Tactic, 3);
				}

			}

		}

		GUILayout.EndArea();
		Rect someRect = BasicRectStart;
		someRect.y = Screen.height - BasicRectStart.y - BasicRectStart.height;

		if (GUI.Button(someRect, "Back"))
		{
			SwitchMenuTo(MenuUiControllsType.LobbyCreatingGameMultiplayerMenu);
		}

		someRect.y = someRect.y - BasicRectStart.height - BasicRectStart.y*0.5f;

		if (GUI.Button(someRect, "Setup"))
		{
			OnSupportTeamSetup(GameController.Instance.Human, playerTeamShipsPreset);
			SwitchMenuTo(MenuUiControllsType.LobbyCreatingGameMultiplayerMenu);
		}
		//TODO:: add two button - "setup" if we firstly set ship and ai data, and "update" if we wont update preset data;
	}

	#region Setup Team Events

	/// <summary>
	/// На основе выбранного корабля и тактик (или же полученных данных о других участниках) создаем корабли
	/// </summary>
	public void OnSupportTeamSetup(Player pl, List<int> shipsPreset, bool fromServer = false)
	{
		List<Player> humanTeamPlayers = pl.PlayersFleet;
		for (int i = 0; i != humanTeamPlayers.Count; i++)
		{
			Player player = humanTeamPlayers[i];
			player.Team = pl.Team;
			player.MyShip = Ship.CreateShip((ShipType) shipsPreset[i]);
			Debug.Log("CREATED SHIP"+player.MyShip);
			player.MyShip.Owner = player;

			MultiplayerPlayer multiplayer = player as MultiplayerPlayer;
			if (multiplayer != null && multiplayer.NeedToSetIsReady)
			{
				multiplayer.SetIsReady(multiplayer.NeedToSetIsReady);
				multiplayer.NeedToSetIsReady = false;
			}
		}
		
		UILobbyPanel.Instance.UpdateBattleUiInfo();
		
		if (fromServer)
			return;

		MultiplayerManager.Instance.SendPlayerGameInfoData(PhotonNetwork.player, pl.PlayersFleet);
	}

	#endregion

	#endregion

	#region Common events

	private int OnPlusIntParameter(int intParameter, int maxValue)
	{
		intParameter++;

		if (intParameter >= maxValue)
			intParameter = maxValue;
		return intParameter;
	}

	private int OnMinusIntParameter(int intParameter, int minValue = 1)
	{
		intParameter--;

		if (intParameter < minValue)
			intParameter = minValue;
		return intParameter;
	}

	private void DisplayMapSelecting()
	{
		//Map select
		GUI.BeginGroup(new Rect(10, 10, 400, 70), "Map Name");

		if (GUI.Button(new Rect(0, 20, 120, 30), "Prev"))
		{
			OnPrevMapName();
		}

		if (battle.Maps.Count == 0)
		{
			Debug.LogError("There is no map name in battle.Maps");
			Debug.Break();
		}

		CurrentMapName = GUI.TextArea(new Rect(140, 20, 120, 30), battle.Maps[currentMapsIndex]);

		if (GUI.Button(new Rect(280, 20, 120, 30), "Next"))
		{
			OnNextMapName();
		}

		GUI.EndGroup();

	}


	private void DisplayGameModeSelecting()
	{
		#region Game Mode Select

		//Game Mode group
		GUI.BeginGroup(new Rect(10, 100, 400, 70), "Game Mode");

		if (GUI.Button(new Rect(0, 20, 120, 30), "Prev"))
		{
			OnPrevGameMode();
		}

		GUI.TextArea(new Rect(140, 20, 120, 30), battle.Mode.ToString());

		if (GUI.Button(new Rect(280, 20, 120, 30), "Next"))
		{
			OnNextGameMode();
		}

		GUI.EndGroup();

		#endregion
	}

	private void DisplayAdditionParameterSelecting()
	{
		#region Additional Game Mode Parameters

		GUI.BeginGroup(new Rect(10, 200, 400, 70), "Game Mode Parameters");

		switch (battle.Mode)
		{
			case GameMode.CaptureTheFlag:
				GUI.Label(new Rect(10, 15, 300, 30), "Needed The Count of Captured flags");
				if (GUI.Button(new Rect(10, 40, 50, 30), "-"))
				{
					battle.CountFlagNeed = OnMinusIntParameter(battle.CountFlagNeed);
				}

				GUI.TextArea(new Rect(70, 40, 50, 30), battle.CountFlagNeed.ToString());

				if (GUI.Button(new Rect(130, 40, 50, 30), "+"))
				{
					battle.CountFlagNeed = OnPlusIntParameter(battle.CountFlagNeed, GameController.MaxModeParameterCount);
				}

				break;
			case GameMode.Deathmatch:
				GUI.Label(new Rect(10, 15, 300, 30), "Needed The Count of Frags");
				if (GUI.Button(new Rect(10, 40, 50, 30), "-"))
				{
					battle.CountFragNeed = OnMinusIntParameter(battle.CountFragNeed);
				}

				GUI.TextArea(new Rect(70, 40, 50, 30), battle.CountFragNeed.ToString());

				if (GUI.Button(new Rect(130, 40, 50, 30), "+"))
				{
					battle.CountFragNeed = OnPlusIntParameter(battle.CountFragNeed, GameController.MaxModeParameterCount);
				}
				break;
			case GameMode.TimeCaptureTheFlag:
				GUI.Label(new Rect(10, 15, 300, 30), "Needed The Count of Captured flags");
				if (GUI.Button(new Rect(10, 40, 50, 30), "-"))
				{
					battle.CountFlagNeed = OnMinusIntParameter(battle.CountFlagNeed);
				}

				GUI.TextArea(new Rect(70, 40, 50, 30), battle.CountFlagNeed.ToString());

				if (GUI.Button(new Rect(130, 40, 50, 30), "+"))
				{
					battle.CountFlagNeed = OnPlusIntParameter(battle.CountFlagNeed, GameController.MaxModeParameterCount);
				}

				GUI.Label(new Rect(260, 15, 300, 30), "Time");
				if (GUI.Button(new Rect(210, 40, 50, 30), "-"))
				{
					battle.TimeNeed = OnMinusIntParameter(battle.TimeNeed);
				}

				GUI.TextArea(new Rect(270, 40, 50, 30), battle.TimeNeed.ToString());

				if (GUI.Button(new Rect(330, 40, 50, 30), "+"))
				{
					battle.TimeNeed = OnPlusIntParameter(battle.TimeNeed, GameController.MaxModeParameterCount);
				}

				break;
			case GameMode.Survival:
				break;
		}
		GUI.EndGroup();

		#endregion
	}

	private void DisplayPlayerCountSelecting()
	{
		#region Players Count and Support Count

		//Blue Team Players count group
		GUI.BeginGroup(new Rect(new Rect(10, 300, 200, 50)), "Team Players Count \t Vs.");

		if (GUI.Button(new Rect(0, 20, 50, 30), "-"))
		{
			battle.BlueTeamPlayersCount = OnMinusIntParameter(battle.BlueTeamPlayersCount);
			battle.OrangeTeamPlayersCount = battle.BlueTeamPlayersCount;
		}

		GUI.TextField(new Rect(60, 20, 40, 30), battle.BlueTeamPlayersCount.ToString());

		if (GUI.Button(new Rect(110, 20, 50, 30), "+"))
		{
			battle.BlueTeamPlayersCount = OnPlusIntParameter(battle.BlueTeamPlayersCount, GameController.MaxPlayerCount);
			battle.OrangeTeamPlayersCount = battle.BlueTeamPlayersCount;
		}

		GUI.EndGroup();

		//Support ship count group

		GUI.BeginGroup(new Rect(10, 360, 200, 50), "Support Ship Count");

		if (GUI.Button(new Rect(0, 20, 50, 30), "-"))
		{
			battle.SupportCount = OnMinusIntParameter(battle.SupportCount);
		}

		GUI.TextField(new Rect(60, 20, 40, 30), battle.SupportCount.ToString());

		if (GUI.Button(new Rect(110, 20, 50, 30), "+"))
		{
			battle.SupportCount = OnPlusIntParameter(battle.SupportCount, GameController.MaxShipSupportCount);
		}

		GUI.EndGroup();

		#endregion
	}

	private void DisplayPrivateModeSelecting()
	{
		#region Private mode

		//Difficulty group
		GUI.BeginGroup(new Rect(10, 430, 400, 70), "Private mode");

		if (GUI.Button(new Rect(0, 20, 120, 30), "Prev"))
		{
			battle.IsPrivateGame = !battle.IsPrivateGame;
		}

		GUI.TextArea(new Rect(140, 20, 120, 30), string.Format("{0}", battle.IsPrivateGame ? "Private" : "Common"));

		if (GUI.Button(new Rect(280, 20, 120, 30), "Next"))
		{
			battle.IsPrivateGame = !battle.IsPrivateGame;
		}

		GUI.EndGroup();

		#endregion
	}

	private void DisplayDifficultySelecting()
	{

		#region Difficulty

		//Difficulty group
		GUI.BeginGroup(new Rect(10, 430, 400, 70), "Difficulty");

		if (GUI.Button(new Rect(0, 20, 120, 30), "Prev"))
		{
			OnPrevDifficulty();
		}

		GUI.TextArea(new Rect(140, 20, 120, 30), battle.Difficulty.ToString());

		if (GUI.Button(new Rect(280, 20, 120, 30), "Next"))
		{
			OnNextDifficulty();
		}

		GUI.EndGroup();

		#endregion

	}

	/// <summary>
	/// TODO:: need added logic gettering available map list name
	/// </summary>
	/// <returns></returns>
	public static List<string> GetAvailableMapList()
	{
		List<string> mapListName = new List<string> {"Machine City", "Tutorial"};
		return mapListName;
	}

	public List<string> GetAvailableModeList()
	{
		List<string> modes = new List<string>();
		for (int i = 0; i < 6; i++)
		{
			modes.Add(((GameMode) i).ToString());
		}
		return modes;
	}

	public List<string> GetAvailableListOfComplexity()
	{
		List<string> complexities = new List<string>();
		int difficultyCount = 4;
		for (int i = 0; i < difficultyCount; i++)
		{
			complexities.Add(((DifficultyType) i).ToString());
		}
		return complexities;
	}

	public void SetUIBattle(DataBuffer buffer)
	{
		IBattleConfigurator battleConfig = new BattleConfigurator();

		battleConfig.Deserialize(buffer);

		battleConfig.Maps = GetAvailableMapList();

		currentMapsIndex = buffer.ReadInt();
		Debug.Log ("CREATE MAP");
		battleConfig.Map = GameController.LoadMap(battleConfig.Maps[currentMapsIndex]);
	
		if (battleConfig.Map == null)
		{
			Debug.LogError(string.Format("SetUIBattle: can't load Map"));
		}

		battle.UpdateData(battleConfig);

		print(string.Format("UIController.SetUIBattle - mechanic type is {0}", battle.Mechanics));

		GameController.Instance.OnTacticScreenSelected(battle, true);
	}



	/// <summary>
	/// For Playable Demo Only: Set Default multiplayer game battle setting
	/// </summary>
	public void SetDefaultMultiplayerBattleData()
	{
		battle.IsPrivateGame = false;

		battle.BlueTeamPlayersCount = 1;
		battle.OrangeTeamPlayersCount = 1;
        battle.SupportCount = 2;

		battle.CountFlagNeed = 2;

	}

	#endregion

	#region Events

	public void OnBattleStart()
	{

		int idCounter = 0;

		bool isNeedCreateShip = GameController.Instance.CurrentGameType != GameType.Multiplayer;

		SetPlayerAdditionalData(GameController.Instance.BlueTeamPlayers, ref idCounter, isNeedCreateShip);
		SetPlayerAdditionalData(GameController.Instance.RedTeamPlayers, ref idCounter, isNeedCreateShip);
	}

	private void SetPlayerAdditionalData(List<Player> players, ref int currentCounter, bool isNeedCreateShip)
	{
		for (int i = 0; i != players.Count; i++)
		{
			Player player = players[i];

			if (isNeedCreateShip)
			{
				player.MyShip =
					Ship.CreateShip((player.Team == TeamColor.BlueTeam)
						                ? (ShipType) blueTeamShipsPreset[i]
						                : (ShipType) orangeTeamShipsPreset[i]);
			}

			player.MyShip.Owner = player;

			player.Init();
		}
	}

	#endregion
}