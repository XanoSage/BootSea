using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using System;


public class ConfigWeapons  {
	
	public static Dictionary<WeaponsType,ConfigWeapon> Weapon;
	
	public ConfigWeapons( CommandData UpgradeJson )
	{
		Weapon = new Dictionary<WeaponsType, ConfigWeapon>();
		CommandData data = UpgradeJson.GetCommandData (MessageField.Weapons);
		
		foreach (string eachWeapon in data.GetKeys()) {
			WeaponsType type = (WeaponsType)Enum.Parse(typeof(WeaponsType),eachWeapon);
			ConfigWeapon weapon = new ConfigWeapon(data.GetCommandData(eachWeapon));
			Weapon.Add(type,weapon);
		}
	
	}
}




public class ConfigWeapon  {
	public int PriceInCoin;
	public int PriceInGears;
	public int Damage;
	public float Speed;
	public float FlightClassic;
	public float FlightNewVawe;
	public float FireCooldown;
	public float AfterFireCooldown;
	public string Description;
	
	public ConfigWeapon(CommandData data )
	{
		Speed = data.GetFloat (MessageField.Speed);
		Damage = data.GetInt (MessageField.Damage);			
		PriceInCoin = data.GetInt (MessageField.PriceInCoin);			
		PriceInGears = data.GetInt (MessageField.PriceInGears);			
		FlightClassic = data.GetFloat (MessageField.FlightClassic);
		FlightNewVawe = data.GetFloat (MessageField.FlightNewVawe);
		FireCooldown = data.GetFloat (MessageField.FireCooldown);
		AfterFireCooldown = data.GetFloat (MessageField.AfterFireCooldown);
		Description = data.GetString (MessageField.Description);
	}
}

