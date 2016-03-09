using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ConfigBonuses  {
	
	public static Dictionary<BonusesType,ConfigBonus> Bonuses;
	
	public ConfigBonuses( CommandData BonusJson )
	{
		Bonuses = new Dictionary<BonusesType, ConfigBonus>();
		CommandData data = BonusJson.GetCommandData (MessageField.Bonuses);
		
		foreach (string eachBonus in data.GetKeys()) {
			BonusesType type = (BonusesType)Enum.Parse(typeof(BonusesType),eachBonus);
			ConfigBonus bonus = new ConfigBonus(data.GetCommandData(eachBonus));
			Bonuses.Add(type,bonus);
		}
		
	}
}




public class ConfigBonus  {
	public float value;
	public float time;
	
	
	public ConfigBonus(CommandData data )
	{
		value = data.GetFloat (MessageField.Value);
		time = data.GetFloat (MessageField.Time);
	}
}
