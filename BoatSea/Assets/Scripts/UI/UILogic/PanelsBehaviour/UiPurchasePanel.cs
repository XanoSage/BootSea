using UnityEngine;
using System.Collections;
using Aratog.NavyFight.Models.Unity3D.Weapons;

public class UiPurchasePanel : MonoBehaviour {
	public static UiPurchasePanel Instance;


	[SerializeField]
	private UIEventListener _Close;

	[SerializeField]
	private UIEventListener _Buy;
	[SerializeField]
	private GameObject buy;


	[SerializeField]
	private UIEventListener _Equip;
	[SerializeField]
	private GameObject equip;

	[SerializeField]
	private UIEventListener _UnEquip;
	[SerializeField]
	private GameObject unEquip;

	[SerializeField]
	private UILabel _descrLabel;

	[SerializeField]
	private UILabel _descrTitle;

	[SerializeField]
	private UIEventListener _add;

	[SerializeField]
	private UIEventListener _remove;

	[SerializeField]
	private UISprite _weapon;

	[SerializeField]
	private UILabel _weaponNumber;

	[SerializeField]
	private UILabel _moneyNumber;

	public int weaponCount;
	public int moneyCount;

	private WeaponsType weapon;
	private UpgradesType upgrades;
	private bool isWeapon;
	private bool isEquip;

	// Use this for initialization
	void Start () {
		Instance = this;
		_add.onClick += Plus;
		_remove.onClick += Remove;
		_Equip.onClick += Equip;
		_UnEquip.onClick += UnEquip;
		_Buy.onClick += Buy;
		_Close.onClick += Close;
		transform.localScale = new Vector3 (0.1f, 0.1f,0.1f);
	}

	private void Plus(GameObject sender)
	{

		if (isEquip) {
			if (isWeapon) {
				if (weaponCount < PlayerInfo.Instance.inventory.ReturnWeapons (weapon)) {
					weaponCount++;
				}
			}
		} else {
			weaponCount++;
		
		}
		if (!isEquip) {
			int i = 0;
			if (isWeapon) {
				i = weaponCount * ConfigWeapons.Weapon [weapon].PriceInCoin;
			} else {
				i = weaponCount * ConfigUpgrades.Upgrades [upgrades].Cost;
			}
			moneyCount = i;

			_moneyNumber.text = i.ToString ();
		
		}
		_weaponNumber.text = weaponCount.ToString ();
	}
	private void Remove(GameObject sender)
	{
		if (weaponCount > 1) {
			weaponCount--;
			int i = 0;
			if (isWeapon) {
				i = weaponCount * ConfigWeapons.Weapon [weapon].PriceInCoin;
			} else {
				i = weaponCount * ConfigUpgrades.Upgrades [upgrades].Cost;
			}
			if (!isEquip) {
				moneyCount = i;
				_moneyNumber.text = i.ToString ();
			
			}
			_weaponNumber.text = weaponCount.ToString ();
		}
	}


	private void Close(GameObject sender)
	{
		Hide ();
		//gameObject.SetActive (false);
	}
	private void Buy(GameObject sender)
	{
		if (!PlayerInfo.Instance.inventory.MoneyChange (moneyCount)) {
		
			Debug.Log("Not MOney");
			return;
		}


		if (isWeapon) {
			PlayerInfo.Instance.inventory.BuyWeapon (weapon, weaponCount);
		} else {
			PlayerInfo.Instance.inventory.BuyUpgrades (upgrades,weaponCount);
		}

		Hide ();
	}
	private void Equip(GameObject sender)
	{
		if (isWeapon)
		{
			if (PlayerInfo.Instance.inventory.BuyWeapon (weapon, (weaponCount * -1))) {
					UIShipsTacticPanel.Instance.ActiveShip.EquipAdvanceWeapon (weapon, weaponCount);
					Hide ();
					}
			else {
				Debug.Log ("error");
			}
		} 
		else
		{
				
		}
	
	}
	private void UnEquip(GameObject sender)
	{
		
	}


