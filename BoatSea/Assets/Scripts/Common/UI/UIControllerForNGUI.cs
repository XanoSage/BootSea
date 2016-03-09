using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Battles;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Ship;
//using Aratog.NavyFight.Models.Unity3D.Ships;

public class UIControllerForNGUI : MonoBehaviour
{
	public static UIControllerForNGUI Instance { get; private set; }

	[SerializeField] private UIEventListener _startBattle,_startCompaign, _tacticScreen,_comapainScreen;

	[SerializeField] private UIEventListener _tutorialBtn;


	[SerializeField] private List<UIChooser> _map, _mode, _complexity;

	[SerializeField] private List<UICounter> _blueCount, _orangeCount, _flagCount, _supportCount;

	[SerializeField] private List<UIShipItem> _shipsSlots;

	[SerializeField] private UILoadingScreen _loadingScreen;

	public bool IsTutorial = false;

	public bool IsMultiplayer
	{
		get { return GameController.Instance.CurrentGameType == GameType.Multiplayer; }
	}

	private UIChooser _mapField
	{
		get
		{
			if (IsMultiplayer)
				return _map[1];
			return _map[0];
		}
	}

	private UIChooser _modeField
	{
		get
		{
			if (IsMultiplayer)
			{Debug.Log("mode Change");
				return _mode[1];}

			return _mode[0];
		}
	}

	private UICounter _flagCountField
	{
		get
		{
			if (IsMultiplayer)
				return _flagCount[1];
			return _flagCount[0];
		}
	}

	private UIChooser _complexityField
	{
		get
		{
			if (GameController.Instance.CurrentGameType == GameType.Multiplayer)
				return _complexity[1];
			return _complexity[0];
		}
	}

	private UICounter _blueCountField
	{
		get
		{
			if (IsMultiplayer)
				return _blueCount[1];
			return _blueCount[0];
		}
	}

	private UICounter _orangeCountField
	{
		get
		{
			if (IsMultiplayer)
				return _orangeCount[1];
			return _orangeCount[0];
		}
	}
	
	private UICounter _supportCountField
	{
		get
		{
			return _supportCount[0];
		}
	}

	private void Start()
	{
		_mode[0].nextButton.onClick += HideFlagCounter;
		_mode[0].prevButton.onClick += HideFlagCounter;

		Instance = this;

		_startBattle.onClick += OnStartBattle;
		_startCompaign.onClick += OnStartCompaign;
		_tacticScreen.onClick += OnTacticScreen;
		_comapainScreen.onClick += OnTacticScreenCampaign;
		_tutorialBtn.onClick += OnTutorialClick;


		UIBattleDetailsPanel.Instance.NextBtn.onClick += UIBattleDetailsPanel.Instance.OnNextBtnClick;

		foreach (UIChooser t in _map)
			t.SetOptions(UIController.GetAvailableMapList());

		//TODO:: after playable demo remove [0]
	/*	foreach (UIChooser t in _mode) {
						t.SetOption (UIController.Instance.GetAvailableModeList () [0]);
				}*/

		_complexity[0].SetOptions(UIController.Instance.GetAvailableListOfComplexity());

		_blueCountField.SetLimitations(0, GameController.MaxPlayerCount);
		_orangeCountField.SetLimitations(0, GameController.MaxPlayerCount);
	}




	void HideFlagCounter(GameObject sender)
	{
		if (_mode [0].CurrentSelection == "Capture the flag") {
			_flagCount[0].gameObject.SetActive(true);
		}
		else if (_mode [0].CurrentSelection == "deathmatch") {
			_flagCount[0].gameObject.SetActive(true);
		}
		else if (_mode [0].CurrentSelection == "base defense") {
			_flagCount[0].gameObject.SetActive(false);
		}
		else if (_mode [0].CurrentSelection == "Survival") {
			_flagCount[0].gameObject.SetActive(false);
		}

	}


