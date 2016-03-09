using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using ShipsGlobal;
using Aratog.NavyFight.Models.Unity3D.Weapons;

public class ShipsSaves  {

	private WeaponsType _weapon = WeaponsType.AdvanceEnergy;
	public  WeaponsType Weapon {
		set{
			_weapon = value;
			SaveWeapon();
		}
		get{
			return _weapon;

		}
	}


	private int _weaponCount;
	public  int weaponCount {
		get{
			return _weaponCount;
		}
		set{
			_weaponCount = value;
			SaveWeaponCount();
		}
	}

	public  UpgradesType [] upgrades;

	private ShipType _type;
	public ShipType Type
	{
		set{
			_type = value;
			SaveType();
		}
		get{
			return _type;

		}
	}

	private int id;



	public ShipsSaves(int _id)
	{
		id = _id;
		_weaponCount = LoadWeaponCount ();
		_weapon = LoadWeapon ();
		upgrades = new UpgradesType[4];
		LoadUpgrades ();
		Type = LoadShipType ();
	}

	#region Loading

	private ShipType LoadShipType()
	{
		string value = PlayerPrefs.GetString("ship"+id+"type");
		switch (value) {
		case "Small":
			return ShipType.Small;
		case "Middle":
			return ShipType.Middle;
		case "Big":
			return ShipType.Big;
		case "SmallMetal":
			return ShipType.SmallMetal;
		case "MiddleMetal":
			return ShipType.MiddleMetal;
		case "BigMetal":
			return ShipType.BigMetal;
		case "SmallAtlant":
			return ShipType.SmallAtlant;
		case "MiddleAtlant":
			return ShipType.MiddleAtlant;
		case "BigAtlant":
			return ShipType.BigAtlant;
		case "SmallDark":
			return ShipType.SmallDark;
		case "MiddleDark":
			return ShipType.MiddleDark;
		case "BigDark":
			return ShipType.BigDark;
		default:
			return ShipType.Small;
		}

	}

	private int LoadWeaponCount()
	{
		return PlayerPrefs.GetInt ("ship"+id+"weaponCount");
		
	}

	private WeaponsType LoadWeapon()
	{
		string value = PlayerPrefs.GetString ("ship"+id+"weaponType");

		switch (value)
		{
		case "BasicProjectile":
		return	WeaponsType.BasicProjectile;
		case "BasicTorpedo":
			return	WeaponsType.BasicTorpedo;
		case "BasicBomb":
			return	WeaponsType.BasicBomb;
		case "Missile":
			return	WeaponsType.Missile;
		case "HomingMissile":
			return	WeaponsType.HomingMissile;
		case "OneRicochet":
			return	WeaponsType.OneRicochet;
		case "TwoRicochet":
			return	WeaponsType.TwoRicochet;
		case "Napalm":
			return	WeaponsType.Napalm;
		case "SuperTorpedo":
			return	WeaponsType.SuperTorpedo;
		case "FrozenProjectile":
			return	WeaponsType.FrozenProjectile;
		case "DeepBomb":
			return	WeaponsType.DeepBomb;
		default:
			return WeaponsType.Missile;
		}
	}

	public void DeleteUpgrades(UpgradesType type)
	{
		
		for (int i=0; i<4; i++) {
			string value = PlayerPrefs.GetString ("ship" + id + "upgrades" + i, upgrades [i].ToString ());
		
			if(upgrades [i] == type)
			{
				PlayerPrefs.SetString("ship" + id + "upgrades" + i,"none");
			}

		}
	}

	private void LoadUpgrades()
	{
		for (int i=0; i<4; i++) {
			string value = PlayerPrefs.GetString ("ship" + id + "upgrades" + i, upgrades [i].ToString ());
			if (value =="AcceleratorInf") {
				upgrades[i] = UpgradesType.AcceleratorInf;
			}
			else 
			{
				upgrades[i] = UpgradesType.None;

			}
		}

	}

	#endregion


	#region Saves
	private void SaveType()
	{
		PlayerPrefs.SetString("ship"+id+"type",Type.ToString());
	}

	private void SaveWeaponCount()
	{
		PlayerPrefs.SetInt ("ship"+id+"weaponCount",weaponCount);

	}

	private void SaveWeapon()
	{
		PlayerPrefs.SetString ("ship"+id+"weaponType",Weapon.ToString());
	}

	public void SaveUpgrades(UpgradesType type,int i )
	{
		if(type == UpgradesType.AcceleratorInf)
		{
			Debug.Log("SAVE Path: ship" + id + "upgrades"+i+" Value: " +upgrades [i].ToString());
			PlayerPrefs.SetString ("ship" + id + "upgrades"+i, upgrades [i].ToString());
		}
	}
	#endregion

}
