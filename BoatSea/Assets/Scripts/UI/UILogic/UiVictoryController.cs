using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Games;


public class UiVictoryController : BasicPanel<UiVictoryController> {

	[SerializeField]
	private UISprite _StateIcon;
	[SerializeField]
	private UILabel _StateLabel;
	[SerializeField]
	private UISprite _BattleTypeIcon;



	private string lose = "defeat_title";
	private string win = "victory_title";

	private string ctf = "ctf";
	private string deathmatch = "deathmatch";
	private string survival = "survival";
	private string timeCtf = "time_ctf";
	private string defence = "base_Defence";

	[SerializeField]
	private UILabel _redScoreLabel;
	[SerializeField]
	private UISprite _redScoreSprite;
	[SerializeField]
	private UILabel _blueScoreLabel;
	[SerializeField]
	private UISprite _blueScoreSprite;
	[SerializeField]
	private UILabel _MoneyLabel;
	[SerializeField]
	private UILabel _Record;
	[SerializeField]
	private UILabel _Timer;

	[SerializeField]
	private UIVictoryPlayerController [] _players;

	[SerializeField]
	private UIEventListener _NextBtn;




	[SerializeField]
	private  float Timer;


	void Start()
	{
		_NextBtn.onClick += VictoryClose;
	}

	void Update()
	{
		Timer += Time.deltaTime;
	}

	void VictoryClose (GameObject go)
	{
		Application.LoadLevel(GameController.MainSceneName);
	}

	private int MatchScore()
	{
		int i = 1;


		return i;
	}

	private void BattleTypeIcon()
	{
		if(BattleController.Instance.ActiveBattle.Mode == GameMode.CaptureTheFlag)
		{
			_StateLabel.text = "Capture Flag";
			_BattleTypeIcon.spriteName = ctf;
		}
		else if(BattleController.Instance.ActiveBattle.Mode == GameMode.Deathmatch)
		{
			_StateLabel.text = "Deathmatch";
			_BattleTypeIcon.spriteName = deathmatch;
		}
		else if(BattleController.Instance.ActiveBattle.Mode == GameMode.BaseDefense)
		{
			_StateLabel.text = "Base Defence";
			_BattleTypeIcon.spriteName = deathmatch;
		}
		else if(BattleController.Instance.ActiveBattle.Mode == GameMode.Survival)
		{
			_StateLabel.text = "Survival";
			_BattleTypeIcon.spriteName = survival;
		}
	}

	public override void Show()
	{

		//Меняем надпись Выиграш / проиграш
		if (BattleController.Instance.IsWin) {
			_StateIcon.spriteName = win;
		} else {
			_StateIcon.spriteName = lose;
		}



		BattleTypeIcon ();



		//Показываем время
		float total = (float)Timer / 60;
		int minutes = (int)total;
		int seconds = (int)((total - Mathf.Floor(total)) * 60);
		_Timer.text = String.Format("{0}:{1}", minutes, seconds);


		_Record.text = MatchScore ().ToString ();

		//Показываем статистику кораблей за бой
		int counter = 0;
		List<Player> playersFleet = GameSetObserver.Instance.Human.PlayersFleet;


		//активируем иконки кораблей команды
		foreach (Player player in playersFleet)
		{
			UIVictoryPlayerController shipItem = _players[counter];
			shipItem.gameObject.SetActive(true);
			shipItem.Show(player);
			counter++;
		}

		base.Show ();



	}
		
}
