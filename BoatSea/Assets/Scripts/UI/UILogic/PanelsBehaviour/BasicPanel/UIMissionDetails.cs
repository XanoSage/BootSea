using UnityEngine;
using System.Collections.Generic;

public class UIMissionDetails : BasicPanel<UIMissionDetails> {
	
	public UIEventListener NextBtn;
	public GameObject[] quests;
	public UISprite [] questIcons;
	public UILabel [] questLabels;
	public GameObject buttonNext;
	public GameObject buttonPlay;
	void Start () {
	

	NextBtn.onClick+=OnNextBtnClick;

	}
	void OnNextBtnClick(GameObject sender){
		Debug.Log("click play Campign");
	 	buttonNext.SetActive (false);
		buttonPlay.SetActive (true);
	//	UIShipsTacticPanel.Instance.isCampaign = true;
	//	UIShipsTacticPanel.Instance.Show();
		UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() {UIMapPanel.Instance});
	//	UIMapPanel.Instance.Hide();
	}

	public override void Hide ()
	{
		base.Hide ();

	}

	public override void Show ()
	{
		base.Show ();
		buttonNext.SetActive (true);
		buttonPlay.SetActive (false);
		SetLevelQuest ();
		//UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() { UIMapPanel.Instance});
	}


	public void SetLevelQuest()
	{

		int currLevel = PlayerInfo.Instance.CurrentLevel;

		string map = "1";
		string mission ="1";
		string missionvalue ="1";
		string playerTeam = "1";
		string playerShip = "";
		string enemiesTeam = "";
		string kill = "";
		Debug.Log (currLevel);

		if (currLevel < 100) {

			quests [0].SetActive (true);
			map = ConfigCampaign.Levels [currLevel].MapName;
			mission = ConfigCampaign.Levels [currLevel].MissionType.ToString ();
			missionvalue = ConfigCampaign.Levels [currLevel].MissionValue.ToString ();
		 	playerTeam = ConfigCampaign.Levels [currLevel].PlayerShipCount.ToString ();
			playerShip = ConfigCampaign.Levels [currLevel].PlayerShip;
			enemiesTeam = ConfigCampaign.Levels [currLevel].EnemiesCounts.ToString ();
			kill = "Them all";
		} else {
			//+1 нужен из за разницы нумерования dictionary и уровня 
			int admiralLevel = PlayerInfo.Instance.AdmiralQuest;
			quests [0].SetActive (true);
			 map = "**ADMIRAL QUEST**\n"+ConfigAdmiral.Quest [admiralLevel].MapName;
			 mission = ConfigAdmiral.Quest [admiralLevel].MissionType.ToString ();
			 missionvalue = ConfigAdmiral.Quest [admiralLevel].MissionValue.ToString();
			 playerTeam = ConfigAdmiral.Quest [admiralLevel].PlayerShipCount.ToString();
			 playerShip = ConfigAdmiral.Quest [admiralLevel].PlayerShip;
			 enemiesTeam = ConfigAdmiral.Quest [admiralLevel].EnemiesCounts.ToString();
			kill  =  ConfigAdmiral.Quest [admiralLevel].MissionTarget +  ConfigAdmiral.Quest [admiralLevel].MissionValue;
		}

		questLabels[0].text =map +"\n"+ mission +"("+missionvalue+")"+"\n"+"kill "+kill+"\nPlayer team: "+playerTeam+"\nPlayer Ship: "+ playerShip+
			"\nEnemies Team:"+enemiesTeam;//LocalizationConfig.getText(text);
		
	
		}

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.Mission;
	}
}