	private void ChangeIcon(WeaponsType type)
	{

				switch (type) {
				case WeaponsType.Missile:
						_weapon.spriteName = "wp_rocket1";
						break;
				case WeaponsType.HomingMissile:
						_weapon.spriteName = "wp_rocket";
						break;
				case WeaponsType.OneRicochet:
						_weapon.spriteName = "wp_ricoshet1";
						break;
				case WeaponsType.TwoRicochet:
						_weapon.spriteName = "wp_ricoshet";
						break;
				case WeaponsType.Napalm:
						_weapon.spriteName = "wp_napalm";
						break;
				case WeaponsType.SuperTorpedo:
						_weapon.spriteName = "wp_torpedo";
						break;
				case WeaponsType.FrozenProjectile:
						_weapon.spriteName = "wp_icecube";
						break;
				case WeaponsType.DeepBomb:
						_weapon.spriteName = "wp_mine";
						break;
				case WeaponsType.Construct:
						_weapon.spriteName = "wp_construct";
						break;
				case WeaponsType.AdvanceConstruct:
						_weapon.spriteName = "wp_construct1";
						break;
				case WeaponsType.Energy:
						_weapon.spriteName = "wp_energy1";
						break;
				case WeaponsType.AdvanceEnergy:
						_weapon.spriteName = "wp_energy";
						break;
				}
		}
	private void ChangeIcon(UpgradesType type)
	{
		switch(type){
		case UpgradesType.Accelerator:
			_weapon.spriteName = "up_turbo_speed2";
			break;
		case UpgradesType.AcceleratorInf:
			_weapon.spriteName = "up_turbo_speed";
			break;
		case UpgradesType.Armor:
			_weapon.spriteName = "wp_armore";
			break;
		case UpgradesType.ArmorAdvance:
			_weapon.spriteName = "wp_armore1";
			break;
		case UpgradesType.FastShell:
			_weapon.spriteName = "up_fast_wp";
			break;
		case UpgradesType.FastShipShell:
			_weapon.spriteName = "up_speed_core";
			break;
		case UpgradesType.FastTorpede:
			_weapon.spriteName = "up_fast_torpedo";
			break;
		case UpgradesType.IceHouseDestroy:
			_weapon.spriteName = "up_icecream";
			break;
		case UpgradesType.None:
			_weapon.spriteName = "wp1";
			break;
		case UpgradesType.RapidShot:
			_weapon.spriteName = "up_speed_canon";
			break;
		}
	}

	private void Hide()
	{
		StartCoroutine ("HideCour");
	}


	
	IEnumerator HideCour()
	{
		for(float i = 1;i>0;i-=0.1f)
		{
			
			transform.localScale = new Vector3 (i, i,0.1f);
			
			if(i<=0)
			{
				StopCoroutine("HideCour");
			}
			yield return null;
		}
	}

	IEnumerator Show()
	{
		for(float i = 0.1f;i<=1;i+=0.1f)
		{

			transform.localScale = new Vector3 (i, i,0.1f);

			if(i>=0.9)
			{
				transform.localScale = new Vector3 (1, 1,0.1f);
				StopCoroutine("Show");
			}
			yield return null;
		}
	}

	public void ShowEquip(WeaponsType type)
	{
		StartCoroutine ("Show");

		isWeapon = true;
		isEquip = true;
		weapon = type;
		
		ChangeIcon(type);
		buy.SetActive (false);
		equip.SetActive (true);
		unEquip.SetActive (false);
		int weaponNumber = PlayerInfo.Instance.inventory.ReturnWeapons (type);
		weaponCount = weaponNumber;
		moneyCount = 0;
		_weaponNumber.text = weaponCount.ToString ();
		_moneyNumber.text = moneyCount.ToString ();
		_descrTitle.text = type.ToString ();
		_descrLabel.text = ConfigWeapons.Weapon [type].Description;
	}
	public void ShowEquip(UpgradesType type)
	{
		isWeapon = false;
		isEquip = true;
		ChangeIcon (type);

		buy.SetActive (true);
		equip.SetActive (true);
		unEquip.SetActive (false);
		weaponCount = 0;
		moneyCount = 0;
		_weaponNumber.text = weaponCount.ToString ();
		_moneyNumber.text = moneyCount.ToString ();

	}


	public void ShowBuy(WeaponsType type)
	{
		StartCoroutine ("Show");

		isWeapon = true;
		isEquip = false;
		weapon = type;

		ChangeIcon(type);
		buy.SetActive (true);
		equip.SetActive (false);
		unEquip.SetActive (false);

		weaponCount = 1;
		_weaponNumber.text = weaponCount.ToString ();
		int i = 0;
		if(isWeapon){
			i = weaponCount * ConfigWeapons.Weapon[weapon].PriceInCoin;
		}
		else 
		{
			i = weaponCount * ConfigUpgrades.Upgrades[upgrades].Cost;
		}
		moneyCount  = i;
		
		_moneyNumber.text = i.ToString();
		_weaponNumber.text = weaponCount.ToString ();


		_descrTitle.text = type.ToString ();
		_descrLabel.text = ConfigWeapons.Weapon [type].Description;


		}
	public void ShowBuy(UpgradesType type)
	{
		Debug.Log (type);

		StartCoroutine ("Show");
		isWeapon = false;
		isEquip = false;
		upgrades = type;

	

		buy.SetActive (true);
		equip.SetActive (false);
		unEquip.SetActive (false);
		weaponCount = 1;
		_weaponNumber.text = weaponCount.ToString ();
		int i = 0;
		if(isWeapon){
			i = weaponCount * ConfigWeapons.Weapon[weapon].PriceInCoin;
		}
		else 
		{
			i = weaponCount * ConfigUpgrades.Upgrades[upgrades].Cost;
		}
		moneyCount  = i;
		
		_moneyNumber.text = i.ToString();
		_weaponNumber.text = weaponCount.ToString ();
		_descrTitle.text = type.ToString ();
		ChangeIcon (upgrades);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