	private bool SendShipType (string ship)
	{
		ShipsGlobal.ShipType type = ShipsGlobal.ShipType.Small;
		switch (ship) {
		case "Small":
			type = ShipsGlobal.ShipType.Small;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "Middle":
			type = ShipsGlobal.ShipType.Middle;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "Big":
			type = ShipsGlobal.ShipType.Big;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "SmallMetal":
			type = ShipsGlobal.ShipType.SmallMetal;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "MiddleMetal":
			type = ShipsGlobal.ShipType.MiddleMetal;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "BigMetal":
			type = ShipsGlobal.ShipType.BigMetal;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "SmallAtlant":
			type = ShipsGlobal.ShipType.SmallAtlant;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "MiddleAtlant":
			type = ShipsGlobal.ShipType.MiddleAtlant;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "BigAtlant":
			type = ShipsGlobal.ShipType.BigAtlant;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "SmallDark":
			type = ShipsGlobal.ShipType.SmallDark;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "MiddleDark":
			type = ShipsGlobal.ShipType.MiddleDark;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		case "BigDark":
			type = ShipsGlobal.ShipType.BigDark;
			PlayerInfo.Instance.CurrCampaignShip = type;
			return true;
		default :
			return false;
		}


	}

	void OnTutorialClick (GameObject go)
	{
		Debug.Log("Tutorial");

		_loadingScreen.Show();
		
		IsTutorial = true;



	
		//create battle param
		UIController.Instance.battle.OrangeTeamPlayersCount = 0;
		UIController.Instance.battle.SupportCount = 1;
		UIController.Instance.battle.BlueTeamPlayersCount = 1;
		PlayerInfo.Instance.shipsLock = false;
		
	//	UIController.Instance.battle.CountFlagNeed = _flagCountField.CurrentValue;
	//	UIController.Instance.battle.CountFragNeed = _flagCountField.CurrentValue;
		
		
		UIController.Instance.setGameMode("base defense");
		
		UIController.Instance.LoadTutorial();
		
		UIController.Instance.SwitchMenuTo(UIController.MenuUiControllsType.TacticScreenMenu);





		//create battle

		GameController.Instance.SetBasicParameters();
		
		_shipsSlots.Clear();
		
		_shipsSlots = UIShipsTacticPanel.Instance.Ships;
		Debug.Log (_shipsSlots.Count);
		Debug.Log(GameController.Instance.CurrentBattle.SupportCount);
		for (int i = 0; i < GameController.Instance.CurrentBattle.SupportCount+1; i++)
		{
			UIController.Instance.blueTeamShipsPreset[i] = (int) GetNewShipType(_shipsSlots[i].Type);
			
			AIPlayer aiPlayer = GameController.Instance.BlueTeamPlayers[i] as AIPlayer;
			
			if (aiPlayer != null){
				Debug.Log("Set AI TACKTICK");
				aiPlayer.Tactic = (AITactic) _shipsSlots[i].shipTactic;
			}
			Debug.Log("INIT BLUE time ships"+_shipsSlots[i].advanceWeapon);
			
			GameController.Instance.BlueTeamPlayers[i].AdvanceWeaponNumber = _shipsSlots[i].aWeaponNumber;
			GameController.Instance.BlueTeamPlayers[i].AdvanceWeapon = _shipsSlots[i].advanceWeapon;
			GameController.Instance.BlueTeamPlayers[i].upgrades = _shipsSlots[i].upgrades;
			
		}
		
		GameController.Instance.StartBattle();



	




	}

