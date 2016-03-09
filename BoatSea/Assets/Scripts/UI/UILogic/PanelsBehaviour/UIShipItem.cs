//using UnityEditor;

using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using ShipsGlobal;
using Aratog.NavyFight.Models.Unity3D.Weapons;

public class UIShipItem: MonoBehaviour
{
    #region View Variables

    public static Dictionary<AIType, string> AiTypeToString = new Dictionary<AIType, string>
        {
            { AIType.BaseDefense, "defense" },
            { AIType.CaptureEnemy, "attack" },
            };

    public event Action<UIShipItem> OnShipItemClick;

	public int id;

	[SerializeField]
	private GameObject _activateFrame;
    [SerializeField]
    private UIEventListener _shipSelectionBtn;

    [SerializeField]
    private UISprite shipIcon;

    [SerializeField]
    private UIEventListener _equipBtn;

    [SerializeField]
    private UIEventListener _tacticBtn;

    [SerializeField]
    private UITweener _tacticTweener;

    [SerializeField]
    private UITacticSelection _tacticSelection;

    [SerializeField]
    private UILabel _speedLabel;

    [SerializeField]
    private UILabel _armorLabel;

    [SerializeField]
    private UILabel _minesLabel;

    [SerializeField]
    private UILabel _rateLabel;

    [SerializeField]
    private GameObject _humanControlImage;

	[SerializeField]
	private UISprite _advanceWeaponImage;

	[SerializeField]
	private UILabel _advanceWeaponLabel;

    private bool _isHumanControls;

	private TeamColor team;


	public WeaponsType advanceWeapon;
	

	public void SetShipColor(TeamColor color)
	{
		team = color;
	}

    private AITactic _shipTactic;
    public AITactic shipTactic
    {
        get { return _shipTactic; }
        set
        {
            _shipTactic = value;

            //if (_shipTactic == AIType.HumanControls)
            //{
            //    _isHumanControls = true;
            //    _tacticSelection.gameObject.SetActive(false);
            //    _humanControlImage.gameObject.SetActive(true);
            //}
            //else
            //{
            //    _tacticSelection.gameObject.SetActive(true);
            //    _humanControlImage.gameObject.SetActive(false);
            //    _isHumanControls = false;
            //    _tacticSelection.CurrentSelection = AiTypeToString[_shipTactic];
            //}
        }
    }



	public int aWeaponNumber;


	public UpgradesType [] upgrades;
	public UISprite[] upgradesIcon; 


    public bool IsHumanControls
    {
        get { return _isHumanControls; }
        set
        {
            _isHumanControls = value;

            if (_isHumanControls == null)
            {
                return;
            }

            if (_isHumanControls)
            {
                //_shipTactic = AIType.HumanControls;
                _humanControlImage.SetActive(true);
                _tacticSelection.gameObject.SetActive(false);
            }
            else
            {
                _humanControlImage.SetActive(false);
                _tacticSelection.gameObject.SetActive(true);
            }
        }
    }

	public void HideIfNeed()
	{
		//_tacticSelection.ChangeScelection(_tacticSelection.CurrentSelection);
		UIButtonTween buttonTween = _tacticSelection.GetComponent<UIButtonTween>();

		if (buttonTween != null)
		{
			buttonTween.Play(false);
		}
	}


    private bool _isActive;

