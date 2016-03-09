using System.Collections.Generic;
using UnityEngine;

public class UITitlePanel : BasicPanel<UITitlePanel>
{
    public UIEventListener BattleModeBtn;
    public UIEventListener CampaignBtn;
    public UIEventListener MultiplayerBtn;
    public UIEventListener OptionsBtn;
	
    [SerializeField] private GameObject _mapPanel;
    [SerializeField] private GameObject _shipBack;
    [SerializeField] private GameObject _topPanel;

	[SerializeField]
	private GameObject _backPanel;

	[SerializeField]
	private GameObject _cogWheel;
   

    private void Start()
    {
        CampaignBtn.onClick += OnCampaignBtnClick;
        BattleModeBtn.onClick += OnBattleModeBtnClick;
        OptionsBtn.onClick += OnOptionsBtnClick;
        MultiplayerBtn.onClick += OnMultiPlayerBtnClick;
    }

    private void OnCampaignBtnClick(GameObject sender)
    {
        SoundController.PlayMenuClick();

        UIMapPanel.Instance.Show();
        UITopPanel.Instance.Show();
        UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() { Instance });
        Hide();
    }

    private void OnBattleModeBtnClick(GameObject sender)
    {
        SoundController.PlayMenuClick();

        UITopPanel.Instance.Show();
        UIBattleSettingsPanel.Instance.Show();
        UIBattleDetailsPanel.Instance.Show();
       // UIBattleDetailsPanel.Instance.IsMultiplayer = false;
        UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() { Instance });
        Hide();
    }

    private void OnOptionsBtnClick(GameObject sender)
    {
        SoundController.PlayMenuClick();

        UITopPanel.Instance.Show();
        UIGameSettingsPanel.Instance.Show();
        UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() { Instance });
        Hide();
    }

    private void OnMultiPlayerBtnClick(GameObject sender)
    {
        SoundController.PlayMenuClick();

        UIGameList.Instance.Show();
        UITopPanel.Instance.Show();
        UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() { Instance });
        Hide();

		UIController.Instance.OnMultiplayerMenuClicked();
    }

	public override void Hide () {
		_shipBack.SetActive(false);
		gameObject.SetActive(false);

	}

	public void HideAdvance () {
		//_backPanel.SetActive(false);
		_cogWheel.SetActive(false);

	}

    public override void Show()
    {

        SoundController.PlayMenuOpen();
       // print("TitlePanel");
        IsActive = true;
        _shipBack.SetActive(true);
        gameObject.SetActive(true);

		//_backPanel.SetActive(true);
		_cogWheel.SetActive(true);
		
        UITopPanel.Instance.Hide();
    }

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.Title;
	}
}