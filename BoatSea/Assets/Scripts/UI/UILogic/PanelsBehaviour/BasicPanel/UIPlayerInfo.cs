using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Ship;
using Assets.Scripts.Common.GameLogic.Multiplayer;
using UnityEngine;
using System.Collections;

public class UIPlayerInfo : MonoBehaviour
{

	private const string ReadyText = "Ready";
	private const string NotReadyText = "Not Ready";

	public MultiplayerEntity PlayerEntity;

	public UITexture AvatarIco;

	public UILabel PlayerNameLabel;

	public UICheckbox IsReadyButton;

	public List<UISprite> ShipsIcon;

	public TeamColor Color;

	public void UpdatePlayerInfo()
	{
		
		PlayerNameLabel.text = PlayerEntity.photonPlayer.name;

		MultiplayerPlayer player = PlayerEntity.player as MultiplayerPlayer;

		if (player.MyShip == null)
		{
			Debug.Log("UIPlayerInfo.UpdatePlayerInfo(): players - ship not created");


			IsReadyButton.isChecked = false;
			return;
		}

		IsReadyButton.isChecked = player.IsReady;

		for (int i = 0; i < player.PlayersFleet.Count; i++)
		{
			Player plFleet = player.PlayersFleet[i];
			ShipsIcon[i].gameObject.SetActive(true);
			UIAtlas atlas = ShipsIcon[i].atlas;

			string shipIcon = ResourceBehaviourController.GetShipIcon(plFleet.Team, plFleet.MyShip.Type);

			ShipsIcon[i].spriteName = shipIcon;

			ShipsIcon[i].UpdateUVs(false);

			//ShipsIcon[i] = NGUITools.AddSprite(ShipsIcon[i].transform.parent.gameObject, atlas, shipIcon);
		}

		for (int i = player.PlayersFleet.Count; i < ShipsIcon.Count; i++)
		{
			ShipsIcon[i].gameObject.SetActive(false);
		}
	}

	public void ResetShipIcon()
	{
		for (int i = 0; i < ShipsIcon.Count; i++)
		{
			ShipsIcon[i].gameObject.SetActive(false);
		}
	}

	// Use this for initialization
	private void Start()
	{


		IsReadyButton.onStateChange += OnStateChange;
		IsReadyButton.onStateChange += UILobbyPanel.Instance.ActivateStartButton;

		foreach (UISprite uiSprite in ShipsIcon)
		{
			uiSprite.gameObject.SetActive(false);
		}
	}

	// Update is called once per frame
	private void Update()
	{

	}

	private void OnStateChange(bool state)
	{
		Debug.Log(string.Format("Player: {0}, trying set is ready", PlayerEntity.photonPlayer.name));
		UILabel text = IsReadyButton.transform.GetComponentInChildren<UILabel>();

		if (MultiplayerManager.MyMultiplayerEntity == PlayerEntity)
		{
			MultiplayerPlayer mpPlayer = PlayerEntity.player as MultiplayerPlayer;
			if (mpPlayer == null || mpPlayer.MyShip == null)
			{
				Debug.LogError("Player is not multiplayer player");
				IsReadyButton.isChecked = false;


				if (text != null)
				{
					text.text = IsReadyButton.isChecked ? ReadyText : NotReadyText;
				}
				return;
			}
			else
				mpPlayer.SetIsReady(IsReadyButton.isChecked);

		}
		else
		{
			MultiplayerPlayer mpPlayer = PlayerEntity.player as MultiplayerPlayer;
			if (mpPlayer == null || mpPlayer.MyShip == null)
			{
				Debug.LogError("Player is not multiplayer player");
				IsReadyButton.isChecked = false;

				if (text != null)
				{
					text.text = IsReadyButton.isChecked ? ReadyText : NotReadyText;
				}

				return;
			}
			else
				IsReadyButton.isChecked = mpPlayer.IsReady;
		}

		//UILabel text = IsReadyButton.transform.GetComponentInChildren<UILabel>();

		if (text != null)
		{
			text.text = IsReadyButton.isChecked ? ReadyText : NotReadyText;
		}
	}
}
