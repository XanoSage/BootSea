using UnityEngine;
using System.Collections;

public class UIMultiplayerBattleSettingsPanel : BasicPanel<UIMultiplayerBattleSettingsPanel>
{
    public override void Show()
    {
        SoundController.PlayMenuOpen();

        base.Show();
        UITopPanel.Instance.CurrentPanels.Add(Instance);
		UITopPanel.Instance.leftPanelTitleLbl.text =  LocalizationConfig.getText("Battle");
    }

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.MultiplayerBattleSettings;
	}
}
