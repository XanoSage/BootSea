using System;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Extensions;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static Camera Camera;

	public Camera raycastCamera;

	public static UIManager Instance { get; private set; }
	
#region Current/Previous panels Feature
	public UIMenuInterfaceControllsType CurrentMenu, PreviousMenu;
	
	void SetDefaultValues()
	{
		CurrentMenu =  UIMenuInterfaceControllsType.Title;// UITitlePanel.Instance.MenuType;
		PreviousMenu = UIMenuInterfaceControllsType.None;
		History.Add(CurrentMenu);
	}

	void SwitchPanel(UIMenuInterfaceControllsType type)
	{
		if (type == CurrentMenu)
			return;

		HistoryID++;
		PreviousMenu = CurrentMenu;
		CurrentMenu = type;
		TraceHistory(CurrentMenu);
	}
#endregion
	
#region History Feature
	public int HistoryID = 0;
	public List<UIMenuInterfaceControllsType> History;

	[SerializeField]
	int _poolLimit = 4;
	
	void TraceHistory(UIMenuInterfaceControllsType type)
	{
		if (HistoryID + 1 <= History.Count)
			return;
		History.Add(type);
		if (History.Count > _poolLimit)
			History.RemoveAt(0);
	}
#endregion
	
	void Start() {
		Instance = this;

		Camera = transform.root.GetComponentInChildren<UICamera>().GetComponent<Camera>();
		//TODO::remove this
        if (!GameObject.Find("Panels"))
		    return; 

		SetDefaultValues();

#region TopPanel Subscriptions

		if (UITopPanel.Instance != null)
		{
			UITopPanel.Instance.backBtn.onClick += OnBackButton;
			UITopPanel.Instance.moreCoinsBtn.onClick += OnMoreCoinsButton;
		}

		#endregion

#region Title Subscriptions

		if (UITitlePanel.Instance != null)
		{
			UITitlePanel.Instance.OptionsBtn.onClick += OnOptionsButton;
			UITitlePanel.Instance.MultiplayerBtn.onClick += OnMultiplayerButton;
			UITitlePanel.Instance.BattleModeBtn.onClick += OnBattleButton;
			UITitlePanel.Instance.CampaignBtn.onClick += OnCampaignButton;
		}

		#endregion

#region Multiplayer Subscriptions

		if (UIGameList.Instance != null)
		{
			UIGameList.Instance.CreateGameBtn.onClick += OnCreateGameButton;
			UIGameList.Instance.JoinBtn.onClick += OnJoinButton;
		}

		if (UILobbyPanel.Instance != null)
			UILobbyPanel.Instance.SetupSuppotBtn.onClick += OnSetupTeamButton;

		if (UIBattleDetailsPanel.Instance != null)
			UIBattleDetailsPanel.Instance.NextBtn.onClick += OnNextBattleButton;

		#endregion
	}


	public static void OnBattleStart () {
        if (UITitlePanel.Instance)
        {
            UITitlePanel.Instance.Hide();
            UITitlePanel.Instance.HideAdvance();
        }
	}


#region TopPanel Methods
	public void OnBackButton(GameObject sender)
	{
		if (UIGameList.Instance.IsBlockAction)
					return;

		switch (CurrentMenu)
		{
			case UIMenuInterfaceControllsType.Lobby:
				UIController.Instance.OnBackButtonInLobbyCreatingGameMenuClicked();
				GameController.Instance.ClearHumanPlayersFleet();
				UILobbyPanel.Instance.UpdateBattleUiInfo();
				break;

			case UIMenuInterfaceControllsType.MultiplayerBattleSettings:
				//Если мы уже создали комнату или подсоединились к комнате 
				if (PhotonNetwork.room != null)
				{
					PhotonNetwork.LeaveRoom();
				}

				UIController.Instance.OnBackButtonMultiplayeMenuClicked();

				//TODO:: temprorary for test
				if (GameController.Instance.CurrentBattle != null && GameController.Instance.CurrentBattle.IsBattleCreated)
				{
					GameController.Instance.RestartBasicData();
				}

				UIController.Instance.BattleDataReset();

				break;
			case UIMenuInterfaceControllsType.GameList:
				

				break;

		}

		HistoryID--;

		if (HistoryID < 0)
			HistoryID = 0;

		if (History.Count < 1)
		{
			HistoryID = 0;
		}

		if (HistoryID > History.Count - 1)
			HistoryID = History.Count - 1;


		Utils.Swap(ref CurrentMenu, ref PreviousMenu);
		CurrentMenu = History[HistoryID];
	}
	
	void OnMoreCoinsButton(GameObject sender)
	{
		SwitchPanel(UIMenuInterfaceControllsType.MoreCoinsPopUp);
	}
#endregion

#region Title Methods
	void OnOptionsButton(GameObject sender)
	{
        SwitchPanel(UIMenuInterfaceControllsType.GameSettings);
	}
	
	void OnMultiplayerButton(GameObject sender)
	{
		SwitchPanel(UIMenuInterfaceControllsType.GameList);
		UIController.Instance.OnMainMenuButtonClicked(UIController.MenuUIResponceType.MultiplayerMenuButtonClicked);
	}
	
	void OnBattleButton(GameObject sender)
	{
		SwitchPanel(UIMenuInterfaceControllsType.BattleSettings);
		UIController.Instance.OnMainMenuButtonClicked(UIController.MenuUIResponceType.BattleFreeButtonClicked);
	}

	void OnCampaignButton(GameObject sender)
	{
		SwitchPanel(UIMenuInterfaceControllsType.Map);
		UIController.Instance.OnMainMenuButtonClicked(UIController.MenuUIResponceType.CampaignMenuButtonClicked);
	}

	public void OnShipSelectionBtn(GameObject sender)
	{
		SwitchPanel(UIMenuInterfaceControllsType.ShipSelection);
	}
#endregion
	
#region Multiplayer Methods
	void OnCreateGameButton(GameObject sender)
	{
		SwitchPanel(UIMenuInterfaceControllsType.MultiplayerBattleSettings);
		UIController.Instance.OnMainMenuButtonClicked(UIController.MenuUIResponceType.MultiplayerMenuButtonClicked);
	}
	
	void OnJoinButton(GameObject sender)
	{
		SwitchPanel(UIMenuInterfaceControllsType.Lobby);
	}

	public void OnNextBattleButton(GameObject sender)
	{
		if (UIBattleDetailsPanel.Instance.IsMultiplayer)
			SwitchPanel(UIMenuInterfaceControllsType.Lobby);
		else
		{
			SwitchPanel(UIMenuInterfaceControllsType.ShipsSettings);
		}
	}

	void OnSetupTeamButton(GameObject sender)
	{
		SwitchPanel(UIMenuInterfaceControllsType.ShipsSettings);
	}
#endregion
}