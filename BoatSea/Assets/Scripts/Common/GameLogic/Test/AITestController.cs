using System.Collections.Generic;
using System.Globalization;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Base;
using Aratog.NavyFight.Models.Unity3D.Battles;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Ship;
using Assets.Scripts.Common.GameLogic;
using LinqTools;
using UnityEngine;
using System.Collections;

public class AITestController : MonoBehaviour, IGameController {

	#region Variables
	public static AITestController Instance { get; private set; }

	public GameType CurrentGameType { get; set;}

	public List<Player> Players { get; set; }

	public Player Human { get; set; }
	public List<Player> BlueTeamPlayers { get; set; }
	public List<Player> RedTeamPlayers { get; set; }

	public Battle CurrentBattle { get; set; }

	public bool IsBattleStarted { get; set; }

	public bool IsPause { get; set; }
	
	public MechanicsType Mechanics { get; set; }
	public Player GetPlayer(int playerId)
	{
		return Players.FirstOrDefault(player => player.Id == playerId);
	}

	private AITactic _aiTactic;

	public UIEventListener StartButton;

	public UIEventListener AddBlueBoatButton;
	public UIEventListener AddRedBoatButton;

	public UIEventListener SetRandomPointButton;

	public UIEventListener SetBlueFlagDroppedButton;
	public UIEventListener SetOrangeFlagDroppedButton;

	public UIEventListener SetRedBaseAlarmButton;

	public UIEventListener StartBlueFlagEffectButton;

	public UIEventListener AddBlueBigShipButton;
	public UIEventListener AddRedBigShipButton;

	[SerializeField] private UIEventListener _addRedSubmarine;
	[SerializeField] private UIEventListener _addBlueSubmarine;
	
	public event GameController.OnStartEventHandler OnStartBattle;
	public event GameController.OnBattleEndEventHandler OnEndBattle;

	public UILabel AlarmTextLabel;
	public UILabel AlarmCounterLabel;
	public UILabel AIPlayerStateLabel;

	private AIPlayer aiPlayer;

	private string aiPlayerStateText = "AIPlayerState:{0}";

	private FlagSpotBehaviour _flagSpotBehaviour;

	public UIEventListener AddBluePlayerButton;
	public GameObject HUDRoot;


	[SerializeField] private UIEventListener _chanhgeAiTacticButton;
	[SerializeField] private UILabel _tacticLabel;

	#endregion


	#region MonoBehaviour functions

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start ()
	{

		Players = new List<Player>();

		SubscribeEvents();

		SetActivityTestUiElements(false);	

		AlarmCounterLabel.gameObject.SetActive(false);
		AlarmTextLabel.gameObject.SetActive(false);

		_aiTactic = AITactic.CaptureEnemy;
		_tacticLabel.text = _aiTactic.ToString();
	}
	
	// Update is called once per frame
	void Update () {


		if (_flagSpotBehaviour != null)
		{
			if (_flagSpotBehaviour.Base.State == BaseState.Alarm)
			{
				if (AlarmCounterLabel.gameObject.activeSelf)
				{
					int minutes = (int)_flagSpotBehaviour.AlarmCounter/60;
					int seconds = (int) _flagSpotBehaviour.AlarmCounter%60;
					AlarmCounterLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
				}
			}
		}

		if (aiPlayer != null)
		{
			AIPlayerStateLabel.text = string.Format(aiPlayerStateText, aiPlayer.MyShip.StateOfShipBehaviour);
		}

		if (Human != null)
		{
			Submarine submarine = Human.MyShip as Submarine;

			if (submarine != null)
				AIPlayerStateLabel.text = string.Format(aiPlayerStateText, Human.MyShip.StateOfShipBehaviour);
		}
	}

	#endregion

	#region function

