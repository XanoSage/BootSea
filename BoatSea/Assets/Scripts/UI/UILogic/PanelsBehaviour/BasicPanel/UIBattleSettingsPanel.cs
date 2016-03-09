using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class UIBattleSettingsPanel : BasicPanel<UIBattleSettingsPanel>
{
	void Start () {
		//SetMenuBasicData();
	}

    public override void Show()
    {
        //if (IsActive) return;
	
        base.Show();
		UITopPanel.Instance.leftPanelTitleLbl.text =  LocalizationConfig.getText("Battle");
    }

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.BattleSettings;
	}

	/*public void SetMenuBasicData () {
		UIChooser chooser = transform.GetChild(0).FindChild("chooser").GetComponent<UIChooser>();

		if (chooser == null) {
			Debug.LogError("Cannot find object: chooser");
			return;
		}

		List<string> ddd = new List<string>();

		ddd.Add("asdasd1");
		ddd.Add("asdasd2");
		ddd.Add("asdasd3");
		ddd.Add("asdasd4");
		ddd.Add("asdasd5");

		chooser.SetOptions(ddd);
	}*/
}
