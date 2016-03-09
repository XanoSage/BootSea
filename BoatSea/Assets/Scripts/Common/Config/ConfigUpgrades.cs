using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using System;

public class ConfigUpgrades {

	public static Dictionary<UpgradesType,ConfigUpgrade> Upgrades;
	
	public ConfigUpgrades( CommandData ShipJson )
	{
		Upgrades = new Dictionary<UpgradesType, ConfigUpgrade>();
		CommandData data = ShipJson.GetCommandData (MessageField.Upgrades);
		
		foreach (string eachUpgrade in data.GetKeys()) {
			UpgradesType type = (UpgradesType)Enum.Parse(typeof(UpgradesType),eachUpgrade);
			ConfigUpgrade upgrade = new ConfigUpgrade(data.GetCommandData(eachUpgrade));
			Upgrades.Add(type,upgrade);
		}
		
	}
}




public class ConfigUpgrade  {
	public float value;
	public int Cost;
	
	
	public ConfigUpgrade(CommandData data )
	{
		value = data.GetFloat (MessageField.Value);
		Cost = data.GetInt (MessageField.Cost);

	}
}