	private void SubscribeEvents()
	{
		StartButton.onClick = OnStart;

		AddBlueBoatButton.onClick = OnAddBlueBoatButton;

		AddRedBoatButton.onClick = OnAddRedBoatButton;

		SetRandomPointButton.onClick = OnSetRandomPointButtton;

		SetBlueFlagDroppedButton.onClick = OnSetBlueFlagDroppedButton;
		SetOrangeFlagDroppedButton.onClick = OnSetRedFlagDropedButton;

		SetRedBaseAlarmButton.onClick = TurnOnRedBaseAlarm;

		StartBlueFlagEffectButton.onClick = OnStartBlueFlagEffect;

		AddBlueBigShipButton.onClick = OnAddBlueBigShipButton;
		AddRedBigShipButton.onClick = OnAddRedBigShipButton;

		AddBluePlayerButton.onClick = OnAddBluePlayerButton;

		_addBlueSubmarine.onClick = OnAddBlueSubmarine;
		_addRedSubmarine.onClick = OnAddRedSubmarine;

		_chanhgeAiTacticButton.onClick = OnChangeAiTacticButtonClick;
	}

	private void SetActivityTestUiElements(bool isAactive)
	{
		AddRedBoatButton.gameObject.SetActive(isAactive);
		AddBlueBoatButton.gameObject.SetActive(isAactive);

		AddBlueBigShipButton.gameObject.SetActive(isAactive);
		AddRedBigShipButton.gameObject.SetActive(isAactive);

		SetRandomPointButton.gameObject.SetActive(isAactive);

		SetBlueFlagDroppedButton.gameObject.SetActive(isAactive);
		SetOrangeFlagDroppedButton.gameObject.SetActive(isAactive);

		SetRedBaseAlarmButton.gameObject.SetActive(isAactive);

		StartBlueFlagEffectButton.gameObject.SetActive(isAactive);

		AIPlayerStateLabel.gameObject.SetActive(isAactive);

		AddBluePlayerButton.gameObject.SetActive(isAactive);

		_addBlueSubmarine.gameObject.SetActive(isAactive);
		_addRedSubmarine.gameObject.SetActive(isAactive);

		_tacticLabel.gameObject.SetActive(isAactive);

		_chanhgeAiTacticButton.gameObject.SetActive(isAactive);
	}

	public void OnStart(GameObject sender)
	{
		if (IsBattleStarted)
			return;

		IsBattleStarted = true;

		StartButton.gameObject.SetActive(false);

		SetCurrentBattleParameters();

		SetGameType();

		SetActivityTestUiElements(true);

		if (OnStartBattle != null)
			OnStartBattle();

		aiPlayer = null;

		//IsBattleStarted = true;
	}

	private void SetCurrentBattleParameters()
	{
		BattleConfigurator battle = new BattleConfigurator
			{
				Mode = GameMode.CaptureTheFlag,
				Map = null,
				Maps = UIController.GetAvailableMapList(),
				BlueTeamPlayersCount = 4,
				OrangeTeamPlayersCount = 4,
				SupportCount = 2,
				Difficulty = DifficultyType.Normal,
				TimeNeed = 3,
				TimeSpent = 0,
				CountFlagNeed = 2,
				CountFragNeed = 2,
				IsPrivateGame = false,
				Mechanics = MechanicsType.Classic
			};

		battle.Map = GameController.LoadMap(battle.Maps[0]);
	
		CurrentBattle = new Battle(battle);

		Mechanics = CurrentBattle.Mechanics;

		Player.Mechanics = Mechanics;

	}

	private void SetGameType()
	{
		CurrentGameType = GameType.BattleFree;
	}

	private void OnAddBlueBoatButton(GameObject sender)
	{
		if (!IsCanAddPlayerToScene())
			return;

		int rnd = Random.Range(0, 1000);

		//AddAiBoatToScene(TeamColor.BlueTeam, rnd % 2 == 0 ? AITactic.BaseDefence : AITactic.CaptureEnemy);
		AddAiBoatToScene(TeamColor.BlueTeam, _aiTactic);

		Debug.Log("AITestController.OnAddBlueBoatButton: add blue boat");
	}

	private void OnAddRedBoatButton(GameObject sender)
	{
		if (!IsCanAddPlayerToScene())
			return;

		Debug.Log("AITestController.OnAddRedBoatButton: add red boat");

		int rnd = 0;//Random.Range(0, 1000);
		
		//AddAiBoatToScene(TeamColor.OrangeTeam, rnd % 2 == 0 ? AITactic.BaseDefence : AITactic.CaptureEnemy);
		AddAiBoatToScene(TeamColor.OrangeTeam, _aiTactic);
	}

