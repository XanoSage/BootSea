using System.Collections.Generic;
using LinqTools;
using System.Security.Cryptography.X509Certificates;
using Aratog.NavyFight.Models.Unity3D.Players;
using Assets.Scripts.Common.GameLogic.Multiplayer;
using UnityEngine;
using System.Collections;

public class UILobbyPanel : BasicPanel<UILobbyPanel>
{
    [SerializeField]
    private UITweener _blueTeamBlock;
    [SerializeField]
    private UITweener _redTeamBlock;
    [SerializeField]
    private UITweener _buttonsBlock;
    [SerializeField]
    private UITweener _headerBlock;

    public UIEventListener ChangeTeamBtn;
    public UIEventListener StartBtn;
    public UIEventListener SetupSuppotBtn;

	//Battle info variables

	public UISprite BattleTypeIcon;
	public UILabel BattleTypeLabel;
	public UILabel BattleMapLabel;

	public UILabel BattleParametersLabel;


	//Blue team variables
	public Transform BlueTeamScoreTablet;
	public UILabel BlueTeamScoreLabel;

	public UITable BlueTeamPlayerTable;

	public List<UIPlayerInfo> BlueTeamPlayers;

	//Orange team variables
	public Transform OrangeTeamScoreTablet;
	public UILabel OrangeTeamScoreLabel;

	public UITable OrangeTeamPlayerTable;

	public List<UIPlayerInfo> OrangeTeamPlayers;

 
	void Start()
	{
		BlueTeamPlayers = new List<UIPlayerInfo>();
		OrangeTeamPlayers = new List<UIPlayerInfo>();

		StartBtn.GetComponent<UIButton>().isEnabled = false;

		StartBtn.onClick += OnStartGame;
		SetupSuppotBtn.onClick += OnTeamSetup;
		ChangeTeamBtn.onClick += OnChangeTeam;

		OrangeTeamPlayers = new List<UIPlayerInfo>();
		BlueTeamPlayers = new List<UIPlayerInfo>();

		UIPlayerInfo [] playersInfo = gameObject.GetComponentsInChildren<UIPlayerInfo>();

		if (playersInfo == null)
			return;

		foreach (UIPlayerInfo playerInfo in playersInfo)
		{
			switch (playerInfo.Color)
			{
				case TeamColor.OrangeTeam:
					OrangeTeamPlayers.Add(playerInfo);
					break;
				case TeamColor.BlueTeam:
					BlueTeamPlayers.Add(playerInfo);
					break;
				default:
					continue;
			}


			playerInfo.gameObject.SetActive(false);
		}

		//if (OrangeTeamPlayers == null || BlueTeamPlayers == null)
		//	return;
		
		//if (OrangeTeamPlayers.Count == 0 || BlueTeamPlayers.Count == 0 || BlueTeamPlayers.Count != OrangeTeamPlayers.Count)
		//	return;

		//for (int i = 0; i < BlueTeamPlayers.Count; i++)
		//{
		//	OrangeTeamPlayers[i].gameObject.SetActive(false);
		//	BlueTeamPlayers[i].gameObject.SetActive(false);
		//}
	}


    public override void Show()
    {
        SoundController.PlayMenuOpen();

        IsActive = true;
        _blueTeamBlock.enabled = true;
        _blueTeamBlock.Play(true);
        _redTeamBlock.enabled = true;
        _redTeamBlock.Play(true);
        _buttonsBlock.enabled = true;
        _buttonsBlock.Play(true);
        _headerBlock.enabled = true;
        _headerBlock.Play(true);

	    //UISprite sprite = NGUITools.AddSprite();

	    //sprite.GetAtlasSprite();

        UITopPanel.Instance.CurrentPanels.Add(Instance);
        UITopPanel.Instance.leftPanelTitleLbl.text = "Lobby";

		StartBtn.gameObject.SetActive(false);

		if (MultiplayerManager.IsMasterClient)
			StartBtn.gameObject.SetActive(true);

		UpdateBattleUiInfo();
    }