    public bool IsActive
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            //_activeFrame.SetActive(value);
            //_dimmer.SetActive(!value);
            //StartCoroutine(ActiveFrameBlink());
            //_activeFrame.GetComponent<UITweener>().enabled=value;
        }
    }

    #endregion

    #region Model Variables

    private int  _armor, _mines;

	private float _speed,_rate;

    private ShipType _type;

    public float Speed
    {
        get { return _speed; }
        set
        {
            _speed = value;
            _speedLabel.text = _speed.ToString();
        }
    }

    public int Armor
    {
        get { return _armor; }
        set
        {
            _armor = value;
            _armorLabel.text = _armor.ToString();
        }
    }

    public int Mines
    {
        get { return _mines; }
        set
        {
            _mines = value;
            _minesLabel.text = _mines.ToString();
        }
    }

    public float Rate
    {
        get { return _rate; }
        set
        {
            _rate = value;
            _rateLabel.text = _rate.ToString();
        }
    }

    public ShipType Type
    {
        set
        {
            _type = value;

			if(shipIcon){


			shipIcon.spriteName = GetSpriteShipName(_type, team);
			}
            shipIcon.MakePixelPerfect();
        }
        get { return _type; }
    }

	public void UpdateShipImage()
	{
		shipIcon.spriteName = GetSpriteShipName(_type, team);
        shipIcon.MakePixelPerfect();
	}

	public static string GetSpriteShipName(ShipType type, TeamColor color)
	{
		string str = "";

		switch (type)
		{
		/*	case ShipType.Big:
				str = color == TeamColor.OrangeTeam ? type.ToString() + "_ico" : "Battleship_ico_b";
				break;
			case ShipType.Middle:
				str = color == TeamColor.OrangeTeam ? type.ToString() + "_ico" : "Middle_ico"; //"Submarine_ico_b";
				break;
			case ShipType.Small:
				str = color == TeamColor.OrangeTeam ? type.ToString() + "_ico" : "Destroyer_ico_b";
				break;*/
		case ShipType.Big:
			str = "Linkor_Med_ico";
			break;
		case ShipType.Middle:
			str = "SM_Med_ico" ;
			break;
		case ShipType.Small:
			str = "Kater_Med_ico" ;
			break;
		case ShipType.BigMetal:
			str = "Linkor_MS_ico";
			break;
		case ShipType.MiddleMetal:
			str = "SM_MS_ico" ;
			break;
		case ShipType.SmallMetal:
			str = "Kater_MS_ico" ;
			break;
		case ShipType.BigAtlant:
			str = "Linkor_atlantis_ico";
			break;
		case ShipType.MiddleAtlant:
			str = "SM_Atlantis_ico" ;
			break;
		case ShipType.SmallAtlant:
			str = "Kater_atlantis_ico" ;
			break;
		case ShipType.BigDark:
			str = "Linkor_gothic_ico";
			break;
		case ShipType.MiddleDark:
			str = "SM_gothic_ico" ;
			break;
		case ShipType.SmallDark:
			str = "Kater_gothic_ico" ;
			break;
		}

		return str;
	}

    #endregion

    private void Start()
    {
        //shipType=ShipType.Small;
        _shipSelectionBtn.onClick += OnShipClick;

        _equipBtn.onClick += OnUnEquipBtnClick;
        _tacticSelection.OnStateChange += OnTacticChange;
        _tacticBtn.onClick += OnTacticBtnClick;



	//	_advanceWeaponImage = transform.FindChild("Sprite (wp1)").GetComponent<UISprite>();
	//	_advanceWeaponLabel = transform.FindChild("ammo").transform.FindChild("Label").GetComponent<UILabel>();
	
    }

    /*void OnShipSelection(GameObject sender){
		OnShipItemClick(gameObject.GetComponent<UIShipItem>());
		UIArmoryPanel.Instance.Hide();s
		UIShipCustomizationPanel.Instance.Show();
	}*/



	private void ChangeUpgradeIcon(int icon,UpgradesType type)
	{
		switch(type){
		case UpgradesType.Accelerator:
			upgradesIcon[icon].spriteName = "up_turbo_speed2";
			break;
		case UpgradesType.AcceleratorInf:
			upgradesIcon[icon].spriteName = "up_turbo_speed";
			break;
		case UpgradesType.Armor:
			upgradesIcon[icon].spriteName = "wp_armore";
			break;
		case UpgradesType.ArmorAdvance:
			upgradesIcon[icon].spriteName = "wp_armore1";
			break;
		case UpgradesType.FastShell:
			upgradesIcon[icon].spriteName = "up_fast_wp";
			break;
		case UpgradesType.FastShipShell:
			upgradesIcon[icon].spriteName = "up_speed_core";
			break;
		case UpgradesType.FastTorpede:
			upgradesIcon[icon].spriteName = "up_fast_torpedo";
			break;
		case UpgradesType.IceHouseDestroy:
			upgradesIcon[icon].spriteName = "up_icecream";
			break;
		case UpgradesType.None:
			upgradesIcon[icon].spriteName = "wp1";
			break;
		case UpgradesType.RapidShot:
			upgradesIcon[icon].spriteName = "up_speed_canon";
			break;
		}
		
		}

	private bool CheckElseUpgrades(UpgradesType type)
	{
		for (int i=0; i<upgrades.Length; i++) {
			if(upgrades [i]== type)
			{
				return false;
			}
		}
		return true;
		}

	public void EquipUpgradeFromSave(UpgradesType type,int value = 0)
	{

		for (int i=0; i<upgrades.Length; i++) {

			if (upgrades [i] == UpgradesType.None)
			{

				if(CheckElseUpgrades(type)){
					Debug.Log("Ship id"+id+" add Upgrades in "+ i);
						upgrades [i] = type;
						ChangeUpgradeIcon (i, type);
						break;
				}
			}
		}
	}

	public bool UnEquipUpgrade(UpgradesType type)
	{
		
		for (int i=0; i<upgrades.Length; i++) {
			if (upgrades [i] == type)
			{
					if(PlayerInfo.Instance.inventory.BuyUpgrades(type,1))
					{
					if(type == UpgradesType.AcceleratorInf)
					{
						PlayerInfo.Instance.ShipSave[id].DeleteUpgrades(type);
					}
						upgrades [i] = UpgradesType.None;
						ChangeUpgradeIcon (i, UpgradesType.None);
						PlayerInfo.Instance.ShipSave[id].upgrades = upgrades;
						PlayerInfo.Instance.ShipSave[id].SaveUpgrades(UpgradesType.None,i);

						return true;
						
					}
			}
		}
		return false;
	}


	public bool CheckUpgrade(UpgradesType type)
	{
		
		for (int i=0; i<upgrades.Length; i++) {
			if (upgrades [i] == type)
			{
				return true;
			}
		}
		return false;
	}


	public bool EquipUpgrade(UpgradesType type,int value = 0)
	{

		for (int i=0; i<upgrades.Length; i++) {
			if (upgrades [i] == UpgradesType.None)
			{

				if(CheckElseUpgrades(type)){

					if(PlayerInfo.Instance.inventory.BuyUpgrades(type,-1))
					{
						upgrades [i] = type;
						ChangeUpgradeIcon (i, type);
						PlayerInfo.Instance.ShipSave[id].upgrades = upgrades;
						PlayerInfo.Instance.ShipSave[id].SaveUpgrades(type,i);
						return true;

					}
				}
			}
		}
		return false;
	}

	private void weaponIconChange(WeaponsType type)
	{
		
		switch(type){
		case WeaponsType.Missile:
			_advanceWeaponImage.spriteName = "wp_rocket1";
			break;
		case WeaponsType.HomingMissile:
			_advanceWeaponImage.spriteName = "wp_rocket";
			break;
		case WeaponsType.OneRicochet:
			_advanceWeaponImage.spriteName = "wp_ricoshet1";
			break;
		case WeaponsType.TwoRicochet:
			_advanceWeaponImage.spriteName = "wp_ricoshet";
			break;
		case WeaponsType.Napalm:
			_advanceWeaponImage.spriteName = "wp_napalm";
			break;
		case WeaponsType.SuperTorpedo:
			_advanceWeaponImage.spriteName = "wp_torpedo";
			break;
		case WeaponsType.FrozenProjectile:
			_advanceWeaponImage.spriteName = "wp_icecube";
			break;
		case WeaponsType.DeepBomb:
			_advanceWeaponImage.spriteName = "wp_mine";
			break;
		case WeaponsType.Construct:
			_advanceWeaponImage.spriteName = "wp_construct";
			break;
		case WeaponsType.AdvanceConstruct:
			_advanceWeaponImage.spriteName = "wp_construct1";
			break;
		case WeaponsType.Energy:
			_advanceWeaponImage.spriteName = "wp_energy1";
			break;
		case WeaponsType.AdvanceEnergy:
			_advanceWeaponImage.spriteName = "wp_energy";
			break;
		}
		_advanceWeaponImage.transform.localScale = new Vector3 (30,30,1);
	}


	public void EquipAdvanceWeapon(WeaponsType type,int value)
	{
		Debug.Log ("UiShip Equip Weapon");
		if (type != advanceWeapon) {
			if (aWeaponNumber > 0) {
				if (PlayerInfo.Instance.inventory.BuyWeapon (advanceWeapon, aWeaponNumber)) {
					Debug.Log ("return Weapon to inventory");
				}
			}

			advanceWeapon = type;
			_advanceWeaponLabel.text = value.ToString ();
			aWeaponNumber = value;
			PlayerInfo.Instance.ShipSave [id].Weapon = type;
			PlayerInfo.Instance.ShipSave [id].weaponCount = value;
			
			weaponIconChange (type);	

		} else if (type == advanceWeapon){
			advanceWeapon = type;
			aWeaponNumber += value;
			_advanceWeaponLabel.text = aWeaponNumber.ToString ();

			PlayerInfo.Instance.ShipSave [id].Weapon = type;
			PlayerInfo.Instance.ShipSave [id].weaponCount = aWeaponNumber;
			
			weaponIconChange (type);	
		}


	}

	public void EquipAdvanceWeaponFromSave(WeaponsType type,int value)
	{
		advanceWeapon = type;
		_advanceWeaponLabel.text = value.ToString ();
		aWeaponNumber = value;
		weaponIconChange (type);	
		PlayerInfo.Instance.ShipSave [id].Weapon = type;
		PlayerInfo.Instance.ShipSave [id].weaponCount = value;
	}

	private void OnUnEquipBtnClick(GameObject sender)
	{
		if (aWeaponNumber > 0) {
			if (PlayerInfo.Instance.inventory.BuyWeapon (advanceWeapon, aWeaponNumber)) {
				Debug.Log ("return Weapon to inventory");
			}
		}
		
		advanceWeapon = WeaponsType.Missile;
		aWeaponNumber = 0;
		_advanceWeaponLabel.text = aWeaponNumber.ToString ();

		PlayerInfo.Instance.ShipSave [id].Weapon = advanceWeapon;
		PlayerInfo.Instance.ShipSave [id].weaponCount = aWeaponNumber;
		
		weaponIconChange (advanceWeapon);	
	}
    private void OnEquipBtnClick(GameObject sender)
    {
		if (OnShipItemClick != null)
		{
			OnShipItemClick(this);
		}
		UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() { UIShipsTacticPanel.Instance, UITacticMapPanel.Instance });
		UITacticMapPanel.Instance.Hide();
		UIArmoryPanel.Instance.Show();
		UIMissionDetails.Instance.Hide(); 
    }





	public void EventInitialize()
	{
		if (UIManager.Instance != null)
			_shipSelectionBtn.onClick += UIManager.Instance.OnShipSelectionBtn;
		else 
			Debug.LogError("UIManager Instance has't initialize and equals null");
	}

    private void OnTacticBtnClick(GameObject sender)
    {
        OnShipItemClick(gameObject.GetComponent<UIShipItem>());
		//_tacticSelection.
    }

    public void ReversTacticTweener()
    {
        _tacticTweener.Play(false);
    }

	public void DeactivateChooseImage()
	{

		_activateFrame.SetActive (false);
	}

	public void ActivateChooseImage()
	{
	
		_activateFrame.SetActive (true);
	}

    private void OnTacticChange(string tactic)
    {
        SoundController.PlaySerYesSer();

        switch (tactic)
        {
            case "your tactics":
            {
                //shipTactic = AIType.GuardZone;
                shipTactic = AITactic.BaseDefence;
                break;
            }
            case "attack":
            {
                //shipTactic = AIType.CaptureEnemy;
                shipTactic = AITactic.CaptureEnemy;
                break;
            }
            case "follow":
            {
                //shipTactic = AIType.FollowMe;
                shipTactic = AITactic.FollowMe;
                break;
            }
            case "defense":
            {
                //shipTactic = AIType.BaseDefense;
                shipTactic = AITactic.BaseDefence;
                break;
            }
        }
    }

    private void OnShipClick(GameObject sender)
    {
        if (OnShipItemClick != null)
        {
            OnShipItemClick(this);
        }
      //  UITopPanel.Instance.AddPrevPanels(new List<IMenuInterface>() { UIShipsTacticPanel.Instance, UITacticMapPanel.Instance });
      //  UIShipsTacticPanel.Instance.Hide();
        UITacticMapPanel.Instance.Hide();
     //   UIShipCustomizationPanel.Instance.Show();
        UIArmoryPanel.Instance.Show();
		UIMissionDetails.Instance.Hide(); 
    }
}
