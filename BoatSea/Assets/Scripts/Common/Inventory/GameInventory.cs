using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Weapons;


public class GameInventory {



	private Dictionary <WeaponsType,int> weapons;

	private Dictionary<UpgradesType,int> upgrades;

	public int Money;

	public int VipMoney;

	void Start () {

	//	DontDestroyOnLoad (this);

		}
	public void Load()
	{
	
		InitInventory ();
		InitUpgrades ();

		}

	public void Save()
	{
		PlayerPrefs.SetInt ("Money", Money);
		PlayerPrefs.SetInt ("VipMoney",VipMoney);


		//Save Weapons
		PlayerPrefs.SetInt("Missile",weapons[WeaponsType.Missile]);		
		PlayerPrefs.SetInt("HomingMissile",weapons[WeaponsType.HomingMissile]);
		PlayerPrefs.SetInt("OneRicochet",weapons[WeaponsType.OneRicochet]);
		PlayerPrefs.SetInt("TwoRicochet",weapons[WeaponsType.TwoRicochet]);
		PlayerPrefs.SetInt("Napalm",weapons[WeaponsType.Napalm]);
		PlayerPrefs.SetInt("SuperTorpedo",weapons[WeaponsType.SuperTorpedo]);
		PlayerPrefs.SetInt("FrozenProjectile",weapons[WeaponsType.FrozenProjectile]);
		PlayerPrefs.SetInt("DeepBomb",weapons[WeaponsType.DeepBomb]);
		PlayerPrefs.SetInt("Construct",weapons[WeaponsType.Construct]);
		PlayerPrefs.SetInt("AdvanceConstruct",weapons[WeaponsType.AdvanceConstruct]);
		PlayerPrefs.SetInt("Energy",weapons[WeaponsType.Energy]);
		PlayerPrefs.SetInt("AdvanceEnergy",weapons[WeaponsType.AdvanceEnergy]);
	
		//Save Upgrades
		PlayerPrefs.SetInt("Accelerator",upgrades[UpgradesType.Accelerator]);	
		PlayerPrefs.SetInt("AcceleratorInf",upgrades[UpgradesType.AcceleratorInf]);	
		PlayerPrefs.SetInt("Armor",upgrades[UpgradesType.Armor]);	
		PlayerPrefs.SetInt("FastShell",upgrades[UpgradesType.FastShell]);	
		PlayerPrefs.SetInt("FastShipShell",upgrades[UpgradesType.FastShipShell]);	
		PlayerPrefs.SetInt("ArmorAdvance",upgrades[UpgradesType.ArmorAdvance]);	
		PlayerPrefs.SetInt("FastTorpede",upgrades[UpgradesType.FastTorpede]);	
		PlayerPrefs.SetInt("IceHouseDestroy",upgrades[UpgradesType.IceHouseDestroy]);	
		PlayerPrefs.SetInt("RapidShot",upgrades[UpgradesType.RapidShot]);	



		PlayerPrefs.Save ();
	

		}

	private void InitUpgrades()
	{
		upgrades = new Dictionary<UpgradesType, int> ();

		upgrades.Add (UpgradesType.Accelerator,LoadFromPlayerPrefs(UpgradesType.Accelerator));
		upgrades.Add (UpgradesType.AcceleratorInf,LoadFromPlayerPrefs(UpgradesType.AcceleratorInf));
		upgrades.Add (UpgradesType.Armor,LoadFromPlayerPrefs(UpgradesType.Armor));
		upgrades.Add (UpgradesType.ArmorAdvance,LoadFromPlayerPrefs(UpgradesType.ArmorAdvance));
		upgrades.Add (UpgradesType.FastShell,LoadFromPlayerPrefs(UpgradesType.FastShell));
		upgrades.Add (UpgradesType.FastShipShell,LoadFromPlayerPrefs(UpgradesType.FastShipShell));
		upgrades.Add (UpgradesType.FastTorpede,LoadFromPlayerPrefs(UpgradesType.FastTorpede));
		upgrades.Add (UpgradesType.IceHouseDestroy,LoadFromPlayerPrefs(UpgradesType.IceHouseDestroy));
		upgrades.Add (UpgradesType.RapidShot,LoadFromPlayerPrefs(UpgradesType.RapidShot));
		}

	private void InitInventory()
	{
		weapons = new Dictionary<WeaponsType, int>();

		weapons.Add (WeaponsType.Missile,LoadFromPlayerPrefs(WeaponsType.Missile));
		weapons.Add (WeaponsType.HomingMissile,LoadFromPlayerPrefs(WeaponsType.HomingMissile));
		weapons.Add (WeaponsType.OneRicochet,LoadFromPlayerPrefs(WeaponsType.OneRicochet));
		weapons.Add (WeaponsType.TwoRicochet,LoadFromPlayerPrefs(WeaponsType.TwoRicochet));
		weapons.Add (WeaponsType.Napalm,LoadFromPlayerPrefs(WeaponsType.Napalm));
		weapons.Add (WeaponsType.SuperTorpedo,LoadFromPlayerPrefs(WeaponsType.SuperTorpedo));
		weapons.Add (WeaponsType.FrozenProjectile,LoadFromPlayerPrefs(WeaponsType.FrozenProjectile));
		weapons.Add (WeaponsType.DeepBomb,LoadFromPlayerPrefs(WeaponsType.DeepBomb));
		weapons.Add (WeaponsType.Construct,LoadFromPlayerPrefs(WeaponsType.Construct));
		weapons.Add (WeaponsType.AdvanceConstruct,LoadFromPlayerPrefs(WeaponsType.AdvanceConstruct));
		weapons.Add (WeaponsType.Energy,LoadFromPlayerPrefs(WeaponsType.Energy));
		weapons.Add (WeaponsType.AdvanceEnergy,LoadFromPlayerPrefs(WeaponsType.AdvanceEnergy));
		weapons.Add (WeaponsType.MortalProjectile,LoadFromPlayerPrefs(WeaponsType.MortalProjectile));
		weapons.Add (WeaponsType.BombBigDamage,LoadFromPlayerPrefs(WeaponsType.BombBigDamage));


		Money = PlayerPrefs.GetInt("Money");
		VipMoney = PlayerPrefs.GetInt ("VipMoney");


	}

	public bool MoneyChange(int money)
	{
		int check = Money;
		int cheker = check - money;
		if (cheker < 0) {
			UIMessagePanel.Instance.SetMessage("Not Money");
			UIMessagePanel.Instance.Show();
			return false;
		} else {
			Money -= money;
			Save();
			return true;
		}


	}


	public  bool BuyWeapon(WeaponsType type,int value)
	{
		int i = weapons [type];
		i += value;
		if (i >= 0)
		{
			weapons [type] += value;
			Save ();
			return true;
		}
		else 
		{
			return false;		
		}
	}
	public  bool BuyUpgrades(UpgradesType type,int value)
	{
		int i = upgrades [type];
		i += value;
		if (i >= 0)
		{
			upgrades [type] += value;
			Save ();
			return true;
		}
		else 
		{
			return false;		
		}
	}



	public int ReturnUpgrades(UpgradesType type)
	{
		return upgrades [type];
		}

	public int ReturnWeapons(WeaponsType type)
	{

		return weapons [type];
		}



	private int LoadFromPlayerPrefs(WeaponsType type)
	{
		int i = PlayerPrefs.GetInt (type.ToString ());
		return i;
	}
	private int LoadFromPlayerPrefs(UpgradesType type)
	{
		int i = PlayerPrefs.GetInt (type.ToString ());
		return i;
	}




}
