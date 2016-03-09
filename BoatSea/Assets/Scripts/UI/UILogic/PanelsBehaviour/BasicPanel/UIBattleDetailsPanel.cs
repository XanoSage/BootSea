using System.Collections.Generic;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Battles;
using UnityEngine;
using System.Collections;

public class UIBattleDetailsPanel : BasicPanel<UIBattleDetailsPanel> {

	#region Constants

	private const string HiglightText = "Highlight";

	private const string CaptureTheFlagIcon = "ctf";

	#endregion

	#region Variables
	[SerializeField] public UIEventListener NextBtn;

	[SerializeField] public UIEventListener RailsControlsButton;

	[SerializeField] public UIEventListener FreeControlsButton;

	[SerializeField] private UISprite _battleIcon;

	private UISprite railsControlHighlight;
	private UISprite freeControlHighlight;

	private bool IsRailsActive;

	#endregion

	#region MonoBehaviour event

	void Start()
    {
        //NextBtn.onClick += OnNextBtnClick;

		//TODO:: add additional checking for NewWave version, Classic, or full version of the game
	    if (RailsControlsButton != null)
	    {
		    railsControlHighlight = RailsControlsButton.transform.FindChild(HiglightText).GetComponent<UISprite>();
			RailsControlsButton.onClick = OnRailsControlButton;
	    }

		if (FreeControlsButton != null)
		{
			freeControlHighlight = FreeControlsButton.transform.FindChild(HiglightText).GetComponent<UISprite>();
			FreeControlsButton.onClick = OnFreeControlButton;
		}
		OnRailsControlButton(gameObject);
		OnFreeControlButton(gameObject);
    }

	#endregion

	#region events

	public bool IsMultiplayer
	{
		get { return GameController.Instance.CurrentGameType == GameType.Multiplayer; }
	}

    public void OnNextBtnClick(GameObject sender)
    {
	    if (IsMultiplayer)
		{Debug.Log("Click multiplayer");
            UILobbyPanel.Instance.Show();
		    
			//TODO:: Only for PlayableDemo
			//UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() {UIGameList.Instance});
			UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() {Instance, UIMultiplayerBattleSettingsPanel.Instance});
		    Hide();
		    UIMultiplayerBattleSettingsPanel.Instance.Hide();

	    }
	    else
	    {
			Debug.Log("Click else");
			UIShipsTacticPanel.Instance.isCampaign = false;
		    UIShipsTacticPanel.Instance.Show();
		    UITacticMapPanel.Instance.Show();
		    UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() {Instance, UIBattleSettingsPanel.Instance});
		    Hide();
		    UIBattleSettingsPanel.Instance.Hide();
	    }
    }

	private void OnRailsControlButton(GameObject sender)
	{
		if (IsRailsActive) 
			return;

		IsRailsActive = true;
			
		if (railsControlHighlight != null)
			railsControlHighlight.gameObject.SetActive(true);

		if (freeControlHighlight)
			freeControlHighlight.gameObject.SetActive(false);

		GameSetObserver.Instance.Mechanics = MechanicsType.Classic;

		//else
		//{
		//	IsRailsActive = false;
			
		//	if (railsControlHighlight != null)
		//		railsControlHighlight.gameObject.SetActive(false);
		//}
	}

	private void OnFreeControlButton(GameObject sender)
	{
		if (!IsRailsActive) 
			return;

		IsRailsActive = false;

		if (railsControlHighlight != null)
			railsControlHighlight.gameObject.SetActive(false);

		if (freeControlHighlight != null)
			freeControlHighlight.gameObject.SetActive(true);

		GameSetObserver.Instance.Mechanics = MechanicsType.NewWave;
	}

	#endregion

	#region Override events

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.BattleDetails;
	}

	public override void Show()
    {
        //if (IsActive) return;
        base.Show();
        //UITopPanel.Instance.rightPanelTitle.SetActive(false);

		BattleConfigurator battle = UIController.Instance.battle;

		switch (battle.Mode)
		{
			case GameMode.CaptureTheFlag:
				_battleIcon.spriteName = CaptureTheFlagIcon;
				break;
		}

	}
	#endregion
}
