using System;
using System.Collections.Generic;
using LinqTools;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Players;
using Assets.Scripts.Common.GameLogic.Multiplayer;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using Aratog.NavyFight.Models.Unity3D.Extensions;

public class MultiplayerManager : Photon.MonoBehaviour
{

	#region Variables

	public static MultiplayerManager Instance { get; private set; }

	private bool connectFailed;

	//TODO:: for test only
	public string privateRoomName;

	[HideInInspector] public string roomName = "myRoom";

	public static MultiplayerEntity MyMultiplayerEntity;

	public static List<MultiplayerEntity> MultiplayerEntities = new List<MultiplayerEntity>();

	public static bool IsMasterClient
	{
		get
		{
			return MyMultiplayerEntity != null && MyMultiplayerEntity.photonPlayer != null &&
			       MyMultiplayerEntity.photonPlayer.isMasterClient;
		}
	}

	#endregion

	#region MonoBehaviour events

	private void Avake()
	{

	}

	private void Start()
	{
		Instance = this;
		roomName = "myRoom" + Random.Range(0, 1000); //TODO:: remove this comment + Guid.NewGuid();
		privateRoomName = "";

		MyMultiplayerEntity = new MultiplayerEntity {photonPlayer = PhotonNetwork.player, player = null, playerId = -1};

		MyMultiplayerEntity.photonPlayer.name = "Player" + Random.Range(0, 1000);

	}

	private void Update()
	{

	}

	private void FixedUpdate()
	{

	}

	#endregion

	#region Photon standart events

