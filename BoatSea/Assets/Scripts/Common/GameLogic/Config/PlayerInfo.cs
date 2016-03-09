using UnityEngine;
using System.Collections.Generic;
using System.Collections;
//using MiniJSON;
using Aratog.NavyFight.Models.Common;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Players;


public class PlayerInfo : MonoBehaviour {
	public enum Language
		{Ru,En}
	public Language language = Language.Ru;

	public static PlayerInfo Instance;
	//Ships Saves info
	public ShipsSaves [] ShipSave;
	//player inventory
	public GameInventory inventory;



	//TODO: вынести в отдельный класс 
	private bool _healtBar;

	public bool HealthBar{
		get{
			return _healtBar;
		}
		set{
			_healtBar = value;
		
			if(_healtBar)
			{
				PlayerPrefs.SetInt("HealthBar",1);
			}
			else{
				PlayerPrefs.SetInt("HealthBar",0);
			}
		}
	}

	public int BasicBaseHealth = 0;
	//текущий выбраный уровень
	public int CurrentLevel=1;
	//самый первый еще не пройденый уровень до него все пройденые
	public int HighestLevel =1;

	//текущее задание Адмирала
	[SerializeField]
	private int _admiralQuest;
	public int AdmiralQuest {
		get{
			return _admiralQuest;
		}
		set{
			_admiralQuest = value;
			PlayerPrefs.SetInt("AdmiralQuest",_admiralQuest);
		}
	}
	public bool AdmiralQuestComplet = false;

	private bool _admiralQuestGiven;
	public bool AdmiralQuestGiven{
		get{
				return _admiralQuestGiven;
		}
		set{
			_admiralQuestGiven = value;

			if(_admiralQuestGiven)
			{
				PlayerPrefs.SetInt("AdmiralAvaileble",1);
			}
			else{
				PlayerPrefs.SetInt("AdmiralAvaileble",0);
			}
		}
	}

	public bool shipsLock = false;
	public ShipsGlobal.ShipType CurrCampaignShip;

	public bool [] ShipOnActivation;

	// Use this for initialization
	void Start () {
		Instance = this;
		DontDestroyOnLoad (this);


	//	PlayerPrefs.DeleteAll ();

		PlayerShipsInit ();
	
		//создаем и читаем параметры кораблей
		CommandData ShipJson = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigShips").ToString (), "ShipsConfig");
		ConfigShips ships = new ConfigShips (ShipJson);
		
		//создаем и читаем Параметры патронов
		CommandData WeaponJson = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigWeapon").ToString (), "WeaponConfig");
		ConfigWeapons weapon = new ConfigWeapons (WeaponJson);
		
		//создаем и читаем Переводчик
		Debug.Log ("localization"+PlayerInfo.Instance.language);
		CommandData Language = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("localization"+language).ToString (), "Language");
		LocalizationConfig text = new LocalizationConfig(Language);
		
		//создаем и читаем параметры улучшений
		CommandData UpgradesJson = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigUpgrades").ToString (), "UpgradesConfig");
		ConfigUpgrades Upgrades = new ConfigUpgrades (UpgradesJson);
		
		//создаем и читаем параметры Бонусов
		CommandData BonusJson = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigBonuses").ToString (), "BonusesConfig");
		ConfigBonuses Bonus = new ConfigBonuses (BonusJson);
		
		CommandData Campaign = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigCampaignMision").ToString (), "ConfigCampaignMision");
		ConfigCampaign Level = new ConfigCampaign (Campaign);

		CommandData Admiral = CommandDataConverter.FromJsonStringToCommandData (Resources.Load ("ConfigAdmiral").ToString (), "ConfigAdmiral");
		ConfigAdmiral Quest = new ConfigAdmiral (Admiral);


	//	Debug.Log (Resources.Load ("ConfigCannons").ToString ());

		CommandData Cannon = CommandDataConverter.FromJsonStringToCommandData (Resources.Load("ConfigCannons").ToString(),"ConfigCannons");
		ConfigCannons Cannons = new ConfigCannons (Cannon);

		inventory = new GameInventory ();
		inventory.Load ();
		if (inventory.Money <= 10) {
			inventory.Money += 1000;
		}
		int admiralQ = PlayerPrefs.GetInt ("AdmiralAvaileble");
		if (admiralQ == 1)
			_admiralQuestGiven = true;
		else 
			_admiralQuestGiven = false;


		_admiralQuest = PlayerPrefs.GetInt ("AdmiralQuest");
		if(_admiralQuest<=0)
		{
			_admiralQuest = 1;
		}

		int hBar = PlayerPrefs.GetInt ("HealthBar");
		if (hBar == 1)
			_healtBar = true;
		else 
			_healtBar = false;





		ShipSave = new ShipsSaves[4];
		ShipSave [0] = new ShipsSaves (0);
		ShipSave [1] = new ShipsSaves (1);
		ShipSave [2] = new ShipsSaves (2);
		ShipSave [3] = new ShipsSaves (3);


		Application.LoadLevel (1);

	}


	//--------------------------
	//все что ниже надобудет вынести в отдельный класс // тогда когда появится точное представлени о том что там будет
	//
	public void BuyShip(int i)
	{
		ShipOnActivation [i] = true;
		PlayerPrefs.SetInt ("shipActivated"+i,1);
	}
	private bool checkBool(int i)
	{
		if (i == 0) {
			return false;
		} else {
			return true;
		}
	}

	private void PlayerShipsInit()
	{
		ShipOnActivation = new bool[12];
		PlayerPrefs.SetInt ("shipActivated0",1);
		for (int i= 0; i<ShipOnActivation.Length; i++) {
			ShipOnActivation[i] = checkBool(PlayerPrefs.GetInt("shipActivated"+i));

		}




	}
	//-------------------------


}



