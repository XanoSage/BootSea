using System.Collections.Generic;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Battles;
using Aratog.NavyFight.Models.Unity3D.Players;
using Assets.Scripts.Common.GameLogic;
using UnityEngine;
using System.Collections;

public class GameSetObserver : MonoBehaviour, IGameController
{

	#region Vriables

	public static GameSetObserver Instance { get; private set; }

	public GameType CurrentGameType
	{
		get
		{
			return GameController.Instance != null
				       ? GameController.Instance.CurrentGameType
				       : AITestController.Instance.CurrentGameType;
		}
		set { throw new System.NotImplementedException(); }
	}

	public List<Player> Players
	{
		get { return GameController.Instance != null ? GameController.Instance.Players : AITestController.Instance.Players; }
		set { throw new System.NotImplementedException(); }
	}

	public Player Human
	{
		get { return GameController.Instance != null ? GameController.Instance.Human : AITestController.Instance.Human; }
		set { throw new System.NotImplementedException(); }
	}

	public List<Player> BlueTeamPlayers
	{
		get
		{
			return GameController.Instance != null
				       ? GameController.Instance.BlueTeamPlayers
				       : AITestController.Instance.BlueTeamPlayers;
		}
		set { throw new System.NotImplementedException(); }
	}

	public List<Player> RedTeamPlayers
	{
		get
		{
			return GameController.Instance != null
				       ? GameController.Instance.RedTeamPlayers
				       : AITestController.Instance.RedTeamPlayers;
		}
		set { throw new System.NotImplementedException(); }
	}

	public Battle CurrentBattle
	{
		get
		{
			return GameController.Instance != null
				       ? GameController.Instance.CurrentBattle
					//: GameController.Instance.CurrentBattle;
				       : AITestController.Instance.CurrentBattle;
		}
		set { throw new System.NotImplementedException(); }
	}

	public bool IsBattleStarted
	{
		get
		{
			return GameController.Instance != null && GameController.Instance.IsBattleStarted ||
			       AITestController.Instance != null && AITestController.Instance.IsBattleStarted;
		}
		set { throw new System.NotImplementedException(); }
	}

	public bool IsPause
	{
		get
		{
			return GameController.Instance != null && GameController.Instance.IsPause ||
			       AITestController.Instance != null && AITestController.Instance.IsPause;
		}
		set { throw new System.NotImplementedException(); }
	}

	public MechanicsType Mechanics
	{
		get { return GameController.Instance != null ? GameController.Instance.Mechanics : AITestController.Instance.Mechanics; }
		set
		{
			if (GameController.Instance != null)
				GameController.Instance.Mechanics = value;
			else
				AITestController.Instance.Mechanics = value;
		}
	}

	public Player GetPlayer(int playerId)
	{
		if (GameController.Instance != null)
			return GameController.Instance.GetPlayer(playerId);

		if (AITestController.Instance != null)
			return AITestController.Instance.GetPlayer(playerId);

		return null;
	}

	#endregion

	#region MonoBehavoiur function

	private void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{

	}

	#endregion
}
