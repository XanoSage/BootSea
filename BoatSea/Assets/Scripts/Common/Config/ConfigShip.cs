using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Ships;
using System;

public class ConfigShips  {
	
	public static Dictionary<ShipType,ConfigShip> Ships;



	public ConfigShips( CommandData ShipJson )
	{
		Ships = new Dictionary<ShipType, ConfigShip>();
		CommandData data = ShipJson.GetCommandData (MessageField.Ships);

		foreach (string eachShip in data.GetKeys()) {
			ShipType type = (ShipType)Enum.Parse(typeof(ShipType),eachShip);
			ConfigShip ship = new ConfigShip(data.GetCommandData(eachShip));
			Ships.Add(type,ship);
		}
	
	}
}
	
	



public class ConfigShip  {
	public  float Speed;
	public  int Health;
	public  float MaxSpeed;
	public  int BombCount;
	public  int CameraFOVFrom;
	public  int CameraFOVTo;
	public  float RotationSpeed;
	public int Price;

	public float Acceleration;
	public float AccelerationDown;
	public float MaxVelocity;

	public ConfigShip(CommandData data )
	{
		Speed = data.GetFloat (MessageField.Speed);
		Health = data.GetInt (MessageField.Health);			
		MaxSpeed = data.GetFloat (MessageField.MaxSpeed);			
		BombCount = data.GetInt (MessageField.BombCount);			
		CameraFOVFrom = data.GetInt (MessageField.CameraFOVFrom);
		CameraFOVTo = data.GetInt (MessageField.CameraFOVTo);
		RotationSpeed = data.GetFloat (MessageField.RotationSpeed);
		Price = data.GetInt (MessageField.Price);

		Acceleration = data.GetFloat(MessageField.Acceleration);
		AccelerationDown = data.GetFloat(MessageField.AccelerationDown);

		MaxVelocity = data.GetFloat(MessageField.MaxVelocity);
	}

}

