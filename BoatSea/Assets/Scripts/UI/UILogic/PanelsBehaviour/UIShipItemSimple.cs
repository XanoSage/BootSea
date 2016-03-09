using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Ship;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using Assets.Scripts.Common.Useful;
using UnityEngine;
using System.Collections;

public class UIShipItemSimple : MonoBehaviour, IShowable {
	#region Constants

	private const string CaptainIconName = "capain_anchor";
	private const string CaptainText = "Captain";

	private const string DefenseIcon = "defence_ai_icon";
	private const string DefenseText = "defence";

	private const string AttackIcon = "attack_ai_icon";
	private const string AttackText = "attack";


	#endregion

	#region Variables

	[SerializeField] private UISprite _shipIcon;

	[SerializeField] private UILabel _tacticLabel; // if captain then was displaying captain tect and captain icon
	[SerializeField] private UISprite _tacticIcon;

	[SerializeField] private UILabel _maxSpeedLabel;
	[SerializeField] private UILabel _healthPointLabel;
	[SerializeField] private UILabel _minesLabel;
	[SerializeField] private UILabel _rateLabel;

	[SerializeField]
	private UILabel _weaponLabel;
	[SerializeField]
	private UISprite _weaponSprite;

	[SerializeField]
	private UISprite[] _upgradesSprite;



	private Player _player;


	public bool Visible { get; private set; }
	#endregion
	

	#region actions

	public void Init(Player player)
	{
		_player = player;
		ShipStatsInit(_player.MyShip);
		ShipIconInit(_player);
		PlayerTacticInit(_player);
		ShipWeaponIcon (_player);
		UpgradesIcon ();
		Debug.Log ("init");
	}

	private void ShipStatsInit(Ship ship)
	{
		_maxSpeedLabel.text = Mathf.Ceil(ship.MaxSpeed).ToString();
		_healthPointLabel.text = ship.HealthPoint.ToString();
		_minesLabel.text = ship.BombCount.ToString();
		_rateLabel.text = ship.Rate.ToString();
		Debug.Log ("initStats");
	}


	private void UpgradesIcon()
	{
		for (int i=0; i<_upgradesSprite.Length; i++) 
		{
			ChangeUpgradeIcon (i,_player.upgrades[i]);
		
		}
	}

	private void ChangeUpgradeIcon(int icon,UpgradesType type)
	{
		switch(type){
		case UpgradesType.Accelerator:
			_upgradesSprite[icon].spriteName = "up_turbo_speed2";
			break;
		case UpgradesType.AcceleratorInf:
			_upgradesSprite[icon].spriteName = "up_turbo_speed";
			break;
		case UpgradesType.Armor:
			_upgradesSprite[icon].spriteName = "wp_armore";
			break;
		case UpgradesType.ArmorAdvance:
			_upgradesSprite[icon].spriteName = "wp_armore1";
			break;
		case UpgradesType.FastShell:
			_upgradesSprite[icon].spriteName = "up_fast_wp";
			break;
		case UpgradesType.FastShipShell:
			_upgradesSprite[icon].spriteName = "up_speed_core";
			break;
		case UpgradesType.FastTorpede:
			_upgradesSprite[icon].spriteName = "up_fast_torpedo";
			break;
		case UpgradesType.IceHouseDestroy:
			_upgradesSprite[icon].spriteName = "up_icecream";
			break;
		case UpgradesType.None:
			_upgradesSprite[icon].spriteName = "wp1";
			break;
		case UpgradesType.RapidShot:
			_upgradesSprite[icon].spriteName = "up_speed_canon";
			break;
		}
		
	}

	private void ShipWeaponIcon(Player player)
	{
		string weaponName = "";
		switch(player.AdvanceWeapon){
		case WeaponsType.Missile:

			weaponName = "wp_rocket1";
			break;
		case WeaponsType.HomingMissile:
			weaponName = "wp_rocket";
			break;
		case WeaponsType.OneRicochet:
			weaponName = "wp_ricoshet1";
			break;
		case WeaponsType.TwoRicochet:
			weaponName = "wp_ricoshet";
			break;
		case WeaponsType.Napalm:
			weaponName = "wp_napalm";
			break;
		case WeaponsType.SuperTorpedo:
			weaponName = "wp_torpedo";
			break;
		case WeaponsType.FrozenProjectile:
			weaponName = "wp_icecube";
			break;
		case WeaponsType.DeepBomb:
			weaponName = "wp_mine";
			break;
		case WeaponsType.Construct:
			weaponName = "wp_construct";
			break;
		case WeaponsType.AdvanceConstruct:
			weaponName = "wp_construct1";
			break;
		case WeaponsType.Energy:
			weaponName = "wp_energy1";
			break;
		case WeaponsType.AdvanceEnergy:
			weaponName = "wp_energy";
			break;
		}

		_weaponSprite.spriteName = weaponName;

		int weaponCount = player.AdvanceWeaponNumber;
		_weaponLabel.text = weaponCount.ToString();
	}






	private void ShipIconInit(Player player)
	{

		Debug.Log (player.MyShip.Type);

	
		_shipIcon.spriteName = ShipName(player.MyShip.Type);
	}

	private string ShipName(ShipType type)
	{
		string str = "";

		switch (type)
		{
		case ShipType.BigShip:
			str = "Linkor_Med_ico";
			break;
		case ShipType.Submarine:
			str = "SM_Med_ico" ;
			break;
		case ShipType.Boat:
			str = "Kater_Med_ico" ;
			break;
		case ShipType.BigMetal:
			str = "Linkor_MS_ico";
			break;
		case ShipType.MiddleMetal:
			str = "SM_MS_ico" ;
			break;
		case ShipType.SmallMetal:
			str = "Kater_MS_ico" ;
			break;
		case ShipType.BigAtlant:
			str = "Linkor_atlantis_ico";
			break;
		case ShipType.MiddleAtlant:
			str = "SM_Atlantis_ico" ;
			break;
		case ShipType.SmallAtlant:
			str = "Kater_atlantis_ico" ;
			break;
		case ShipType.BigDark:
			str = "Linkor_gothic_ico";
			break;
		case ShipType.MiddleDark:
			str = "SM_gothic_ico" ;
			break;
		case ShipType.SmallDark:
			str = "Kater_gothic_ico" ;
			break;
		}
		
		return str;
	}

	private void PlayerTacticInit(Player player)
	{
		Debug.Log ("initPlayerTactic");
		if (player.IsCaptain)
		{
			_tacticIcon.spriteName = CaptainIconName;
			_tacticLabel.text = CaptainText;
		}

		else if (player is AIPlayer)
		{
			AIPlayer aiPlayer = player as AIPlayer;
			switch (aiPlayer.Tactic)
			{
				case AITactic.BaseDefence:
					_tacticIcon.spriteName = DefenseIcon;
					_tacticLabel.text = DefenseText;
					break;

				case AITactic.CaptureEnemy:
					_tacticIcon.spriteName = AttackIcon;
					_tacticLabel.text = AttackText;
					break;
			}
		}
	}
	#endregion


	#region IShowable implementation

	public void Show()
	{
		Debug.Log ("show");
		Visible = true;
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		Visible = false;

		gameObject.SetActive(false);
	}

	#endregion
}
