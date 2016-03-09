using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Aratog.NavyFight.Models.Games;


public class ConfigCampaign {
	public static Dictionary<int,CampaignLevel> Levels;

	public ConfigCampaign(CommandData data)
	{

		Levels = new Dictionary<int, CampaignLevel>();
		CommandData _data = data.GetCommandData("Island1");
		int i = 1;
		foreach (string eachLevel in _data.GetKeys()) {
			CampaignLevel level = new CampaignLevel(_data.GetCommandData(eachLevel));
			Levels.Add(i,level);
			i++;
		}
	}
}

public class CampaignLevel
{

	public string MapName;
	public GameMode  MissionType;
	public string PlayerShip;
	public int MissionValue;
	public int PlayerShipCount;
	public int EnemiesCounts;
	public int Bots;

	public CampaignLevel(CommandData data)
	{
		MapName = data.GetString (MessageField.MapName);
		PlayerShip = data.GetString (MessageField.PlayerShip);
		MissionValue = data.GetInt(MessageField.MissionValue);
		PlayerShipCount = data.GetInt (MessageField.PlayerShipCount);
		EnemiesCounts = data.GetInt (MessageField.EnemiesShips);
		Bots = data.GetInt (MessageField.Bots);
		MissionType =GetMode(data.GetString (MessageField.MissionType));

	}

	private GameMode GetMode (string value)
	{
		switch (value) {
		case "DeathMath":
			return GameMode.Deathmatch;
		case "CaptureTheFlag":
			return GameMode.CaptureTheFlag;
		case "BaseDefense":
			return GameMode.BaseDefense;
		case "NavalConvoys":
			return GameMode.NavalConvoys;
		case "Survival":
			return GameMode.Survival;
		case "TimeCaptureTheFlag":
			return GameMode.TimeCaptureTheFlag;
		default :
			return GameMode.Deathmatch;
		}
	}
}
