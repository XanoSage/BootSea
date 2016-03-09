using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using ShipsGlobal;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Weapons;
public class UIShipsTacticPanel : BasicPanel<UIShipsTacticPanel> {

	public bool isCampaign = false;
	public GameObject boxColider;

	public List<UIShipItem> _ships;
	public UIShipItem ActiveShip;

	private bool IsEventInitialize = false;

	public List<UIShipItem> UIShips;

	public List<UIShipItem> Ships
	{
	   get
		{

			return _ships;}
	    set { _ships = value; }
	}

    void Start () {

		isCampaign = false;
		Ships=new List<UIShipItem>(gameObject.GetComponentsInChildren<UIShipItem>());
			foreach(var shipItem in Ships){
				shipItem.OnShipItemClick+=SetActiveShip;
			}
	
		LoadFakeShips();

	    foreach (var uiShipItem in UIShips)
	    {
		    uiShipItem.gameObject.SetActive(false);
	    }
		ActiveShip = UIShips [0];
		ActiveShip.ActivateChooseImage ();

	}
	void SetActiveShip(UIShipItem sender){
	
		if (ActiveShip != null && ActiveShip != sender)
		{
			ActiveShip.HideIfNeed();
		}

		ActiveShip.DeactivateChooseImage ();

		sender.ActivateChooseImage ();
	    ActiveShip = sender;
	}
    
	public override void OnTweenFinished(UITweener tween){
		
	}

	public override void Hide () {
	
		base.Hide();
		foreach (var uiShipItem in UIShips)
		{
			uiShipItem.HideIfNeed();
			uiShipItem.gameObject.SetActive(false);
		}
	}

	public override void Show()
	{


		Debug.Log("show");
		//Load Saved Ships  
		for (int i=0; i<4; i++)
		{
			Ships[i].upgrades = new UpgradesType[4];
			Ships [i].EquipAdvanceWeaponFromSave(PlayerInfo.Instance.ShipSave [i].Weapon,
			                                     PlayerInfo.Instance.ShipSave [i].weaponCount);
			Ships [i].EquipUpgradeFromSave(PlayerInfo.Instance.ShipSave [i].upgrades[0],0);
			Ships [i].EquipUpgradeFromSave(PlayerInfo.Instance.ShipSave [i].upgrades[1],1);
			Ships [i].EquipUpgradeFromSave(PlayerInfo.Instance.ShipSave [i].upgrades[2],2);
			Ships [i].EquipUpgradeFromSave(PlayerInfo.Instance.ShipSave [i].upgrades[3],3);
			Ships [i].Type = PlayerInfo.Instance.ShipSave [i].Type;
		}

   SoundController.PlayMenuOpen();

		int playersCount = GameController.Instance.bootsCount + 1;//UIController.Instance.battle.SupportCount + 1;

		if (playersCount <= UIShips.Count)
			for (int i = 0; i < playersCount; i++)
			{
				if (UIShips[i] != null)
					UIShips[i].gameObject.SetActive(true);
			}

		//if(IsActive)return;
		base.Show();
		UITopPanel.Instance.leftPanelTitleLbl.text=  LocalizationConfig.getText("Your team");

		if (!IsEventInitialize)
		{
			foreach (var shipItem in _ships)
			{
				shipItem.EventInitialize();
			}

			UITacticMapPanel.Instance.SetupButton.onClick += OnSetupButton;
		}
		IsEventInitialize = true;

		foreach (UIShipItem uiShipItem in _ships)
		{
			uiShipItem.SetShipColor(GameController.Instance.Human.Team);
			uiShipItem.UpdateShipImage();
		}
	}

    public void NextShip()
    {

        ActiveShip = Ships.IndexOf(ActiveShip) < 3 ? Ships[Ships.IndexOf(ActiveShip) + 1] : Ships[1];
		Debug.Log ("next: "+ (Ships.IndexOf(ActiveShip) < 3 ? Ships[Ships.IndexOf(ActiveShip) + 1] : Ships[1]));

    }
    public void PreviousShip()
    {
        ActiveShip = Ships.IndexOf(ActiveShip) > 1 ? Ships[Ships.IndexOf(ActiveShip) - 1] : Ships[3];


    }


	void LoadFakeShips(){


	/*	_ships [0] = ConfigController.Instance.UiShips [0];
		Ships[1] = ConfigController.Instance.UiShips [1];
		Ships[2] = ConfigController.Instance.UiShips [2];;
		Ships[3] = ConfigController.Instance.UiShips [3];;*/
		//Old Ships Created
		//TODO:: need return Big Ship Setting
		_ships [0].Type = ShipsGlobal.ShipType.Small;
		_ships[0].shipTactic = AITactic.BaseDefence;
		_ships[0].Speed=ConfigShips.Ships[ShipType.Boat].Speed;
		_ships[0].Mines=ConfigShips.Ships[ShipType.Boat].BombCount;
		_ships[0].Rate=ConfigShips.Ships[ShipType.Boat].RotationSpeed;
		_ships[0].Armor=ConfigShips.Ships[ShipType.Boat].Health;

	

		Ships[1].Type=ShipsGlobal.ShipType.Small;
		Ships[1].shipTactic = AITactic.BaseDefence;

		
		Ships[2].Type=ShipsGlobal.ShipType.Middle;
		Ships[2].shipTactic = AITactic.BaseDefence;
	
		
		Ships[3].Type=ShipsGlobal.ShipType.Big;
		Ships[3].shipTactic = AITactic.BaseDefence;

		for(int i=1; i < 4 ;i++)
		{
			if(_ships[i].Type == ShipsGlobal.ShipType.Small)
			{
				_ships[i].Speed = ConfigShips.Ships[ShipType.Boat].Speed;
				_ships[i].Mines = ConfigShips.Ships[ShipType.Boat].BombCount;
				_ships[i].Rate =ConfigShips.Ships[ShipType.Boat].RotationSpeed;
				_ships[i].Armor = ConfigShips.Ships[ShipType.Boat].Health;
			}
			else if(_ships[i].Type == ShipsGlobal.ShipType.Middle)
			{
				_ships[i].Speed =ConfigShips.Ships[ShipType.Submarine].Speed;
				_ships[i].Mines =ConfigShips.Ships[ShipType.Submarine].BombCount;
				_ships[i].Rate = ConfigShips.Ships[ShipType.Submarine].RotationSpeed;
				_ships[i].Armor = ConfigShips.Ships[ShipType.Submarine].Health;
			}
			else if(_ships[i].Type == ShipsGlobal.ShipType.Big)
			{
				_ships[i].Speed = ConfigShips.Ships[ShipType.BigShip].Speed;
				_ships[i].Mines = ConfigShips.Ships[ShipType.BigShip].BombCount;
				_ships[i].Rate = ConfigShips.Ships[ShipType.BigShip].RotationSpeed;
				_ships[i].Armor = ConfigShips.Ships[ShipType.BigShip].Health;
			}
		}
		
	}

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.ShipsSettings;
	}

	private void OnSetupButton(GameObject sender)
	{
		// TODO:: add there code that will be set ship data using UIControllers data

		Debug.Log(string.Format("UIShipTacticPanel.OnSetupButton - OnSetup"));
		UIController.Instance.SetPlayerTeamShipsPreset(UIShips);

	}
}
