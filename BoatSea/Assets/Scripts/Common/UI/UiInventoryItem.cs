using UnityEngine;
using System.Collections;
using Aratog.NavyFight.Models.Unity3D.Weapons;

public class UiInventoryItem : MonoBehaviour {

	public WeaponsType _type;

	public UIEventListener _buy;
	public UIEventListener _equip;

	public UILabel _itemNumber;


	// Use this for initialization
	void Start () {
		_buy = transform.FindChild ("buy_btn").gameObject.AddComponent<UIEventListener>();
		_buy.onClick += BuyItem;

		_equip = transform.FindChild ("equip_unequip_btn").gameObject.AddComponent<UIEventListener> ();
		_equip.onClick += EquipItem;

		_itemNumber = transform.FindChild("weapon_slot").transform.FindChild("Label").gameObject.GetComponent<UILabel>();

		string text = PlayerInfo.Instance.inventory.ReturnWeapons(_type).ToString();

		_itemNumber.text = text;

		UISprite weaponSprite = transform.FindChild ("weapon_slot").FindChild ("Sprite (wp2)").GetComponent<UISprite> ();
		weaponSprite.spriteName = ShipWeaponIcon();

		UILabel label = transform.FindChild("title").FindChild("Label").GetComponent<UILabel>();
		label.text = _type.ToString ();

	}


	private string ShipWeaponIcon()
	{
		string weaponName = "";
		switch (_type) {
		case WeaponsType.Missile:
			weaponName = "wp_rocket1";
			break;
		case WeaponsType.HomingMissile:
			weaponName = "wp_rocket";
			break;
		case WeaponsType.OneRicochet:
			weaponName = "wp_ricoshet1";
			break;
		case WeaponsType.TwoRicochet:
			weaponName = "wp_ricoshet";
			break;
		case WeaponsType.Napalm:
			weaponName = "wp_napalm";
			break;
		case WeaponsType.SuperTorpedo:
			weaponName = "wp_torpedo";
			break;
		case WeaponsType.FrozenProjectile:
			weaponName = "wp_icecube";
			break;
		case WeaponsType.DeepBomb:
			weaponName = "wp_mine";
			break;
		case WeaponsType.Construct:
			weaponName = "wp_construct";
			break;
		case WeaponsType.AdvanceConstruct:
			weaponName = "wp_construct1";
			break;
		case WeaponsType.Energy:
			weaponName = "wp_energy1";
			break;
		case WeaponsType.AdvanceEnergy:
			weaponName = "wp_energy";
			break;
		}
		return weaponName;
	}


	void BuyItem(GameObject sender)
	{
		//GameInventory.Instance.BuyWeapon (_type,1);
		UiPurchasePanel.Instance.ShowBuy (_type);
		Debug.Log ("BUY");
	//	_itemNumber.text = GameInventory.Instance.ReturnWeapons(_type).ToString();


	}
	void EquipItem(GameObject sender)
	{
		UiPurchasePanel.Instance.ShowEquip (_type);
	//	UIShipsTacticPanel.Instance.ActiveShip.EquipAdvanceWeapon (_type,GameInventory.Instance.ReturnWeapons(_type));
	//	UIShipsTacticPanel.Instance.ActiveShip
		Debug.Log ("Equip");

	}

	void Update()
	{
		//TODO: need optimization
		_itemNumber.text = PlayerInfo.Instance.inventory.ReturnWeapons(_type).ToString();
	}

}
