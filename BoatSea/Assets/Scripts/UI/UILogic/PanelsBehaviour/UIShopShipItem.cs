using System.Runtime.InteropServices;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;
using Aratog.NavyFight.Models.Ships;
using System;
public class UIShopShipItem : MonoBehaviour
{
    [SerializeField]
    private ShipsGlobal.ShipType _type;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _armor;
    [SerializeField]
	private float _rate;
    [SerializeField]
    private int _mines;


    [SerializeField]
    private UISprite _shipIcon;
    [SerializeField]
    private UILabel _speedLabel;
    [SerializeField]
    private UILabel _armorLabel;
    [SerializeField]
    private UILabel _minesLabel;
    [SerializeField]
    private UILabel _rateLabel;

    [SerializeField]
    private UIEventListener _selectBtn;
	[SerializeField]
	private UIEventListener _selectShip;

	[SerializeField]
	private GameObject _buyButton;

	[SerializeField]
	private UIEventListener _buyButtonEvent;

	private bool isBuy = false;

	private int indexOfType;
	// Use this for initialization
	void Start ()
	{
		switch (_type) {
		case ShipsGlobal.ShipType.Small:
			_speed =ConfigShips.Ships[ShipType.Boat].Speed;
			_armor =ConfigShips.Ships[ShipType.Boat].Health;
			_mines = ConfigShips.Ships[ShipType.Boat].BombCount;
			_rate  = ConfigShips.Ships[ShipType.Boat].RotationSpeed;
		
			break;
		case ShipsGlobal.ShipType.Middle:
			_speed = ConfigShips.Ships[ShipType.Boat].Speed;
			_armor = ConfigShips.Ships[ShipType.Boat].Health;
			_mines = ConfigShips.Ships[ShipType.Boat].BombCount;
			_rate  = ConfigShips.Ships[ShipType.Boat].RotationSpeed;
			
			break;
		case ShipsGlobal.ShipType.Big:
			_speed = ConfigShips.Ships[ShipType.Boat].Speed;
			_armor = ConfigShips.Ships[ShipType.Boat].Health;
			_mines = ConfigShips.Ships[ShipType.Boat].BombCount;
			_rate  = ConfigShips.Ships[ShipType.Boat].RotationSpeed;
			
			break;
		}
		indexOfType = (int) UIControllerForNGUI.GetNewShipType(_type);
		initBuyButton ();
		
		CheckIsAvailebel ();

		
		_selectBtn.onClick += OnSelectBtnClick;
		_selectShip.onClick += OnSelectBtnClick;
	    _speedLabel.text = _speed.ToString();
        _armorLabel.text = _armor.ToString();
        _minesLabel.text = _mines.ToString();
        _rateLabel.text = _rate.ToString();
		_shipIcon.spriteName = UIShipItem.GetSpriteShipName(_type, TeamColor.BlueTeam);
        _shipIcon.MakePixelPerfect();
	}


	private void OnBuyBtnClick(GameObject sender)
	{
		ShipType type = UIControllerForNGUI.GetNewShipType (_type);
		int cost = ConfigShips.Ships[type].Price;
		if (!PlayerInfo.Instance.inventory.MoneyChange (cost))
			return;



		PlayerInfo.Instance.BuyShip(indexOfType);
		isBuy = true;
		CheckIsAvailebel ();
	}

	private void initBuyButton()
	{
		//find and add all needes part to buy button

		_buyButton = transform.FindChild ("BuyBtn").gameObject;
		_buyButtonEvent = _buyButton.transform.FindChild ("Sprite (counter_btn)").GetComponent<UIEventListener> ();
		_buyButtonEvent.onClick += OnBuyBtnClick;
		UILabel label = _buyButton.transform.FindChild("Label").GetComponent<UILabel>();
		ShipType type = UIControllerForNGUI.GetNewShipType (_type);
		label.text = ConfigShips.Ships [type].Price.ToString();

	}

	private void CheckIsAvailebel()
	{
	
		if (PlayerInfo.Instance.ShipOnActivation [indexOfType]) {
			isBuy = true;
			_buyButton.SetActive(false);
			_selectBtn.gameObject.SetActive (true);
		} else {
		
			_selectBtn.gameObject.SetActive (false);
			_buyButton.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateColor()
	{
		TeamColor team = GameSetObserver.Instance.Human != null ? GameSetObserver.Instance.Human.Team : TeamColor.BlueTeam;

		_shipIcon.spriteName = UIShipItem.GetSpriteShipName(_type, team);
        _shipIcon.MakePixelPerfect();
	}

    void OnSelectBtnClick(GameObject sender)
    {
		if (isBuy) {
			UIShipsTacticPanel.Instance.ActiveShip.Speed = _speed;
			UIShipsTacticPanel.Instance.ActiveShip.Armor = _armor;
			UIShipsTacticPanel.Instance.ActiveShip.Rate = _rate;
			UIShipsTacticPanel.Instance.ActiveShip.Mines = _mines;
			UIShipsTacticPanel.Instance.ActiveShip.Type = _type;

			PlayerInfo.Instance.ShipSave [UIShipsTacticPanel.Instance.ActiveShip.id].Type = _type;

			UIShipCustomizationPanel.Instance.UpdateView ();
		}
    }
}