    public override void Hide()
    {
        IsActive = false;
        _blueTeamBlock.Play(false);
        _redTeamBlock.Play(false);
        _buttonsBlock.Play(false);
        _headerBlock.Play(false);
    }

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.Lobby;
	}

	public void UpdateBattleUiInfo()
	{
		//TODO:: need add condition when this score table need displaying
		BlueTeamScoreTablet.gameObject.SetActive(false);
		OrangeTeamScoreTablet.gameObject.SetActive(false);

		if (GameController.Instance.CurrentBattle == null || GameController.Instance.CurrentBattle.Map == null)
			return;

		BattleParametersLabel.text = UIController.Instance.battle.CountFlagNeed.ToString();

		BattleMapLabel.text = GameController.Instance.CurrentBattle.Map.Name;

		BattleTypeLabel.text = GameController.Instance.CurrentBattle.Map.Type.ToString();


		//BlueTeamPlayerTable.gameObject.SetActive(false);
		//OrangeTeamPlayerTable.gameObject.SetActive(false);

		List<MultiplayerEntity> blueEntities =
			MultiplayerManager.MultiplayerEntities.FindAll(entity => entity.player.Team == TeamColor.BlueTeam);

		List<MultiplayerEntity> orangeEntities =
			MultiplayerManager.MultiplayerEntities.FindAll(entity => entity.player.Team == TeamColor.OrangeTeam);

		if (blueEntities.Count > 0)
		{
			for (int i = 0; i < blueEntities.Count; i++)
			{
				BlueTeamPlayers[i].gameObject.SetActive(true);
				BlueTeamPlayers[i].PlayerEntity = blueEntities[i];
				BlueTeamPlayers[i].UpdatePlayerInfo();
			}


		}

		for (int i = blueEntities.Count; i < BlueTeamPlayers.Count; i++)
		{
			BlueTeamPlayers[i].ResetShipIcon();
			BlueTeamPlayers[i].gameObject.SetActive(false);
		}

		//for (int i = blueEntities.Count; i < BlueTeamPlayers.Count; i++)
		//{

		//	BlueTeamPlayers[i].gameObject.SetActive(false);
		//}

		if (orangeEntities.Count > 0)
		{
			for (int i = 0; i < orangeEntities.Count; i++)
			{
				OrangeTeamPlayers[i].gameObject.SetActive(true);
				OrangeTeamPlayers[i].PlayerEntity = orangeEntities[i];
				OrangeTeamPlayers[i].UpdatePlayerInfo();
			}


		}

		for (int i = orangeEntities.Count; i < OrangeTeamPlayers.Count; i++)
		{
			OrangeTeamPlayers[i].ResetShipIcon();
			OrangeTeamPlayers[i].gameObject.SetActive(false);
		}

		//for (int i = orangeEntities.Count; i < OrangeTeamPlayers.Count; i++)
		//{
		//	OrangeTeamPlayers[i].gameObject.SetActive(false);
		//}

		if (!MultiplayerManager.IsMasterClient) 
			return;

		StartBtn.gameObject.SetActive(true);

		ActivateStartButton(false);
	}

	public void ActivateStartButton(bool state)
	{
		if (!MultiplayerManager.IsMasterClient) 
			return;

		if (UIController.IsPlayersReadyToStart(MultiplayerManager.MultiplayerEntities))
		{
			StartBtn.GetComponent<UIButton>().isEnabled = true;
		}
		else
		{
			StartBtn.GetComponent<UIButton>().isEnabled = false;
		}
	}

	public void OnChangeTeam(GameObject go)
	{
		MultiplayerManager.Instance.OnChangePlayerTeamColor(MultiplayerManager.MyMultiplayerEntity);
	}

	public void OnStartGame(GameObject go)
	{
//		GameController.Instance.StartBattle();
		StartCoroutine(MultiplayerManager.Instance.PreStartBattle(false));
		Debug.Log("UILobbyPanel.OnStartGame: start game");
	}

	public void OnTeamSetup(GameObject go)
	{
		MultiplayerPlayer mpPlayer = MultiplayerManager.MyMultiplayerEntity.player as MultiplayerPlayer;
		if (mpPlayer == null)
		{
			Debug.LogError("OnTeamSetup: player is null");
		}
		else
		{
			//mpPlayer.SetIsReady(false);
			//UIController.Instance.SetDefaultPlayerTeamShipsPreset();

			UIShipsTacticPanel.Instance.Show();
		    UITacticMapPanel.Instance.Show();
		    UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() {Instance/*, UIBattleSettingsPanel.Instance*/});
		    Hide();
			//UIMultiplayerBattleSettingsPanel.Instance.Hide();
		    //UIBattleSettingsPanel.Instance.Hide();

		}

		Debug.Log("Team setup clicked");
	}

	public void SetPlayerIsReady(MultiplayerEntity entity, bool isReady)
	{
		UIPlayerInfo playerInfo = entity.player.Team == TeamColor.BlueTeam
			                          ? BlueTeamPlayers.FirstOrDefault(players => players.PlayerEntity == entity)
			                          : OrangeTeamPlayers.FirstOrDefault(players => players.PlayerEntity == entity);

		if (playerInfo == null)
		{
			Debug.Log("UILobbyPanel.SetPlayerIsReady: can't find necessary player entity");
			return;
		}

		playerInfo.IsReadyButton.isChecked = isReady;
		UpdateBattleUiInfo();
	}
}