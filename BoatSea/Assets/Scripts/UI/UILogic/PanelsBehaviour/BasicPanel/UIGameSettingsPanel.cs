using UnityEngine;
using System.Collections;

public class UIGameSettingsPanel : BasicPanel<UIGameSettingsPanel>
{
    public override void Show()
    {
        base.Show();
        UITopPanel.Instance.leftPanelTitleLbl.text = "Options";
    }

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.GameSettings;
	}
}
