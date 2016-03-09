using UnityEngine;
using System.Collections;

public class TestConfig : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		//создаем и читаем параметры кораблей
		CommandData ShipJson = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigShips").ToString (), "ShipsConfig");
		ConfigShips ships = new ConfigShips (ShipJson);
		
		//создаем и читаем Параметры патронов
		CommandData WeaponJson = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigWeapon").ToString (), "WeaponConfig");
		ConfigWeapons weapon = new ConfigWeapons (WeaponJson);
		
		//создаем и читаем Переводчик
		//Debug.Log ("localization"+PlayerInfo.Instance.language);
		CommandData Language = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("localization"+"Ru").ToString (), "Language");
		LocalizationConfig text = new LocalizationConfig(Language);
		
		//создаем и читаем параметры улучшений
		CommandData UpgradesJson = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigUpgrades").ToString (), "UpgradesConfig");
		ConfigUpgrades Upgrades = new ConfigUpgrades (UpgradesJson);
		
		//создаем и читаем параметры Бонусов
		CommandData BonusJson = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigBonuses").ToString (), "BonusesConfig");
		ConfigBonuses Bonus = new ConfigBonuses (BonusJson);
		
		CommandData Campaign = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigCampaignMision").ToString (), "ConfigCampaignMision");
		ConfigCampaign Level = new ConfigCampaign (Campaign);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