	public void OnTacticScreenCampaign(GameObject go)
	{
		int i = PlayerInfo.Instance.CurrentLevel;
		//если до 100 то загружаем уровень компании если больше то ето задание адмирала
		if (i < 100) {
			Debug.Log ("Campaign Created");


			//ограничиваем корабль если нужно
			string type = ConfigCampaign.Levels[i].PlayerShip;
			if(type != "")
			{
				if(SendShipType(type))
				{
					PlayerInfo.Instance.shipsLock = true;
				}
				else {
					PlayerInfo.Instance.shipsLock = false;
				}
			}
			else {
				PlayerInfo.Instance.shipsLock = false;
			}


			//Количество команд
			UIController.Instance.battle.OrangeTeamPlayersCount = ConfigCampaign.Levels [i].EnemiesCounts;
			UIController.Instance.battle.BlueTeamPlayersCount = ConfigCampaign.Levels [i].PlayerShipCount;
			//количество ботов в командах
			GameController.Instance.bootsCount = ConfigCampaign.Levels [i].Bots;	
			/*
			 * 
			TODO: когда будет несколько карт тут можно будет менять их 
			//	UIController.Instance.currentMapsIndex = ConfigCampaign.Levels [i].MapName;
			*/

			UIController.Instance.battle.Mode = ConfigCampaign.Levels [i].MissionType;
		} 
		else {

			i = PlayerInfo.Instance.AdmiralQuest;
			Debug.Log ("Admiral Quest Created");
			//ограничиваем корабль если нужно
			string type = ConfigAdmiral.Quest[i].PlayerShip;
			if(type != "")
			{
				if(SendShipType(type))
				{
					PlayerInfo.Instance.shipsLock = true;
				}
				else {
					PlayerInfo.Instance.shipsLock = false;
				}
			}
			else {
				PlayerInfo.Instance.shipsLock = false;
			}

			//Количество команд
			UIController.Instance.battle.OrangeTeamPlayersCount = ConfigAdmiral.Quest [i].EnemiesCounts;
			UIController.Instance.battle.BlueTeamPlayersCount = ConfigAdmiral.Quest [i].PlayerShipCount;
			//количество ботов в командах
			GameController.Instance.bootsCount = ConfigAdmiral.Quest [i].Bots;	
			GameController.Instance.AdmiralTarget = ConfigAdmiral.Quest[i].MissionTarget;
			GameController.Instance.AdmiralsTargetCount = ConfigAdmiral.Quest[i].MissionValue;
			/*
			 * 
			TODO: когда будет несколько карт тут можно будет менять их 
			//	UIController.Instance.currentMapsIndex = ConfigCampaign.Levels [i].MapName;
			*/
			
			UIController.Instance.battle.Mode = ConfigAdmiral.Quest [i].MissionType;
		
		}



			//TODO::need add condition? that checking type of game and set correct value
			UIController.Instance.battle.CountFlagNeed = _flagCountField.CurrentValue;
	

			UIController.Instance.OnTacticScreen();
			
			UIController.Instance.SwitchMenuTo(UIController.MenuUiControllsType.TacticScreenMenu);
		//ships equip panel



		UIShipsTacticPanel.Instance.isCampaign = true;
		UIShipsTacticPanel.Instance.Show();
	}




	public void OnTacticScreen(GameObject go)
	{
		if (!IsMultiplayer)
		{

			UIController.Instance.battle.OrangeTeamPlayersCount = _orangeCountField.CurrentValue;
			UIController.Instance.battle.SupportCount = BattleConfigurator.SupportCountPlaying;
			UIController.Instance.battle.BlueTeamPlayersCount = _blueCountField.CurrentValue;
			PlayerInfo.Instance.shipsLock = false;

			/*//TODO::need add condition? that checking type of game and set correct value
			switch (UIController.Instance.battle.Mode){
			case GameMode.CaptureTheFlag:
			
				break;
			case GameMode.Deathmatch:
			
			
				break;
			
			}*/
			UIController.Instance.battle.CountFlagNeed = _flagCountField.CurrentValue;
			UIController.Instance.battle.CountFragNeed = _flagCountField.CurrentValue;


			UIController.Instance.setGameMode(_mode[0].CurrentSelection);

			UIController.Instance.OnTacticScreen();

			UIController.Instance.SwitchMenuTo(UIController.MenuUiControllsType.TacticScreenMenu);
		}

		else
		{
			UIController.Instance.BattleDataReset();
			PlayerInfo.Instance.shipsLock = false;
			UIController.Instance.battle.BlueTeamPlayersCount = _blueCountField.CurrentValue;
			UIController.Instance.battle.OrangeTeamPlayersCount = _blueCountField.CurrentValue;
			UIController.Instance.battle.SupportCount = _supportCountField.CurrentValue;

			UIController.Instance.battle.IsPrivateGame = _complexityField.CurrentSelection == "Private game";

			//TODO::need add condition? that checking type of game and set correct value
			UIController.Instance.battle.CountFlagNeed = _flagCountField.CurrentValue;
			UIController.Instance.battle.Mechanics = GameController.Instance.Mechanics;

			UIController.Instance.battle.Map =
				GameController.LoadMap(UIController.Instance.battle.Maps[_mapField.IndexOfCurrentSelection]);

			if (UIController.Instance.battle.Map == null)
			{
				Debug.LogError(string.Format("CreateGameMultiplayerMenu: can't load Map"));
				return;
			}
			Debug.Log ("CREATE MAP");
			GameController.Instance.OnTacticScreenSelected(UIController.Instance.battle, true);
			UIController.Instance.OnCreateRoomButtonClicked();
		}
	}

