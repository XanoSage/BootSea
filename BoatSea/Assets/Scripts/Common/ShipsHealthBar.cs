using UnityEngine;
using System.Collections;
using Aratog.NavyFight.Models.Unity3D.Weapons;

public class ShipsHealthBar : MonoBehaviour {


	private Transform _camera;
	private ShipBehaviour _ship;



	public UISprite [] HealthSprite;
	public UISprite [] UpgradesSprite;


	[SerializeField]
	private int _health;
	[SerializeField]
	private int _healthCount;
	[SerializeField]
	private int _armorCount;
	[SerializeField]
	private int upgrades;



	private string _healtActive = "options_act1";
	private string _healtInActive = "options_act0";
	private string _armorActive = "wp_armore1";
	private string _armorInActive = "options_act0";



	int timer = 0;
	// Use this for initialization
	void Start () {
	
	}

	private bool _isInit = false;
	public void Init()
	{
		_camera = Camera.main.transform;
		_ship = transform.parent.GetComponent<ShipBehaviour>();
		
		//init health and armor
		_armorCount = _ship.Player.MyShip.BaseArmor;
		_health = ConfigShips.Ships [_ship.shipType].Health ;
		_healthCount = _health + _armorCount;
		
		
		for(int i=0;i<_healthCount;i++)
		{
			HealthSprite[i].gameObject.SetActive(true);

			if(i>=_health)
			{
				Debug.Log("ARMOR");
				HealthSprite[i].spriteName = _armorActive;
			}
			
		}

		//init upgrades
		for (int i = 0; i<UpgradesSprite.Length; i++) {
			UpgradesType type = _ship.Player.upgrades[i];
			if( type!= UpgradesType.None)
			{
				UpgradesSprite[i].gameObject.SetActive(true);
				ChangeUpgradeIcon(i,type);
			}
		}
		_isInit = true;
	}



	public void ChangeUpgradeIcon(int icon, UpgradesType type)
	{
		switch(type){
		case UpgradesType.Accelerator:
			UpgradesSprite[icon].spriteName = "up_turbo_speed2";
			break;
		case UpgradesType.AcceleratorInf:
			UpgradesSprite[icon].spriteName = "up_turbo_speed";
			break;
		case UpgradesType.Armor:
			UpgradesSprite[icon].spriteName = "wp_armore";
			break;
		case UpgradesType.ArmorAdvance:
			UpgradesSprite[icon].spriteName = "wp_armore1";
			break;
		case UpgradesType.FastShell:
			UpgradesSprite[icon].spriteName = "up_fast_wp";
			break;
		case UpgradesType.FastShipShell:
			UpgradesSprite[icon].spriteName = "up_speed_core";
			break;
		case UpgradesType.FastTorpede:
			UpgradesSprite[icon].spriteName = "up_fast_torpedo";
			break;
		case UpgradesType.IceHouseDestroy:
			UpgradesSprite[icon].spriteName = "up_icecream";
			break;
		case UpgradesType.None:
			UpgradesSprite[icon].spriteName = "wp1";
			break;
		case UpgradesType.RapidShot:
			UpgradesSprite[icon].spriteName = "up_speed_canon";
			break;
		}
		
		}


	public void UpdateHealthBar()
	{

		int health = _ship.health;
		int armor = health + _armorCount;
		int currArmor = _ship.iArmor;

		for(int i=_healthCount;i>=0;i--)
		{
			if(i>=_health)
			{
				if(currArmor>=0){
					HealthSprite[i].spriteName = _armorActive;
					currArmor--;
				}
				else {
					HealthSprite[i].spriteName = _armorInActive;
				}
			}
			else if(i>=health)
			{
			HealthSprite[i].spriteName = _healtInActive;
			}
			else 
			{
				HealthSprite[i].spriteName =_healtActive ;
			}
			HealthSprite[i].gameObject.transform.localScale = new Vector3(0.25f,0.5f,1);
			
		}



	}
	
	// Update is called once per frame
	void Update () {

		if (_camera == null)
		{
			if (!_isInit)
				Init();
			return;
		}
		transform.LookAt(_camera.position);
	/*	timer++;
		if (timer >= 5) {

			timer = 0;
		}*/

	}
}
