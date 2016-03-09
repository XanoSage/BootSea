using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Base;
using Aratog.NavyFight.Models.Unity3D.Flags;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;

public class FlagSpotBehaviour : PoolItem {

	#region Variables

	public TeamColor Color;

	//TODO:: on init set necessary transform;
	[HideInInspector]
	public FlagsBehaviour Flag;

	[HideInInspector]
	public BaseParent Base;

	public float AlarmCounter { get; private set; }
	private float _alarmCount = 20f;

	private EffectsBehaviour _flagEffect;

	private bool isFlagEffectShowed;

	#endregion

	#region MonoBehavoiur events
	// Use this for initialization
	void Start () {
		AlarmCounter = _alarmCount;
		isFlagEffectShowed = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Base == null) 
			return;

		if (Base.State == BaseState.Alarm)
		{
			if (AlarmCounter > 0)
			{
				AlarmCounter -= Time.deltaTime;
			}

			if (AlarmCounter <= 0)
			{
				AlarmCounter = _alarmCount;
				AlarmOff();
			}
		}

		if (isFlagEffectShowed)
		{
			if (_flagEffect != null)
			{
				if (!_flagEffect.IsParticleSystemAlive)
				{
					isFlagEffectShowed = false;
					_flagEffect = null;
				}
			}
		}
	}


	void OnTriggerEnter (Collider other) {
		if (GameSetObserver.Instance.CurrentBattle.Mode == GameMode.CaptureTheFlag||GameSetObserver.Instance.CurrentBattle.Mode == GameMode.Survival) {
						if (Flag == null || Flag.Flag == null)
								return;

						if (Flag.Flag.State == FlagState.Taken)
								return;

						ShipBehaviour ship = null;
						if (other.transform.parent != null) 
						if (other.transform.parent.parent != null)
							ship = other.transform.parent.parent.GetComponent<ShipBehaviour> ();

						
						
							TakeFlag (ship);
						
				}
	}

	

	#endregion

	#region Events

	public void TakeFlag (ShipBehaviour ship, bool fromServer = false) {
		if (ship == null || !ship.IsCanTakeFlag) 
			return;

		if (ship.Player.Team != Color) {
			if (BattleController.Instance.ActiveBattle.Mode != GameMode.CaptureTheFlag) {
				if (BattleController.Instance.CurrFlagCollor == Color) {
					ship.Player.MyShip.OnFlagTaken (Color);
					ship.enemyFlag = Flag;
					Flag.SetParent (ship.transform);

					AlarmOn ();
				}
			}
			if (BattleController.Instance.ActiveBattle.Mode == GameMode.CaptureTheFlag) {
				ship.Player.MyShip.OnFlagTaken (Color);
				ship.enemyFlag = Flag;
				Flag.SetParent (ship.transform);
				
				AlarmOn ();
			}

		}
				
			
		

	 	else if (ship.Player.MyShip.IsFlagTaken)
		{
			ship.Player.MyShip.OnFlagDeliveredEvent += OnFlagDelivered;

			ship.Player.MyShip.OnFlagDelivered(ship.enemyFlag.Color);

			ship.Player.Statistic_flag++;

			ship.Player.MyShip.OnFlagDeliveredEvent -= OnFlagDelivered;

			ShowFlagEffect(Flag);

			ship.enemyFlag.OnFlagReturned(ship.enemyFlag.Color);

			ship.enemyFlag.Base.RemoveFlagEffect();
			
			ship.enemyFlag = null;
		}

		if (fromServer)
			return;

		//add multiplayer event
		if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer) {
			MultiplayerManager.Instance.NeedFlagSpotTakeFlag(Color, ship.Player.Id);
		}
	}

	public void Init () {
		transform.position = Base.Position;
		Base.AlarmZone = Map.GetEdgePointOnRadius(GameSetObserver.Instance.CurrentBattle.Map, Base.Position, 5);
		Base.GuardZone = Map.GetEdgePointOnRadius(GameSetObserver.Instance.CurrentBattle.Map, Base.Position,
		                                          BaseParent.GuardRadius);
	}

	private void OnFlagDelivered(TeamColor color)
	{
		ShowFlagEffect(Flag);

        PlayFanfareSound(color);
	}

	public void ShowFlagEffect(FlagsBehaviour flag)
	{
		if (_flagEffect == null)
		{
			EffectsBehaviour.EffectsType effectsType = flag.Color == TeamColor.BlueTeam
				                                           ? EffectsBehaviour.EffectsType.FlagBlue
				                                           : EffectsBehaviour.EffectsType.FlagRed;

			_flagEffect = ResourceBehaviourController.Instance.GetEffectsFromPool(effectsType);
		}

		if (_flagEffect != null)
		{
			_flagEffect.SetBasicData(transform.position);
			isFlagEffectShowed = true;
		}
	}

	public void RemoveFlagEffect()
	{
		if (_flagEffect == null) 
			return;

		_flagEffect.Remove();
		_flagEffect = null;
	}


	public void AlarmOn()
	{
		if (Base != null)
		{	
			Base.AlarmOn();

            PlayAlarmSound();

			Debug.Log(string.Format("{0} base is under attack, now it state is: {1}", Color, Base.State));
		}
	}

	public void AlarmOff()
	{
		if (Base != null)
		{
			Base.AlarmOff();

            StopAlarmSound();
		}
	}

	#endregion

	#region Ovveride events

	public override bool EqualsTo (PoolItem item) {
		if (!(item is FlagSpotBehaviour))
			return false;

		FlagSpotBehaviour flagSpot = item as FlagSpotBehaviour;

		if (flagSpot.Color != Color)
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

    #region Sound

    private AudioSource _audioSourceForAlarm;

    private void PlayAlarmSound()
    {
		if (GameSetObserver.Instance.Human == null)
			return;

        if (Color == GameSetObserver.Instance.Human.Team)
        {
            _audioSourceForAlarm = SoundController.PlayAlarmSound();
        }
    }

    private void StopAlarmSound()
    {
		if (GameSetObserver.Instance.Human == null)
			return;

        if (Color == GameSetObserver.Instance.Human.Team)
        {
            if (_audioSourceForAlarm != null)
            {

                SoundController.StopAlarmSound(_audioSourceForAlarm);

                _audioSourceForAlarm = null;
            }
        }
    }

    private void PlayFanfareSound(TeamColor color)
    {
		if (GameSetObserver.Instance.Human == null)
			return;

        if (color != GameSetObserver.Instance.Human.Team)
        {
            // Possitive
            SoundController.PlayPossitiveFanfare();
        }
        else
        {
            // Negative
            SoundController.PlayNegativeFanfare();
        }
    }

    #endregion
}