	private void OnStartBattle(GameObject go)
	{
		Debug.Log ("startBattle click");
		_loadingScreen.Show();

		StartCoroutine(StartBattleWait());
	}

	private void OnStartCompaign(GameObject go)
	{

		if (PlayerInfo.Instance.shipsLock)
		{
		if(UIShipsTacticPanel.Instance.Ships[0].Type != PlayerInfo.Instance.CurrCampaignShip)
			{
				Debug.Log("Ship LOCK");
				UIMessagePanel.Instance.SetMessage("You must play at\n another ship");
				UIMessagePanel.Instance.Show();
			}
			else 
			{
				Debug.Log("Ok");
				_loadingScreen.Show ();
				StartCoroutine (StartBattleWait ());
			}
		
		}
		else
		{
			_loadingScreen.Show ();
			StartCoroutine (StartBattleWait ());
		}
	}
	private IEnumerator StartBattleWait()
	{
		yield return new WaitForSeconds(1.0f);
		StartBattle();
	}
 
	private void StartBattle()
	{

		GameController.Instance.SetBasicParameters();

		_shipsSlots.Clear();

		_shipsSlots = UIShipsTacticPanel.Instance.Ships;
		Debug.Log (_shipsSlots.Count);
		Debug.Log(GameController.Instance.CurrentBattle.SupportCount);
		for (int i = 0; i < GameController.Instance.CurrentBattle.SupportCount+1; i++)
		{
            UIController.Instance.blueTeamShipsPreset[i] = (int) GetNewShipType(_shipsSlots[i].Type);

			AIPlayer aiPlayer = GameController.Instance.BlueTeamPlayers[i] as AIPlayer;

			if (aiPlayer != null){
				Debug.Log("Set AI TACKTICK");
				aiPlayer.Tactic = (AITactic) _shipsSlots[i].shipTactic;
				}
			Debug.Log("INIT BLUE time ships"+_shipsSlots[i].advanceWeapon);

			GameController.Instance.BlueTeamPlayers[i].AdvanceWeaponNumber = _shipsSlots[i].aWeaponNumber;
			GameController.Instance.BlueTeamPlayers[i].AdvanceWeapon = _shipsSlots[i].advanceWeapon;
			GameController.Instance.BlueTeamPlayers[i].upgrades = _shipsSlots[i].upgrades;

		}
	
        GameController.Instance.StartBattle();
	}

	public static ShipType GetNewShipType(ShipsGlobal.ShipType t)
	{
		switch (t)
		{
			case ShipsGlobal.ShipType.Big:
				return ShipType.BigShip;
			case ShipsGlobal.ShipType.Middle:
				return ShipType.Submarine;
			case ShipsGlobal.ShipType.Small:
				return ShipType.Boat;
		case ShipsGlobal.ShipType.SmallMetal:
			return ShipType.SmallMetal;
		case ShipsGlobal.ShipType.MiddleMetal:
			return ShipType.MiddleMetal;
		case ShipsGlobal.ShipType.BigMetal:
			return ShipType.BigMetal;
		case ShipsGlobal.ShipType.SmallAtlant:
			return ShipType.SmallAtlant;
		case ShipsGlobal.ShipType.MiddleAtlant:
			return ShipType.MiddleAtlant;
		case ShipsGlobal.ShipType.BigAtlant:
			return ShipType.BigAtlant;
		case ShipsGlobal.ShipType.SmallDark:
			return ShipType.SmallDark;
		case ShipsGlobal.ShipType.MiddleDark:
			return ShipType.MiddleDark;
		case ShipsGlobal.ShipType.BigDark:
			return ShipType.BigDark;
		}
		return ShipType.Boat;
	}
}