	private void OnAddBlueBigShipButton(GameObject sender)
	{
		if (!IsCanAddPlayerToScene())
			return;

		int rnd = Random.Range(0, 1000);

		//AddAiBigShipToScene(TeamColor.BlueTeam, rnd % 1 == 0 ? AITactic.BaseDefence : AITactic.CaptureEnemy);
		AddAiBigShipToScene(TeamColor.BlueTeam, _aiTactic);
	}

	private void OnAddRedBigShipButton(GameObject sender)
	{
		if (!IsCanAddPlayerToScene())
			return;

		int rnd = Random.Range(0, 1000);

		//AddAiBigShipToScene(TeamColor.OrangeTeam, rnd % 1 == 0 ? AITactic.BaseDefence : AITactic.CaptureEnemy);
		AddAiBigShipToScene(TeamColor.OrangeTeam, _aiTactic);
	}

	private bool IsCanAddPlayerToScene()
	{
		return Players != null && Players.Count < CurrentBattle.MaxCountPlayers;
	}

	private void AddAiBoatToScene(TeamColor team, AITactic tactic = AITactic.CaptureEnemy)
	{
		AddAiShipToScene(team, ShipType.Boat, tactic);
	}

	private void AddAiBigShipToScene(TeamColor team, AITactic tactic = AITactic.BaseDefence)
	{
		AddAiShipToScene(team, ShipType.BigShip, tactic);
	}

	private void AddAiSubmarineToScene(TeamColor team, AITactic tactic = AITactic.BaseDefence)
	{
		AddAiShipToScene(team, ShipType.Submarine, tactic);
	}

	private void AddAiShipToScene(TeamColor team, ShipType shipType, AITactic tactic)
	{
		Player player = CreatePlayer(team, shipType, PlayerType.AIPlayer, tactic);

		player.Id = Players.Count;

		Players.Add(player);

		ResourceBehaviourController.Instance.InitShip(player);

		BattleController.Instance.SetBattleDataToPlayer(player);

		OnAddShipToScene(player);

		AIPlayer aiPlayer1 = player as AIPlayer;
		//if (aiPlayer1 != null && !aiPlayer1.IsAnyShipInMyFleetTakeEnemyFlag) 
		//	aiPlayer1.StartAiAdvanced();

		if (CameraFollowsShip.Instance != null && CameraFollowsShip.Instance.Target == null)
		{
			CameraFollowsShip.Instance.Target = BattleController.Instance.ships[0];
			aiPlayer = aiPlayer1;
		}
	}

	private void OnAddRedSubmarine(GameObject sender)
	{
		Debug.Log("AITestController.OnAddRedSubmarine - OK");

		AddAiSubmarineToScene(TeamColor.OrangeTeam, _aiTactic);
	}

	private void OnAddBlueSubmarine(GameObject sender)
	{
		Debug.Log("AITestController.OnAddBlueSubmarineClick - OK");

		AddAiSubmarineToScene(TeamColor.BlueTeam, _aiTactic);
	}

	private void OnAddShipToScene(Player player)
	{
		if (player.Team == TeamColor.BlueTeam)
		{
			if (BlueTeamPlayers == null)
				BlueTeamPlayers = new List<Player>();

			BlueTeamPlayers.Add(player);
			player.AllPlayerInMyTeam = BlueTeamPlayers; 
		}

		else
		{
			if (RedTeamPlayers == null)
				RedTeamPlayers = new List<Player>();

			RedTeamPlayers.Add(player);
			player.AllPlayerInMyTeam = RedTeamPlayers;
		}

	}

	private Player CreatePlayer(TeamColor team, ShipType shipType, PlayerType playerType, AITactic tactic = AITactic.CaptureEnemy)
	{
		Player player = Player.CreatePlayer(playerType, true, team);

		player.PlayersFleet = new List<Player> {player};

		player.MyShip = Ship.CreateShip(shipType);
		Debug.Log ("AI CREATED SHIP");
		player.MyShip.Owner = player;

		player.Init();

		AIPlayer aiPlayer = player as AIPlayer;

		if (aiPlayer != null)
		{
			aiPlayer.Tactic = tactic;
		}

		return player;
	}


