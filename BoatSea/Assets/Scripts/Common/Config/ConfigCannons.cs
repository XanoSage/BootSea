using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ConfigCannons  {
	
	public static Dictionary<CannonType,ConfigCannon> Cannons;
	
	public ConfigCannons( CommandData CannonJson )
	{
		Cannons = new Dictionary<CannonType,ConfigCannon>();
		CommandData data = CannonJson.GetCommandData (MessageField.Cannons);
		
		foreach (string eachCannon in data.GetKeys()) {
			CannonType type = (CannonType)Enum.Parse(typeof(CannonType),eachCannon);
			ConfigCannon bonus = new ConfigCannon(data.GetCommandData(eachCannon));
			Cannons.Add(type,bonus);
		}
		
	}
}




public class ConfigCannon  {
	public int health;
	public float RotationSpeed ;
	public float ShootSpeed;
	public float Distance;
	
	public ConfigCannon(CommandData data )
	{
		health = data.GetInt (MessageField.Health);
		RotationSpeed = data.GetFloat (MessageField.RotationSpeed);
		ShootSpeed = data.GetFloat (MessageField.ShootSpeed);
		Distance = data.GetFloat (MessageField.Distance);

	}
}