	public void TryingConnectToPhoton()
	{
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings("1.0");
		}
	}

	// We have two options here: we either joined(by title, list or random) or created a room.
	private void OnJoinedRoom()
	{
		Debug.Log("We have joined a room.");

		//TODO::for test Only. Remove after testing

		if (!PhotonNetwork.player.isMasterClient && PhotonNetwork.room != null)
		{
			DataBuffer buffer =
				new DataBuffer(
					(string) PhotonNetwork.room.customProperties[UIController.Instance.RoomPropertyList[UIController.BattleDataIndex]]);
			UIController.Instance.SetUIBattle(buffer);
		}

		if (MultiplayerEntities.Count < 1)
		{
			DataBuffer playerBuffer = null;

			GameController.Instance.CreateHumanPlayers();

			if (MyMultiplayerEntity.player != null)
			{
				playerBuffer = new DataBuffer();
				MyMultiplayerEntity.player.Serialize(playerBuffer);
			}


			photonView.RPC("AddPlayerToList", PhotonTargets.All, PhotonNetwork.player,
			               playerBuffer != null ? playerBuffer.Bytes : null);
		}
		else
			foreach (MultiplayerEntity plEntity in MultiplayerEntities)
			{
				DataBuffer playerBuffer = null;

				if (plEntity.player != null)
				{
					playerBuffer = new DataBuffer();
					plEntity.player.Serialize(playerBuffer);

				}

				photonView.RPC("AddPlayerToList", PhotonNetwork.player, plEntity.photonPlayer,
				               playerBuffer != null ? playerBuffer.Bytes : null);

				if (plEntity.player != null && plEntity.player.MyShip != null)
				{
					SendPlayerGameInfoData(plEntity.photonPlayer, plEntity.player.PlayersFleet);
				}
			}

		UIController.Instance.OnPlayerJoinedToGame();

		if (!IsMasterClient /*UIManager.Instance.CurrentMenu == UIMenuInterfaceControllsType.GameList*/)
		{
			UIGameList.Instance.OnJoinBtnClick();
		}

		UILobbyPanel.Instance.UpdateBattleUiInfo();

		StartCoroutine(MoveToGameScene());
	}

	private void OnCreatedRoom()
	{
		Debug.Log("We have created a room.");
		//When creating a room, OnJoinedRoom is also called, so we don't have to do anything here.
	}


	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected: " + player);

		foreach (MultiplayerEntity plEntity in MultiplayerEntities)
		{
			DataBuffer playerBuffer = null;

			if (plEntity.player != null)
			{
				playerBuffer = new DataBuffer();
				plEntity.player.Serialize(playerBuffer);
			}

			photonView.RPC("AddPlayerToList", player, plEntity.photonPlayer, playerBuffer != null ? playerBuffer.Bytes : null);
			if (plEntity.player != null && plEntity.player.MyShip != null)
			{
				SendPlayerGameInfoData(plEntity.photonPlayer, plEntity.player.PlayersFleet);
			}
		}

		UILobbyPanel.Instance.UpdateBattleUiInfo();
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPlayerDisconnected: " + player);
		photonView.RPC("RemovePlayerFromList", PhotonTargets.All, player);
	}


	private void OnFailedToConnectToPhoton(object parameters)
	{
		this.connectFailed = true;
		Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
	}

	private IEnumerator MoveToGameScene()
	{
		//Wait for the 
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}

		Debug.LogWarning("Normally we would load the game scene right now.");
		/*
            PhotonNetwork.LoadLevel( LEVEL TO LOAD);
         */

	}

	//CALLBACKS - for debug info only

	// ROOMS

	private void OnLeftRoom()
	{
		Debug.Log("This client has left a game room. " + MyMultiplayerEntity.photonPlayer.ToString());
		if (MultiplayerEntities.Count > 1 && PhotonNetwork.connected)
			photonView.RPC("RemovePlayerFromList", PhotonTargets.All, MyMultiplayerEntity.photonPlayer);
		MultiplayerEntities.Clear();
		UITopPanel.Instance.backBtn.onClick(gameObject);
	}

	private void OnPhotonCreateRoomFailed()
	{
		Debug.Log("A CreateRoom call failed, most likely the room name is already in use.");
	}

	private void OnPhotonJoinRoomFailed()
	{
		Debug.Log("A JoinRoom call failed, most likely the room name does not exist or is full.");
	}

	private void OnPhotonRandomJoinFailed()
	{
		Debug.Log("A JoinRandom room call failed, most likely there are no rooms available.");
	}

	public void OnMasterClientSwitched(PhotonPlayer player)
	{
		Debug.Log("OnMasterClientSwitched: " + player);
	}

	// LOBBY EVENTS

	private void OnJoinedLobby()
	{
		Debug.Log("We joined the lobby.");
	}

	private void OnLeftLobby()
	{
		Debug.Log("We left the lobby.");
	}

	// ROOMLIST

	private void OnReceivedRoomList()
	{
		Debug.Log("We received a new room list, total rooms: " + PhotonNetwork.GetRoomList().Length);
	}

	private void OnReceivedRoomListUpdate()
	{
		UIController.Instance.UpdateRoomInfo();
		Debug.Log("We received a room list update, total rooms now: " + PhotonNetwork.GetRoomList().Length);
	}

	#endregion

	#region Events

	public static MultiplayerEntity GetMultiplayerEntity(PhotonPlayer player)
	{
		return MultiplayerEntities.FirstOrDefault(entity => player != null && Equals(entity.photonPlayer, player));
	}

	public static MultiplayerEntity GetMultiplayerEntity(Player player)
	{
		return MultiplayerEntities.FirstOrDefault(entity => player != null && Equals(entity.player, player));
	}

	public void OnUpdatePlayerTeamColor(MultiplayerEntity entity)
	{
		if (entity.player == null)
			return;

		if (IsMasterClient)
		{
			photonView.RPC("UpdatePlayerTeamColor", PhotonTargets.Others, entity.photonPlayer, (int) entity.player.Team);
		}
		else 
		{
			photonView.RPC("UpdatePlayerTeamColor", PhotonTargets.MasterClient, entity.photonPlayer, (int) entity.player.Team);
		}
	}

	public void OnChangePlayerTeamColor(MultiplayerEntity entity)
	{
		if (entity.player == null)
			return;

		if (IsMasterClient)
		{
			GameController.Instance.OnPlayerChangeTeamColor(entity.player);
			photonView.RPC("UpdatePlayerTeamColor", PhotonTargets.All, entity.photonPlayer, (int) entity.player.Team);
		}
		else
		{
			photonView.RPC("ChangePlayerTeamColor", PhotonTargets.MasterClient, entity.photonPlayer);
		}
	}

	/// <summary>
	/// Отправляем информацию о игроке его кораблях и об кораблях ИИ
	/// </summary>
	/// <param name="photonPlayer"></param>
	/// <param name="playersFleet"></param>
	public void SendPlayerGameInfoData(PhotonPlayer photonPlayer, List<Player> playersFleet)
	{

		DataBuffer buffer = new DataBuffer();

		buffer.Write(playersFleet.Count);

		for (int i = 0; i != playersFleet.Count; i++)
		{
			buffer.Write((int) playersFleet[i].MyShip.Type);

			if (playersFleet[i] is AIPlayer)
				buffer.Write((int) (playersFleet[i] as AIPlayer).Tactic);
			else
			{
				buffer.Write(-1);
			}
		}

		photonView.RPC("SetupPlayerGameInfoData", PhotonTargets.All, photonPlayer, buffer.Bytes);
	}

	/// <summary>
	/// Устанавливает/сбрасывает значение готовности для игроков по сети
	/// </summary>
	/// <param name="isReady"></param>
	public void SetPlayerIsReady(bool isReady)
	{

		MultiplayerPlayer multiplayerPlayer = MyMultiplayerEntity.player as MultiplayerPlayer;

		if (multiplayerPlayer == null)
		{
			Debug.LogError("SetPlayerIsReady: multiplayer is null");
			return;
		}

		photonView.RPC("SetupPlayerIsReady", PhotonTargets.All, MyMultiplayerEntity.photonPlayer, multiplayerPlayer.IsReady);
		
	}


	/// <summary>
	/// Передаём начало быитвы для всех игроков
	/// </summary>
	public void StartBattle()
	{
		photonView.RPC("LetStartTheBattle", PhotonTargets.All, MyMultiplayerEntity.photonPlayer);
	}

	public void ResetMultiplayer()
	{
		for (int i = 0; i != MultiplayerEntities.Count; i++)
		{
			RemovePlayerFromList(MultiplayerEntities[i].photonPlayer);
			i--;
		}
	}

	public void LeaveActiveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	/// <summary>
	/// Запрос со стороны клиентов к мастер-клиенту чтобы синхронизировать Id всех игроков и плееров
	/// </summary>
	public void RequestForPlayerIdSynchronize()
	{
		if (IsMasterClient)
			return;

		photonView.RPC("SynchronizePlayerId", PhotonTargets.MasterClient, MyMultiplayerEntity.photonPlayer);
	}

	/// <summary>
	/// Взорвать нужный снаряд
	/// </summary>
	/// <param name="shellOwner"></param>
	/// <param name="viewID"></param>
	public void NeedBlowUpShell(PhotonPlayer shellOwner, int viewID)
	{
		photonView.RPC("ShellBlowUp", PhotonTargets.All, shellOwner, viewID);
	}


	/// <summary>
	/// Передаём по сети урон который мы нанесли плавсредству
	/// </summary>
	/// <param name="shipOwner"></param>
	/// <param name="damage"></param>
	/// <param name="shellOwnerId"></param>
	public void NeedShipDealDamage(Player shipOwner, int damage, int shellOwnerId)
	{

		Player player = GameController.GetCaptain(shipOwner);

		if (player == null)
		{
			Debug.LogError(string.Format("NeedShipDealDamage: Player with Id:{0}, not found on list", shipOwner.Id));
			return;
		}

		photonView.RPC("ShipTakeDamage", PhotonTargets.All, MyMultiplayerEntity.photonPlayer, /* entity.photonPlayer,*/
		               shipOwner.Id, damage, shellOwnerId);
	}

	public void NeedPlayerFireBasic(int playerId)
	{
		photonView.RPC("PlayerFireBasic", PhotonTargets.All, MyMultiplayerEntity.photonPlayer, playerId);
	}

	public void NeedPlayerFireBomb(int playerId)
	{
		photonView.RPC("PlayerFireBomb", PhotonTargets.All, MyMultiplayerEntity.photonPlayer, playerId);
		Debug.Log(string.Format("from {1}: Player {0} is fired bomb", playerId, MyMultiplayerEntity.photonPlayer));
	}

	public void NeedFlagSpotTakeFlag(TeamColor team, int shipId)
	{
		photonView.RPC("FlagSpotTakeFlag", PhotonTargets.All, MyMultiplayerEntity.photonPlayer, (int) team, shipId);
	}

	public void NeedFlagsTakeFlag(TeamColor team, int shipId)
	{
		photonView.RPC("FlagsTakeFlag", PhotonTargets.All, MyMultiplayerEntity.photonPlayer, (int) team, shipId);
	}

	public void NeedEndBattle(GameMode mode, TeamColor team)
	{
		photonView.RPC("EndBattle", PhotonTargets.All, MyMultiplayerEntity.photonPlayer, (int) mode, (int) team);
	}

	#endregion

	#region RPC functions

	#region Basic Manager functions

	[RPC]
	private void AddPlayerToList(PhotonPlayer phPlayer, byte[] playerBuffer = null)
	{
		if (MultiplayerEntities.FirstOrDefault(player => Equals(player.photonPlayer, phPlayer)) != null)
			return;

		if (Equals(PhotonNetwork.player, phPlayer))
			MultiplayerEntities.Add(MyMultiplayerEntity);
		else
		{
			MultiplayerEntity entity = new MultiplayerEntity {photonPlayer = phPlayer, playerId = 0};

			if (playerBuffer != null)
			{
				Player remotePlayer = GameController.CreatePlayer(new DataBuffer(playerBuffer), entity);
				entity.player = remotePlayer;
			}

			MultiplayerEntities.Add(entity);

			OnUpdatePlayerTeamColor(entity);

			GameController.Instance.SetBasicParameters();
		}

		MultiplayerEntities = MultiplayerEntities.Unique();

	}

	[RPC]
	private void RemovePlayerFromList(PhotonPlayer photonPlayer)
	{
		MultiplayerEntity photon = null;
		foreach (
			MultiplayerEntity mPlayer in MultiplayerEntities.Where(mpPlayer => Equals(mpPlayer.photonPlayer, photonPlayer)))
			photon = mPlayer;

		if (photon == null) return;

		Debug.Log(string.Format("Player {0} was removed", photon.photonPlayer.name));

		if (photon.player != null)
		{
			GameController.Instance.Players.Remove(photon.player);
			foreach (Player player in photon.player.PlayersFleet)
			{
				GameController.Instance.Players.Remove(player);
			}
		}

		MultiplayerEntities.Remove(photon);

		GameController.Instance.SetBasicParameters();

		UILobbyPanel.Instance.UpdateBattleUiInfo();
	}

	#endregion


	#region Player Data functions

	[RPC]
	private void SetPlayerData(PhotonPlayer sender, byte[] playerDataBuffer)
	{
		if (Equals(MyMultiplayerEntity.photonPlayer, sender))
			return;

		MultiplayerEntity entity = GetMultiplayerEntity(sender);
	}

	[RPC]
	private void UpdatePlayerTeamColor(PhotonPlayer sender, int color)
	{
		MultiplayerEntity entity = GetMultiplayerEntity(sender);
		if (entity == null)
		{
			Debug.LogError(string.Format("Player {0}, not found on list", sender.ToString()));
			return;
		}
		TeamColor team = (TeamColor) color;

		if (IsMasterClient)
		{
			UILobbyPanel.Instance.UpdateBattleUiInfo();
			return;
		}
		entity.player.OnChangeTeamColor((TeamColor) color); // Team = (TeamColor) color;

		UILobbyPanel.Instance.UpdateBattleUiInfo();
	}

	[RPC]
	private void ChangePlayerTeamColor(PhotonPlayer player)
	{
		MultiplayerEntity entity = GetMultiplayerEntity(player);
		if (entity == null)
		{
			//TODO:: add send error message to sender
			return;
		}

		OnChangePlayerTeamColor(entity);
	}

	[RPC]
	private void SetupPlayerGameInfoData(PhotonPlayer player, byte[] infoData)
	{
		if (Equals(MyMultiplayerEntity.photonPlayer, player))
			return;

		MultiplayerEntity entity = GetMultiplayerEntity(player);
		if (entity == null)
		{
			Debug.LogError("SetupPlayerGameInfoData: can't get entity data");
			return;
		}

		DataBuffer buffer = new DataBuffer(infoData);
		int dataCount = buffer.ReadInt();

		if (dataCount == 0)
		{
			Debug.LogError("SetupPlayerGameInfoData: can't get preset ship data - number of the info data is 0");
			return;
		}

		List<int> shipPreset = new List<int>();
		List<int> aiTacticPreset = new List<int>();

		for (int i = 0; i != dataCount; i++)
		{
			shipPreset.Add(buffer.ReadInt());
			aiTacticPreset.Add(buffer.ReadInt());
		}


		UIController.Instance.OnSupportTeamSetup(entity.player, shipPreset, true);

		GameController.AiPlayerTacticSet(entity.player.PlayersFleet, aiTacticPreset);
	}


	[RPC]
	private void SetupPlayerIsReady(PhotonPlayer sender, bool isReady)
	{
		if (Equals(MyMultiplayerEntity.photonPlayer, sender))
			return;

		MultiplayerEntity entity = GetMultiplayerEntity(sender);

		if (entity == null)
		{
			Debug.LogError("SetupPlayerIsReady: can't get entity data");
			return;
		}

		MultiplayerPlayer multiplayer = entity.player as MultiplayerPlayer;

		if (multiplayer == null)
		{
			Debug.LogError("SetupPlayerIsReady: can't get multiplayer data");
			return;
		}

		multiplayer.SetIsReady(isReady, true);
		UILobbyPanel.Instance.SetPlayerIsReady(entity, isReady);

		//TODO:: for Master-Client after setting any player IsReady call function that check condition for start battle;

	}

	[RPC]
	private void LetStartTheBattle(PhotonPlayer sender)
	{
		if (Equals(MyMultiplayerEntity.photonPlayer, sender))
			return;

		StartCoroutine(PreStartBattle(true));

	}

	public IEnumerator PreStartBattle(bool fromServer)
	{
		UILoadingScreen loadingScreen = FindObjectOfType<UILoadingScreen>();

		if (loadingScreen != null)
			loadingScreen.Show();

		yield return new WaitForSeconds(1.0f);

		GameController.Instance.StartBattle(fromServer);
	}
		
	[RPC]
	private void SetPlayerId(PhotonPlayer playerToSet, int id)
	{
		MultiplayerEntity entity = GetMultiplayerEntity(playerToSet);

		if (entity == null)
		{
			Debug.LogError("SetPlayerId: can't get multiplayer data");
			return;
		}

		entity.player.SetPlayerId(id);
	}

	[RPC]
	private void SetPlayerIsCaptain(PhotonPlayer playerToSet)
	{
		MultiplayerEntity entity = GetMultiplayerEntity(playerToSet);

		if (entity == null)
		{
			Debug.LogError("SetPlayerId: can't get multiplayer data");
			return;
		}

		entity.player.SetCaptain();
	}

	[RPC]
	private void SynchronizePlayerId(PhotonPlayer photonPlayer)
	{
		foreach (MultiplayerEntity entity in MultiplayerEntities)
		{
			Debug.Log(string.Format("SynchronizePlayerId: player:{0}, id:{1}", entity.photonPlayer, entity.player.Id));
			photonView.RPC("SetPlayerId", photonPlayer, entity.photonPlayer, entity.player.Id);

		}

	}

	#endregion

	#region Battle Logic

	[RPC]
	private void ShipTakeDamage(PhotonPlayer sender, int playerId, int damage, int shellOwnerId)
	{
		if (Equals(MyMultiplayerEntity.photonPlayer, sender))
			return;

		ShipBehaviour ship = BattleController.Instance.GetShipBehaviour(playerId);

		if (ship == null)
		{
			Debug.Log(string.Format("ShipTakeDamage: ship with id:{0} not found", playerId));
			return;
		}

		ship.DealShipDamage(shellOwnerId, damage, true);

	}

	[RPC]
	private void ShellBlowUp(PhotonPlayer sender, int shellID)
	{
		if (Equals(MyMultiplayerEntity.photonPlayer, sender))
			return;

		WeaponBehaviour shell = BattleController.Instance.GetShellByViewID(shellID);

		if (shell != null)
		{
			shell.BlowUp();
		}
	}

	[RPC]
	private void PlayerFireBasic(PhotonPlayer sender, int playerId)
	{
		if (Equals(MyMultiplayerEntity.photonPlayer, sender))
			return;

		Player player = GameController.Instance.GetPlayer(playerId);

		if (player == null)
		{
			Debug.Log(string.Format("PlayerFireBasic: player with id:{0} not found", playerId));
			return;
		}

		player.MyShip.OnFireBasic(true);
	}

	[RPC]
	private void PlayerFireBomb(PhotonPlayer sender, int playerId)
	{
		Debug.Log(string.Format("Trying  fire bomb for player with Id: {0} is fired bomb", playerId));

		if (Equals(MyMultiplayerEntity.photonPlayer, sender))
			return;

		Player player = GameSetObserver.Instance.GetPlayer(playerId);

		if (player == null)
		{
			Debug.Log(string.Format("PlayerFireBomb: player with id{0} not found", playerId));
			return;
		}

		Debug.Log(string.Format("from {1}: Player {0} is fired bomb", playerId, MyMultiplayerEntity.photonPlayer));


		player.MyShip.OnFireBomb(true);
	}

	[RPC]
	private void FlagSpotTakeFlag(PhotonPlayer sender, int spotColor, int shipId)
	{
		if (Equals(MyMultiplayerEntity.photonPlayer, sender))
			return;

		FlagSpotBehaviour flagSpot = BattleController.Instance.GetFlagSpotBehaviour((TeamColor) spotColor);

		if (flagSpot == null)
		{
			Debug.Log(string.Format("FlagSpotTakeFlag: flagSpot with spot color {0} not found", (TeamColor) spotColor));
			return;
		}

		ShipBehaviour ship = BattleController.Instance.GetShipBehaviour(shipId);

		if (ship == null)
		{
			Debug.Log(string.Format("FlagSpotTakeFlag: ship with id:{0} not found", shipId));
			return;
		}

		flagSpot.TakeFlag(ship, true);
	}

	[RPC]
	private void FlagsTakeFlag(PhotonPlayer sender, int flagColor, int shipId)
	{
		if (Equals(MyMultiplayerEntity.photonPlayer, sender))
			return;

		FlagsBehaviour flag = BattleController.Instance.GetFlagsBehaviour((TeamColor) flagColor);

		if (flag == null)
		{
			Debug.Log(string.Format("FlagsTakeFlag: flags with color {0} not found", (TeamColor) flagColor));
			return;
		}

		ShipBehaviour ship = BattleController.Instance.GetShipBehaviour(shipId);

		if (ship == null)
		{
			Debug.LogError(string.Format("FlagsTakeFlag: ship with id:{0} not found"));
			return;
		}

		flag.TakeFlag(ship, true);

	}

	[RPC]
	private void EndBattle(PhotonPlayer sender, int mode, int team)
	{
		if (Equals(MyMultiplayerEntity.photonPlayer, sender))
			return;

		BattleController.Instance.OnEndBattle((GameMode) mode, true, (TeamColor) team);
	}

	#endregion

	#endregion
}