	private void OnSetRandomPointButtton(GameObject sender)
	{
		ShipBehaviour ship = BattleController.Instance.ships[0];

		if (ship != null)
		{
			AIPlayer aiPlayer = ship.Player as AIPlayer;
			if (aiPlayer != null)
			{
				Vector3 testPosition = new Vector3(-37, 0, -12);
				testPosition = Map.GetWorldPosition(CurrentBattle.Map, Map.GetMapPosition(CurrentBattle.Map, testPosition));
				if (aiPlayer.Position == aiPlayer.GoalPosition)
				{
					aiPlayer.SetTargetPosition(Map.GetRandomPositionOnMap(CurrentBattle.Map));
				}
				else
					//aiPlayer.SetTargetPosition(testPosition);
					aiPlayer.SetTargetPosition(aiPlayer.GoalPosition);
				//aiPlayer.SetTargetPosition(Map.GetRandomPositionOnMap(CurrentBattle.Map));
			}
		}
	}

	private void OnSetBlueFlagDroppedButton(GameObject sender)
	{
		SetFlagDropped(TeamColor.BlueTeam);
	}

	private void OnSetRedFlagDropedButton(GameObject sender)
	{
		SetFlagDropped(TeamColor.OrangeTeam);
	}

	private void SetFlagDropped(TeamColor color)
	{
		FlagsBehaviour flags = color == TeamColor.BlueTeam
			                       ? BattleController.Instance.BlueFlag
			                       : BattleController.Instance.OrangeFlag;

		flags.SetFlagDropped();
	}

	private void TurnOnRedBaseAlarm(GameObject sender)
	{
		_flagSpotBehaviour = BattleController.Instance.OrangeFlagSpot;

		_flagSpotBehaviour.Base.OnAlarmOnEvent += TurnOnBaseAlarm;
		_flagSpotBehaviour.Base.OnAlarmOffEvent += TurnOffBaseAlarm;

		_flagSpotBehaviour.Base.AlarmOn();
	}

	private void TurnOnBaseAlarm()
	{
		SetAlarmTextActivity(true);

		if (_flagSpotBehaviour != null)
		{
			//_flagSpotBehaviour.AlarmOn();
		}
	}

	private void SetAlarmTextActivity(bool isActive)
	{
		if (AlarmCounterLabel != null && AlarmTextLabel != null)
		{
			AlarmCounterLabel.gameObject.SetActive(isActive);
			AlarmTextLabel.gameObject.SetActive(isActive);
		}
	}

	private void TurnOffBaseAlarm()
	{
		SetAlarmTextActivity(false);

		if (_flagSpotBehaviour != null)
		{
			_flagSpotBehaviour.Base.OnAlarmOnEvent -= TurnOnBaseAlarm;
			_flagSpotBehaviour.Base.OnAlarmOffEvent -= TurnOffBaseAlarm;

			//_flagSpotBehaviour.AlarmOff();
			_flagSpotBehaviour = null;
		}
		
	}

	private void OnStartBlueFlagEffect(GameObject sender)
	{
		BattleController.Instance.BlueFlagSpot.ShowFlagEffect(BattleController.Instance.BlueFlag);
	}

	private void OnAddBluePlayerButton(GameObject sender)
	{
		AddHumanShipToScene(TeamColor.OrangeTeam, ShipType.Boat, AITactic.BaseDefence);
		
		HUDRoot.SetActive(true);
	}

	private void AddHumanShipToScene(TeamColor team, ShipType shipType, AITactic tactic)
	{
		Player player = CreatePlayer(team, shipType, PlayerType.HumanPlayer,tactic);

		player.Id = Players.Count;

		Players.Add(player);

		Human = player;

		ResourceBehaviourController.Instance.InitShip(player);

		BattleController.Instance.SetBattleDataToPlayer(player);

		OnAddShipToScene(player);
		

		if (CameraFollowsShip.Instance != null && CameraFollowsShip.Instance.Target == null)
		{
			CameraFollowsShip.Instance.Target = BattleController.Instance.ships[0];
		}
	}

	private void OnChangeAiTacticButtonClick(GameObject sender)
	{
		Debug.Log("AItestController.OnChangeAiTacicButtonClick - OK");
		_aiTactic = _aiTactic == AITactic.BaseDefence ? AITactic.CaptureEnemy : AITactic.BaseDefence;

		_tacticLabel.text = _aiTactic.ToString();
	}

	#endregion
}
