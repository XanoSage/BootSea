using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class UIGameList : BasicPanel<UIGameList>
{

	#region Variables

	[SerializeField] private UITweener _filtersBlock;
	[SerializeField] private UITweener _gamesListBlock;
	[SerializeField] private UITweener _buttonsBlock;

	public UIEventListener RandomBtn;
	public UIEventListener CreateGameBtn;
	public UIEventListener JoinBtn;

	[SerializeField] public List<UIMultiplayerGameInfo> GameInfos;

	private RoomInfo currentRoom = null;

	[HideInInspector] 
	public bool IsBlockAction;

	private int gameInfosCount;

	#endregion

	#region MonoBehaviour events

	private void Start()
	{
		RandomBtn.onClick += OnRandomBtnClick;
		CreateGameBtn.onClick += OnCreateGameBtnClick;
		JoinBtn.onClick += OnJoinBtnClick;

		RandomBtn.GetComponent<UIButton>().isEnabled = false;
		JoinBtn.GetComponent<UIButton>().isEnabled = false;

		IsBlockAction = false;

		DeactivateList();

		if (GameInfos != null)
		{
			gameInfosCount = GameInfos.Count;
		}
	}

	private void Update()
	{

	}

	#endregion


	#region Override events

	public override void Show()
	{
        SoundController.PlayMenuOpen();

       IsActive = true;
		_filtersBlock.enabled = true;
		_filtersBlock.Play(true);
		_gamesListBlock.enabled = true;
		_gamesListBlock.Play(true);
		_buttonsBlock.enabled = true;
		_buttonsBlock.Play(true);
		UITopPanel.Instance.leftPanelTitleLbl.text = "Game List";
		UITopPanel.Instance.CurrentPanels.Add(UIGameList.Instance);
		// UITopPanel.Instance.ChangeState(TopPanelState.GameList);

		JoinBtn.GetComponent<UIButton>().isEnabled = false;
		RandomBtn.GetComponent<UIButton>().isEnabled = false;

		CreateGameBtn.GetComponent<UIButton>().isEnabled = true;

		DeactivateList();

		MultiplayerManager.Instance.TryingConnectToPhoton();
		UIController.Instance.UpdateRoomInfo();

		IsBlockAction = false;
	}

	public override void Hide()
	{
		IsActive = false;
		_filtersBlock.Play(false);
		_gamesListBlock.Play(false);
		_buttonsBlock.Play(false);
	}

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.GameList;
	}

	#endregion

	#region events

	private void OnRandomBtnClick(GameObject sender)
	{
		PhotonNetwork.JoinRandomRoom();

		//RandomBtn.GetComponent<UIButton>().enabled = false;
		//JoinBtn.GetComponent<UIButton>().enabled = false;

		OnRoomConnecting();

		/*Hide();
        UILobbyPanel.Instance.Show();*/
	}

	private void OnCreateGameBtnClick(GameObject sender)
	{
		Hide();

		//UIControllerForNGUI.Instance.OnTacticScreen(sender);
		//UIBattleDetailsPanel.Instance.OnNextBtnClick(sender);

		UIMultiplayerBattleSettingsPanel.Instance.Show();
		UIBattleDetailsPanel.Instance.Show();
		UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() {Instance});
		//UIBattleDetailsPanel.Instance.IsMultiplayer = true;
	}

	private RoomInfo GetSelcetedRoom()
	{
		RoomInfo room = null;

		for (int i = 0; i < GameInfos.Count; i++)
		{
			if (GameInfos[i].IsChecked)
			{
				room = GameInfos[i].Room;
				break;
			}
		}

		return room;
	}

	private void OnRoomConnecting()
	{
		Debug.Log("UIGameList.OnRoomConnecting: disabled button when trying to connect to room");
		RandomBtn.GetComponent<UIButton>().isEnabled = false;
		JoinBtn.GetComponent<UIButton>().isEnabled = false;
		CreateGameBtn.GetComponent<UIButton>().isEnabled = false;

		currentRoom = null;

		IsBlockAction = true;
	}

	private void OnJoinBtnClick(GameObject sender)
	{
		if (IsBlockAction)
			return;

		//PhotonNetwork.JoinRandomRoom();
		RoomInfo room = GetSelcetedRoom();

		if (room != null)
		{
			PhotonNetwork.JoinRoom(room);
		}
		else
		{
			return;
		}

		OnRoomConnecting();

		//Hide();
		//UILobbyPanel.Instance.Show();
		//UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() {Instance});
	}

	public void OnJoinBtnClick()
	{
		IsBlockAction = false;

		Hide();
		UILobbyPanel.Instance.Show();
		UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() {Instance});
	}

	public void OnRoomInfoUpdate(List<RoomInfo> roomInfos)
	{
		ResetSelection();
		//DeactivateList();
		if (roomInfos.Count > 0)
		{
			//JoinBtn.GetComponent<UIButton>().isEnabled = true;
			RandomBtn.GetComponent<UIButton>().isEnabled = true;
		}
		else
		{
			JoinBtn.GetComponent<UIButton>().isEnabled = false;
			RandomBtn.GetComponent<UIButton>().isEnabled = false;
		}

		int roomCount = roomInfos.Count > gameInfosCount ? gameInfosCount : roomInfos.Count;

		for (int i = 0; i < roomCount; i++)
		{
			GameInfos[i].gameObject.SetActive(true);
			GameInfos[i].UpdateGameInfo(roomInfos[i], i);
			if (currentRoom != null)
			{
				if (Equals(GameInfos[i].Room, currentRoom))
				{
					GameInfos[i].IsChecked = true;
					JoinBtn.GetComponent<UIButton>().isEnabled = true;
				}
			}
		}

		for (int i = roomCount; i < GameInfos.Count; i++)
		{
			GameInfos[i].gameObject.SetActive(false);
		}

	}

	public void OnStateChange(bool state)
	{
		Debug.Log("OnStateChange: checkbox status change");

		if (state)
		{
			JoinBtn.GetComponent<UIButton>().isEnabled = true;

			currentRoom = GetSelcetedRoom();
		}
	}

	public void DeactivateList()
	{
		foreach (UIMultiplayerGameInfo gameInfo in GameInfos)
		{
			gameInfo.gameObject.SetActive(false);
		}
	}

	private void ResetSelection()
	{
		foreach (UIMultiplayerGameInfo gameInfo in GameInfos)
		{
			gameInfo.Reset();

		}
	}

	#endregion
}
