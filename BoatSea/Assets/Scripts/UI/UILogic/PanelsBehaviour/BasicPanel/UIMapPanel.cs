using UnityEngine;
using System.Collections;
using System;

public class UIMapPanel : BasicPanel<UIMapPanel> {
	public GameObject [] Levels;

	public int AdmiralLevelSwitch;

	public override void Show(){
		base.Show ();

		int higest = PlayerInfo.Instance.HighestLevel;

		for(int i=higest;i<Levels.Length;i++)
		{

			Levels[i].SendMessage("Hide");
		}

		for(int i=0;i<higest;i++)
		{
			Levels[i].GetComponent<UIEventListener>().onClick += OnLevel;
			Levels[i].SendMessage("Show");
		}

		Debug.Log ("Campaign map");
		UITopPanel.Instance.leftPanelTitleLbl.text=  LocalizationConfig.getText("Campaign");

		UITopPanel.Instance.rightPanelTitle.SetActive (false);
	}

	public override void Hide(){
		base.Hide();

		for(int i=0;i<Levels.Length;i++)
		{
			Levels[i].GetComponent<UIEventListener>().onClick -= OnLevel;
			
		}
	}


	private void OnLevel(GameObject sender)
	{
		string text = sender.name;
		text = text.Trim (new char[]{'l','e','v','e','l'});
		int i = Int32.Parse (text);

		//если уровень перекрыт заданием адмирала то ставим текущий уровень 100
		if (AdmiralLevelSwitch+1 == i) {
			PlayerInfo.Instance.CurrentLevel = 100;

		} else {
			PlayerInfo.Instance.CurrentLevel = i;
		}

		UIMissionDetails.Instance.Show(); 
		
	}


	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.Map;
	}
}
