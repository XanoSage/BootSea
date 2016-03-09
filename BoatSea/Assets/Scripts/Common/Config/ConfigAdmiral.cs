using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Aratog.NavyFight.Models.Games;

public class ConfigAdmiral  {
	public static Dictionary<int,AdmiralQuest> Quest;
	
	public ConfigAdmiral(CommandData data)
	{
		
		Quest = new Dictionary<int, AdmiralQuest>();
	
		int i = 1;
		foreach (string eachQuest in data.GetKeys()) {
			AdmiralQuest level = new AdmiralQuest(data.GetCommandData(eachQuest));
			Quest.Add(i,level);
			i++;
		}
	}
}

public class AdmiralQuest
{
	public int CampaignLevel;
	public string MapName;
	public GameMode  MissionType;
	public string PlayerShip;
	public int MissionValue;
	public string MissionTarget;
	public int PlayerShipCount;
	public int EnemiesCounts;
	public int Bots;



	public AdmiralQuest(CommandData data)
	{
		CampaignLevel = data.GetInt (MessageField.CampainMision);
		MapName = data.GetString (MessageField.MapName);
		PlayerShip = data.GetString (MessageField.PlayerShip);
		MissionValue = data.GetInt(MessageField.MissionValue);
		PlayerShipCount = data.GetInt (MessageField.PlayerShipCount);
		EnemiesCounts = data.GetInt (MessageField.EnemiesShips);
		Bots = data.GetInt (MessageField.Bots);
		MissionType =GetMode(data.GetString (MessageField.MissionType));
		MissionTarget = data.GetString (MessageField.MissionTarget);
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
