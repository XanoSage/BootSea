//#define FOREVER_ALONE

using Aratog.NavyFight.Models.Unity3D.Battles;
using UnityEngine;
using System.Collections;

public class StartBattleConfig : MonoBehaviour {

	public static StartBattleConfig Instance { get; private set; }

	private BattleConfigurator battle = new BattleConfigurator();
	private Timer timer1;

	private void Awake () {
		Instance = this;
	}

	void Start () {
#if FOREVER_ALONE
        battle.BlueTeamPlayersCount = 1;
        battle.OrangeTeamPlayersCount = 0;
        battle.SupportCount = 0;
#else
		battle.BlueTeamPlayersCount = 2;
		battle.OrangeTeamPlayersCount = 2;
        battle.SupportCount = 2;
#endif
		battle.Maps = UIController.GetAvailableMapList();
		battle.IsPrivateGame = false;


        // TODO: delete delay before firing StartBattleSequence()
		timer1 = gameObject.AddComponent<Timer>();
		timer1.OnEnd += StartBattleSequence;
		timer1.Launch(0.1f, true);

        // TODO: make it work without delay
        //StartBattleSequence();
	}

	public void StartBattleSequence () {
		GameController.Instance.OnMainMenuButtonClicked(UIController.MenuUIResponceType.BattleFreeButtonClicked);
		UIController.Instance.OnTacticScreenButtonClicked(battle, 0);
		UIController.Instance.OnTacticScreenStartBattle();
	}
}
