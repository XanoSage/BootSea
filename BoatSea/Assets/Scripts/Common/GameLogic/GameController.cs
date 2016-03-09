using System;
using System.Collections;
using System.IO;
using Assets.Scripts.Common.GameLogic;
using LinqTools;
using System.Collections.Generic;
using Aratog.NavyFight.GameLogic.Infrastructure;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D;
using Aratog.NavyFight.Models.Unity3D.Battles;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Ship;
using Aratog.NavyFight.Models.Unity3D.TaskManager;
using Assets.Scripts.Common.GameLogic.Multiplayer;
using UnityEngine;
using Aratog.NavyFight.Models.Unity3D.Players;
using ViageSoft.SystemServices.Contextual;
using MultiplayerPlayer = Assets.Scripts.Common.GameLogic.Multiplayer.MultiplayerPlayer;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour, IInitable, IGameController 
{
	#region Constants

	public const int MaxPlayerCount = 4;
	public const int MaxShipSupportCount = 3;
	public const int MaxModeParameterCount = 99;

	public const string LevelXMLPath = "XML/LevelLogic/";

	public const string MainSceneName = "DemoScene";

	#endregion

	#region Variables


	public int AdmiralsTargetCount =0;
	public int AdmiralsTargetCurrent =0;
	public string AdmiralTarget = "";
	public bool IsAdmiralComplet = false;

	// number player team bots;
	public int bootsCount = 2;

	private ClientApplication _application;


	//Campaign variables
	public int currentLevel = 0;


	//Game controller instance
	public static GameController Instance { get; private set; }

	public bool IsPause { get; set; }

	public MechanicsType Mechanics { get; set; }

	// TODO:: for test ONLY, remove after testing
	public Camera UICamera;
	public Camera GameCamera;

	// TODO:: for test ONLY, remove after testing
	public GameObject[] ships;

	[SerializeField] private GameObject _hud, _panels;

	private Timer timer1;

	/// <summary>
	/// Тип Игры
	/// </summary>
	public GameType CurrentGameType { get; set; }

	/// <summary>
	/// Текущая битва
	/// </summary>
	public Battle CurrentBattle { get; set; }

	/// <summary>
	/// Кампания
	/// </summary>
	//TODO:: add campaign class
	//public Campaign CurrentCampaign;

	/// <summary>
	/// Список всех игроков
	/// </summary>
	public List<Player> Players { get; set; }

	/// <summary>
	/// Капитан синей команды
	/// </summary>
	public Player BlueTeamCaptain
	{
		get { return Players.FirstOrDefault(player => player.IsCaptain && player.Team == TeamColor.BlueTeam); }
	}

	/// <summary>
	/// Список игроков Оранжевой команды
	/// </summary>
	public List<Player> RedTeamPlayers { get; set; }

	//public List<Player> RedTeamPlayers { get; set; }

	/// <summary>
	/// Список игроков синей команды
	/// </summary>
	public List<Player> BlueTeamPlayers { get; set; }

	/// <summary>
	/// Капитан синей команды
	/// </summary>
	public Player OrangeTeamCaptain
	{
		get { return Players.FirstOrDefault(player => player.IsCaptain && player.Team == TeamColor.OrangeTeam); }
	}

	public bool IsBattleStarted {get; set; }

	public Player Human { get; set; }

	public List<Player> HumanPlayerFleet
	{
		get { return Human.PlayersFleet; }
	}

	public void ClearHumanPlayersFleet()
	{
		HumanPlayerFleet.Clear();
	}

	public delegate void OnStartEventHandler();

	public delegate void OnBattleEndEventHandler();

	public event OnStartEventHandler OnStartBattle;

	public event OnBattleEndEventHandler OnEndBattle;

	#endregion

	#region MonoBehaviour events

	private void Awake()
	{
		_application = new ClientApplication();

		Instance = this;

		Init();
	}

	private void Start()
	{
		_application.Start();

		if (UIController.Instance != null)
			UIController.Instance.OnMainMenuButtonSelected += OnMainMenuButtonClicked;

		timer1 = gameObject.AddComponent<Timer>();
		timer1.OnEnd += Timer1End;
		timer1.Launch(5, true);



        // Load Music and SFX state
        SoundManager.SetVolumeMusic(Options.MusicVolume);
        SoundManager.SetVolumeSFX(Options.SFXVolume);
	}

	private void OnApplicationQuit()
	{
		if (_application != null)
			_application.Stop();
	}


	#region Options

#if UNITY_EDITOR
	[SerializeField] private bool _resetOptionsOnAwake = false;
#endif

	private void LoadOptions()
	{
		bool resetOptionsToDefaultValues = Options.IsFirstLaunch;
#if UNITY_EDITOR
		if (!resetOptionsToDefaultValues)
			resetOptionsToDefaultValues = _resetOptionsOnAwake;
#endif
		if (resetOptionsToDefaultValues)
			Options.ResetAll();

		Mechanics = Options.Mechanics;
	}

	#endregion

	private void Update()
	{

	}

	#endregion

	#region Events

	private void Timer1End()
	{
		timer1.OnEnd -= Timer1End;
	}

	public void EnableCamera(bool enable = true)
	{
		GameCamera.gameObject.SetActive(enable);
	}

	public void SwitchCamera()
	{
		GameCamera.gameObject.SetActive(false);
	}

	public void Init()
	{
		Players = new List<Player>();

		ShipsPool.Restart();

		IsBattleStarted = false;
		IsGameReadyToStart = false;
	}

	public void RestartBasicData()
	{
		Init();

		if (CurrentBattle != null)
			CurrentBattle.ClearData();
	}

	public void AdmiralTargetCheck()
	{
		if (PlayerInfo.Instance.AdmiralQuestGiven) {
			if (!PlayerInfo.Instance.AdmiralQuestComplet) {
				if (!IsAdmiralComplet) {
					AdmiralsTargetCurrent++;
					if (AdmiralsTargetCurrent >= AdmiralsTargetCount) {
						Toasts.Instance.Add ("Admirals Quest Complet");
						AdmiralTarget = "";
						IsAdmiralComplet = true;
					}
				}
			}
		}
	}

	public void SetGameType(GameType game)
	{
		CurrentGameType = game;
	}


	public void PauseGame()
	{
		IsPause = !IsPause;
	}

	/// <summary>
	/// Функция которая будет вызываться в момент нажатия на кнопку главного меню
	/// </summary>
	/// <param name="responce"></param>
	public void OnMainMenuButtonClicked(UIController.MenuUIResponceType responce)
	{
		Debug.Log ("OnMainMenuButtonSelected");
		switch (responce)
		{
			case UIController.MenuUIResponceType.CampaignMenuButtonClicked:
				SetGameType(GameType.Campaign);
				break;
			case UIController.MenuUIResponceType.BattlePresetButtonClicked:
				SetGameType(GameType.BattlePreset);
				break;
			case UIController.MenuUIResponceType.BattleFreeButtonClicked:
				SetGameType(GameType.BattleFree);
				break;
			case UIController.MenuUIResponceType.MultiplayerMenuButtonClicked:
				SetGameType(GameType.Multiplayer);
				break;
			case UIController.MenuUIResponceType.TacticScreenButtonClicked:
				break;
		}
	}

	public void OnTacticScreenSelected(IBattleConfigurator battle, bool fromServer = false)
	{

		Debug.Log ("Create Battle");
		CurrentBattle = new Battle(battle);
		CurrentBattle.SupportCount = bootsCount;
		CurrentBattle.IsBattleCreated = true;
	
		if (fromServer)
			return;

		CreateHumanPlayers();
	}

	public void CreateHumanPlayers()
	{
		Debug.Log ("CreateHumanPlayers");
		Players = CreatePlayers(CurrentBattle, CurrentGameType);

		SetBasicParameters();
	}

	/// <summary>
	/// Начинаем играть в выбранное сражение
	/// </summary>
	public void StartBattle(bool fromServer = false)
	{
		IsPause = false;

		// Временная фигня
		if (Application.loadedLevelName == "Demo - Handling")
			EnableCamera();
		else
		{
			SwitchCamera();
			EnableCamera();
		}

		_panels.SetActive(false);
		_hud.SetActive(true);

		if (fromServer)
			Mechanics = CurrentBattle.Mechanics;

		Player.Mechanics = Mechanics;

		if (OnStartBattle != null)
		{
			OnStartBattle();
		}

		IsBattleStarted = true;

		if (CurrentGameType != GameType.Multiplayer)
			return;

		if (fromServer)
			return;

		MultiplayerManager.Instance.StartBattle();
	}

	public void BattleEnd()
	{
		IsPause = true;

		IsBattleStarted = false;

		if (OnEndBattle != null)
			OnEndBattle();

		//TODO:: need split multiplayer ending the game

		EnableCamera(false);

		BattleController.Instance.ResetBattleData(true);

		UIController.Instance.ShowDialog(UIController.DialogUiControllsType.CaptureTheFlagVictoryDialog);
	}

	/// <summary>
	/// задаём значения для списка игроков синей/оранжевой команды, определим игрока
	/// </summary>
	public void SetBasicParameters()
	{
		Debug.Log("Basic Param");
		if (Players == null || Players.Count <= 0) return;

		BlueTeamPlayers = Players.FindAll(player => player.Team == TeamColor.BlueTeam);
		RedTeamPlayers = Players.FindAll(player => player.Team == TeamColor.OrangeTeam);

		if (CurrentGameType != GameType.Multiplayer)
			Human = Players.FirstOrDefault(player => player is HumanPlayer);
		else
			Human =
				Players.FirstOrDefault(
					player =>
					(player is MultiplayerPlayer && (player as MultiplayerPlayer).Entity == MultiplayerManager.MyMultiplayerEntity));

		int idCounter = 0;

		for (int i = 0; i != Players.Count; i++)
		{
			Players[i].Id = idCounter++;
		}

		if (CurrentGameType != GameType.Multiplayer)
			return;

		MultiplayerManager.Instance.RequestForPlayerIdSynchronize();
	}


	/// <summary>
	/// Принадлежит ли плеер команде Игрока-человека
	/// </summary>
	/// <param name="pl"></param>
	/// <returns></returns>
	public bool IsShipBelongHumanFleet(Player pl)
	{
		return HumanPlayerFleet.Any(player => player == pl);
	}


	public Player GetPlayer(int playerId)
	{

		return Players.FirstOrDefault(player => player.Id == playerId);
	}

	/// <summary>
	/// Загружаем логическую карту из хранилища карт
	/// </summary>
	/// <param name="name">Имя карты</param>
	/// <returns></returns>
	public static Map LoadMap(string name = "New Map")
	{
		string filename = Path.Combine(LevelXMLPath, name);
		Debug.Log ("File patch "+filename);
		TextAsset file = Resources.Load(filename, typeof (TextAsset)) as TextAsset;
		Debug.Log ("file  "+file);
		Map map = Map.LoadFromText(file.text);
		//map.InitWaypoints();
		return map; //Map.LoadFromText(file.text);
	}


	public void OnPlayerChangeTeamColor(Player pl)
	{

		TeamColor playerColor = pl.Team;
		int teamColorCount = Instance.Players.Count(player => player.IsCaptain && player.Team != playerColor);
		if (teamColorCount < Instance.CurrentBattle.BlueTeamPlayersCount)
		{
			pl.OnChangeTeamColor(playerColor == TeamColor.BlueTeam ? TeamColor.OrangeTeam : TeamColor.BlueTeam);
		}
		else
		{
			//TODO:: need add pop-up message that players with this color is maximum
			Debug.LogError("players with this color is maximum");
		}
	}

	#region PlayersCreating events

	/// <summary>
	/// Создаем игроков в зависисмости от выбранного режима игры
	/// </summary>
	/// <param name="battle"></param>
	/// <param name="gameType"></param>
	/// <returns></returns>
	public static List<Player> CreatePlayers(IBattleConfigurator battle, GameType gameType)
	{
		List<Player> players = new List<Player>();

		if (gameType == GameType.Multiplayer)
		{
			RemotePlayerSetting(battle, players);
		}
		else
		{
			LocalPlayerSetting(battle, players);
		}

		return players;
	}

	public static Player CreatePlayer(DataBuffer playerBuffer, MultiplayerEntity entity)
	{
		Player player = null;

		PlayerType type = (PlayerType) playerBuffer.ReadInt();
		TeamColor color = (TeamColor) playerBuffer.ReadInt();

		bool isCaptain = playerBuffer.ReadBool();

		bool isLocal = type != PlayerType.AIPlayer || playerBuffer.ReadBool();

		if (type != PlayerType.MultiplayerPlayer)
			player = Player.CreatePlayer(type, isCaptain, color, isLocal);

		else
		{

			if (MultiplayerManager.IsMasterClient)
			{

				int blueTeamColorCount = Instance.Players.Count(qplayer => qplayer.IsCaptain && qplayer.Team == TeamColor.BlueTeam);
				int orangeTeamColorCount =
					Instance.Players.Count(qplayer => qplayer.IsCaptain && qplayer.Team == TeamColor.OrangeTeam);
				if (blueTeamColorCount < Instance.CurrentBattle.BlueTeamPlayersCount)
					color = TeamColor.BlueTeam;
				else if (orangeTeamColorCount < Instance.CurrentBattle.OrangeTeamPlayersCount)
					color = TeamColor.OrangeTeam;
				else
				{
					//TODO:: need to show alert message that players is Full end return players to room list info
				}

			}
			player = MultiplayerPlayer.Create(entity, color);

			MultiplayerPlayer multiplayer = player as MultiplayerPlayer;
			if (multiplayer != null)
			{
				multiplayer.NeedToSetIsReady = playerBuffer.ReadBool();
			}

		}

		GameController.Instance.Players.Add(player);

		player.PlayersFleet = new List<Player> {player};

		//Создаем игроков поддержки
		for (int i = 0; i != Instance.CurrentBattle.SupportCount; i++)
		{
			Player supportPlayer = Player.CreatePlayer(PlayerType.AIPlayer, false, player.Team);
			player.PlayersFleet.Add(supportPlayer);
			supportPlayer.PlayersFleet = player.PlayersFleet;
			Instance.Players.Add(supportPlayer);
		}

		return player;
	}

	private static void RemotePlayerSetting(IBattleConfigurator battle, ICollection<Player> players)
	{
		Player player = MultiplayerPlayer.Create(MultiplayerManager.MyMultiplayerEntity,
		                                         battle.Team == TeamColor.None ? TeamColor.BlueTeam : battle.Team);

		MultiplayerManager.MyMultiplayerEntity.player = player;

		players.Add(player);

		player.PlayersFleet = new List<Player> {player};

		//Создаем игроков поддержки
		for (int i = 0; i != battle.SupportCount; i++)
		{
			Player supportPlayer = Player.CreatePlayer(PlayerType.AIPlayer, false, player.Team);
			player.PlayersFleet.Add(supportPlayer);
			supportPlayer.PlayersFleet = player.PlayersFleet;
			players.Add(supportPlayer);
		}
	}

	private static void LocalPlayerSetting(IBattleConfigurator battle, ICollection<Player> players)
	{
		Debug.Log ("LocalPlayerSetting");
		//Создаем плеера для игрока
		Player playerHuman = Player.CreatePlayer(PlayerType.HumanPlayer,
		                                    battle.Team == TeamColor.None ? TeamColor.BlueTeam : battle.Team);
        players.Add(playerHuman);

        playerHuman.PlayersFleet = new List<Player> { playerHuman };

		//Создаем игроков поддержки
		for (int i = 0; i != battle.SupportCount; i++)
		{
            Player supportPlayer = Player.CreatePlayer(PlayerType.AIPlayer, false, playerHuman.Team);
            playerHuman.PlayersFleet.Add(supportPlayer);
            supportPlayer.PlayersFleet = playerHuman.PlayersFleet;
			players.Add(supportPlayer);
		}

		//Создаем игроков-капитанов синей команды
        for (int i = 0; i != battle.BlueTeamPlayersCount - (playerHuman.Team == TeamColor.BlueTeam ? 1 : 0); i++)
		{
			Player player = Player.CreatePlayer(PlayerType.AIPlayer, true, TeamColor.BlueTeam);
			players.Add(player);

			player.PlayersFleet = new List<Player> {player};

			//Создаем игроков поддержки для каждого капитана синей команды
			for (int j = 0; j != battle.SupportCount; j++)
			{
				Player supportPlayer = Player.CreatePlayer(PlayerType.AIPlayer, false, TeamColor.BlueTeam);
				player.PlayersFleet.Add(supportPlayer);
				supportPlayer.PlayersFleet = player.PlayersFleet;
				players.Add(supportPlayer);
			}

			//Задаем тактику для комманд под упралением ИИ
			AIPlayerTacticSelect(player.PlayersFleet);
		}

		//Создаем игроков-капитанов оранжевой команды
        for (int i = 0; i != battle.OrangeTeamPlayersCount - (playerHuman.Team == TeamColor.OrangeTeam ? 1 : 0); i++)
		{
			Player player = Player.CreatePlayer(PlayerType.AIPlayer, true, TeamColor.OrangeTeam);
			players.Add(player);

			player.PlayersFleet = new List<Player> {player};

			//Создаем игроков поддержки для каждого капитана оранжевой команды
			for (int j = 0; j != battle.SupportCount; j++)
			{
				Player supportPlayer = Player.CreatePlayer(PlayerType.AIPlayer, false, TeamColor.OrangeTeam);
				player.PlayersFleet.Add(supportPlayer);
				supportPlayer.PlayersFleet = player.PlayersFleet;
				players.Add(supportPlayer);
			}

			//Задаем тактику для комманд под упралением ИИ
			AIPlayerTacticSelect(player.PlayersFleet);
		}
	}

	#endregion

	#region Set AITactic and Ship Preset

	/// <summary>
	/// задаем тактику для игроков под управлением ИИ
	/// </summary>
	/// <param name="players">Список игроков одного флота</param>
	public static void AIPlayerTacticSelect(IEnumerable<Player> players)
	{
		List<AIPlayer> aiPlayers = players.OfType<AIPlayer>().Select(player => player as AIPlayer).ToList();
		int choise = Random.Range(1, 3);

		AIPlayer.AiTacticPreset(aiPlayers, choise);
	}

	/// <summary>
	/// Задаем предустановки типов кораблей для игроков под упралением ИИ
	/// </summary>
	/// <param name="players"></param>
	/// <returns></returns>
	public static IEnumerable<int> AiPlayersShipTypePreset(IEnumerable<Player> players)
	{
		List<int> shipPreset = new List<int>();

		foreach (Player player in players)
		{
			AIPlayer aiPlayer = player as AIPlayer;
			if (aiPlayer == null)
				continue;
			float chance = Random.Range(0, 10000)/10000f;
			shipPreset.Add((int) AIPlayer.TypeShipPresetByAiTactic(aiPlayer.Tactic, chance));
		}

		return shipPreset;
	}

	public static void AiPlayerTacticSet(List<Player> players, List<int> aiTacticPreset)
	{
		//TODO:: need add condition equals count of this lists

		for (int i = 0; i != players.Count; i++)
		{
			AIPlayer aiPlayer = players[i] as AIPlayer;

			if (aiPlayer == null)
				continue;

			aiPlayer.Tactic = (AITactic) aiTacticPreset[i];
		}
	}


	#endregion

	#region Multiplayer Events

	public bool IsGameReadyToStart { get; private set; }

	#endregion


	public static TeamColor GetOpponentColor(TeamColor color)
	{
		return color == TeamColor.BlueTeam ? TeamColor.OrangeTeam : TeamColor.BlueTeam;
	}

	public static Player GetCaptain(Player player)
	{
		if (player.IsCaptain)
			return player;

		List<Player> fleet = player.PlayersFleet;
		for (int i = 0; i != fleet.Count; i++)
		{
			if (fleet[i].IsCaptain)
				return fleet[i];
		}

		return null;
	}

	#endregion
}
