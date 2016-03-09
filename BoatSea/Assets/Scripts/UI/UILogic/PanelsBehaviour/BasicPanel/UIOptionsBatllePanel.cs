using UnityEngine;
using System.Collections;

public class UIOptionsBatllePanel : BasicPanel<UIOptionsBatllePanel> {

	// Use this for initialization
	void Start () {
	
	}


	public override void Show()
	{
		base.Show();
		UITopPanel.Instance.leftPanelTitleLbl.text = "Options";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
