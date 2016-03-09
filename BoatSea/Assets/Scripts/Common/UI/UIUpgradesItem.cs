using UnityEngine;
using System.Collections;
using Aratog.NavyFight.Models.Unity3D.Weapons;

public class UIUpgradesItem : MonoBehaviour {

	public UpgradesType _type;
	
	public UIEventListener _buy;
	public UIEventListener _equip;
	
	public UILabel _itemNumber;

	private UISprite _equipSprite;
	private UILabel _equipLabel;

	//true - текущее положение unequip
	private bool _IsEquip = false;


	// Use this for initialization
	void Start () {
		_buy = transform.FindChild ("buy_btn").gameObject.AddComponent<UIEventListener>();
		_buy.onClick += BuyItem;
		
		_equip = transform.FindChild ("equip_unequip_btn").gameObject.AddComponent<UIEventListener> ();
		_equip.onClick += EquipItem;
		

		
		_itemNumber = transform.FindChild("weapon_slot").transform.FindChild("Label").gameObject.GetComponent<UILabel>();
		_itemNumber.text = PlayerInfo.Instance.inventory.ReturnUpgrades(_type).ToString();

		_equipSprite = transform.FindChild("equip_unequip_btn").FindChild ("Sprite (equip_btn)").gameObject.GetComponent<UISprite> ();
		_equipLabel = transform.FindChild ("equip_unequip_btn").FindChild ("Label").gameObject.GetComponent<UILabel> ();

		
	}

	public void CheckInventory()
	{
		Debug.Log ("Check");
		if (UIShipsTacticPanel.Instance.ActiveShip.CheckUpgrade (_type)) {
			equipButtonUnEquip();
		} else {
			_IsEquip = false;
			_equipSprite.color = new Color (1, 1, 1, 1);
			_equipLabel.text = LocalizationConfig.getText("equip");
		}

	}

	
	void BuyItem(GameObject sender)
	{

		UiPurchasePanel.Instance.ShowBuy (_type);
	//	PlayerInfo.Instance.inventory.BuyUpgrades (_type,1);
		Debug.Log ("BUY");
//		_itemNumber.text = PlayerInfo.Instance.inventory.ReturnUpgrades(_type).ToString();
		
		
	}

	void equipButtonEquip()
	{
		Debug.Log ("equip");
		UIShipsTacticPanel.Instance.ActiveShip.UnEquipUpgrade (_type);
		_IsEquip = false;
		_equipSprite.color = new Color (1,1,1,1);
		_equipLabel.text = LocalizationConfig.getText("equip");
	}

	void equipButtonUnEquip()
	{
		_IsEquip = true;
		Debug.Log ("uneq");
		_equipSprite.color = new Color (0.5f,0.5f,0.5f,1);
		_equipLabel.text = LocalizationConfig.getText("Unequip");
	}


	void EquipItem(GameObject sender)
	{
		if (!_IsEquip) {
			if (UIShipsTacticPanel.Instance.ActiveShip.EquipUpgrade (_type)) {
				equipButtonUnEquip ();
			}
		} 
		else 
		{
			equipButtonEquip();
		}


		
	}
	void Update()
	{
		_itemNumber.text = PlayerInfo.Instance.inventory.ReturnUpgrades(_type).ToString();
	}




	
}
