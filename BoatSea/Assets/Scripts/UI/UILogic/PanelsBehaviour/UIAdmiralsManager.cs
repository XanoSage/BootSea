using UnityEngine;
using System.Collections;

public class UIAdmiralsManager : MonoBehaviour {
	[SerializeField]
	private UIEventListener _select; 
	[SerializeField]
	private UIEventListener _takeGift;
	[SerializeField]
	private UILabel _questLabel; 

	[SerializeField]
	private GameObject _levelChooser; 

	// Use this for initialization
	void Start () {

		if (PlayerInfo.Instance.AdmiralQuestComplet == false) {

			_takeGift.gameObject.SetActive(false);
			if(PlayerInfo.Instance.AdmiralQuestGiven){
				OnSelect(gameObject);

			}
			else {
				_select.gameObject.SetActive(true);
			}
			_select.onClick += OnSelect;
			ChangeQuestText ();

		} 
		else {
			_questLabel.text = "Congratuation\n Take the Gift";
			_takeGift.gameObject.SetActive(true);
			_select.gameObject.SetActive(false);
				_takeGift.onClick +=OnGift;
		}

	}

	private void OnGift(GameObject sender)
	{
		PlayerInfo.Instance.AdmiralQuestComplet = false;
		UIMapPanel.Instance.AdmiralLevelSwitch = 100;
		PlayerInfo.Instance.AdmiralQuestGiven = false;
		PlayerInfo.Instance.AdmiralQuest = PlayerInfo.Instance.AdmiralQuest+ 1;
		ResetAdmiralPanel ();
	}

	private void ResetAdmiralPanel()
	{
		_takeGift.gameObject.SetActive(false);
		if(PlayerInfo.Instance.AdmiralQuestGiven){
			OnSelect(gameObject);
			
		}
		else {
			_select.gameObject.SetActive(true);
		}
		_select.onClick += OnSelect;
		ChangeQuestText ();

	}

	private void ChangeQuestText()
	{
		int currLevel = PlayerInfo.Instance.AdmiralQuest;
	
		string map = ConfigAdmiral.Quest [currLevel].MapName;
		string mission = ConfigAdmiral.Quest [currLevel].MissionType.ToString ();
		string missionvalue = ConfigAdmiral.Quest [currLevel].MissionValue.ToString();
		string playerTeam = ConfigAdmiral.Quest [currLevel].PlayerShipCount.ToString();
		string playerShip = ConfigAdmiral.Quest [currLevel].PlayerShip;
		string enemiesTeam = ConfigAdmiral.Quest [currLevel].EnemiesCounts.ToString();
		string kill  =  ConfigAdmiral.Quest [currLevel].MissionTarget +  ConfigAdmiral.Quest [currLevel].MissionValue;
		_questLabel.text = map +"\n"+ mission +"("+missionvalue+")"+"\n"+"kill : "+kill+"\nPlayer team: "+playerTeam+"\nPlayer Ship: "+ playerShip+
			"\nEnemies Team:"+enemiesTeam;
	}

	private void OnSelect(GameObject sender)
	{
		int currLevel = PlayerInfo.Instance.AdmiralQuest;

		_select.gameObject.SetActive (false);



		// TODO:изменить логику активации следующего квеста
		//уровень который перекрываем +1
		UIMapPanel.Instance.AdmiralLevelSwitch = ConfigAdmiral.Quest [currLevel].CampaignLevel ;
		//задаем позицыю стрелочки -1 из за разницы в нумеровании
		_levelChooser.transform.position = UIMapPanel.Instance.Levels [currLevel-1].transform.position;


		//сообщаем о взятии квеста
		PlayerInfo.Instance.AdmiralQuestGiven = true;

	}


}
