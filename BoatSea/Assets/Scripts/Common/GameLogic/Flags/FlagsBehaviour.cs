using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Flags;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;

public class FlagsBehaviour : PoolItem {

	#region Variables

	public TeamColor Color;

	private Transform Parent;

	private Vector3 directionOnDropped;
	public GameObject BasicBase;
	[HideInInspector]
	public FlagParent Flag;

	[HideInInspector]
	public FlagSpotBehaviour Base;

	private EffectsBehaviour _flagEffect;

	#endregion


	#region MonoBehaviour events
	// Use this for initialization
	void Start () {
		if (GameSetObserver.Instance.CurrentBattle.Mode != GameMode.BaseDefense) {
			BasicBase.SetActive(false);	
		}
		directionOnDropped = Vector3.zero;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameSetObserver.Instance.IsBattleStarted || GameSetObserver.Instance.IsPause || Flag == null)
			return;


		if (Parent != null)
			transform.rotation = Parent.rotation;//Base.transform.rotation;

		if (Flag.State == FlagState.Taken && Parent != null) {
			Flag.Position = Parent.position;
		}
		else if (Flag.State == FlagState.OnBase ) {
			Flag.Position = Base.Base.Position;
		}
		else if (Flag.State == FlagState.Dropped)
		{
			;
		}

		transform.position = Vector3.Lerp(transform.position, Flag.Position, Time.deltaTime*30f); //Flag.Position;


	}

	private void OnTriggerEnter(Collider other)
	{
		//Debug.Log(string.Format( "FlagsBehaviour.OnTriggerEnter: someone wants take the flag:{0}-{1}", Flag.Color, Flag.State));

		if (GameSetObserver.Instance.CurrentBattle.Mode == GameMode.CaptureTheFlag ||
		    GameSetObserver.Instance.CurrentBattle.Mode == GameMode.Survival)
		{
			if (!GameSetObserver.Instance.IsBattleStarted || GameSetObserver.Instance.IsPause || Flag == null)
				return;

			if (Flag.State != FlagState.Dropped)
			{
				return;
			}

			ShipBehaviour ship = null;
			if (other.transform.parent != null)
				if (other.transform.parent.parent != null)
					ship = other.transform.parent.parent.GetComponent<ShipBehaviour>();

			TakeFlag(ship);

			//Debug.Log(string.Format( "FlagsBehaviour.OnTriggerEnter: flag:{0}-{1} was token", Flag.Color, Flag.State));
		}
	}

	#endregion

	#region FlagsBehaviourEvents

	public void TakeFlag (ShipBehaviour ship, bool fromServer = false) {

		Debug.Log (GameSetObserver.Instance.CurrentBattle.Mode);
		if (GameSetObserver.Instance.CurrentBattle.Mode == GameMode.CaptureTheFlag||GameSetObserver.Instance.CurrentBattle.Mode == GameMode.Survival) {
						if (ship == null || !ship.IsCanTakeFlag) 
								return;

						if (ship.Player.Team == Color) {
								ship.Player.MyShip.OnFlagReturnedEvent += OnFlagReturned;

								ship.Player.MyShip.OnFlagReturned (Color);

								ship.Player.MyShip.OnFlagReturnedEvent -= OnFlagReturned;

						} else {
								ship.Player.MyShip.OnFlagTaken (Color);
								SetParent (ship.transform);
								ship.enemyFlag = this;
						}

		

						if (fromServer)
								return;

						//add multiplayer event
						if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer) {
								MultiplayerManager.Instance.NeedFlagsTakeFlag (Color, ship.Player.Id);
						}
				}
	}

	public void OnFlagReturned (TeamColor color) {

		Parent = null;
		transform.parent = null;
		transform.rotation = Base.transform.rotation;
		Flag.Position = Base.transform.position;
		
		Update();

		Base.ShowFlagEffect(this);

		Flag.State = FlagState.OnBase;
	}

	public void SetFlagDropped()
	{
		Parent = null;
		Flag.Position = Map.GetRandomPositionOnMap(GameSetObserver.Instance.CurrentBattle.Map);
		BattleController.Instance.OnFlagDropped(Color);
	}

	public void InitFlags () {
		
		Flag.State = FlagState.OnBase;
	}

	public void SetParent (Transform parent) {
		Parent = parent;
		if (Parent != null)
			directionOnDropped = Parent.transform.rotation.eulerAngles;
	}

	#endregion

	#region Ovveride events

	public override bool EqualsTo (PoolItem item) {
		if (!(item is FlagsBehaviour))
			return false;

		FlagsBehaviour flags = item as FlagsBehaviour;

		if (flags.Color != Color)
			return false;

		return true;
	}

	public override void Activate () {
		base.Activate();
		gameObject.SetActive(true);
	}

	public override void Deactivate () {
		base.Deactivate();
		gameObject.SetActive(false);
	}

	#endregion

}
