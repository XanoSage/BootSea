using Aratog.NavyFight.Models.Games;
using UnityEngine;
using System.Collections;

public class UITacticMapPanel : BasicPanel<UITacticMapPanel>
{
	public UIEventListener StartButton;

	public UIEventListener SetupButton;

	private bool isCampaign = false;

	private bool IsMultiplayer
	{
		get { return GameController.Instance != null && GameController.Instance.CurrentGameType == GameType.Multiplayer; }
	}

	void Start()
	{
		if (SetupButton != null)
			SetupButton.onClick += OnSetupButton;
	}

    public override void Show()
    {
		Debug.Log ("TacticmapShow");
		isCampaign = false;
        //print("TacticmapShow");
        base.Show();
        UITopPanel.Instance.rightPanelTitle.SetActive(true);
		UITopPanel.Instance.rightPanelTitleLbl.text =  LocalizationConfig.getText("Tactic map");

		if (IsMultiplayer)
		{
			if (StartButton != null)
				StartButton.gameObject.SetActive(false);

			if (SetupButton != null)
				SetupButton.gameObject.SetActive(true);
		}
		else
		{
			if (StartButton != null)
				StartButton.gameObject.SetActive(true);
			
			if (SetupButton != null)
				SetupButton.gameObject.SetActive(false);
		}

    }




    public override void Hide()
    {
        //print("TacticmapHide");
        base.Hide();
       UITopPanel.Instance.rightPanelTitle.SetActive(false);
    }

	private void OnSetupButton(GameObject sender)
	{
		//Hide();
		UITopPanel.Instance.backBtn.onClick(sender);
	}
}
