using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Ship;

public class UIShipsSettingsPanel : BasicPanel<UIShipsSettingsPanel> {
	List<UIShipItem> _ships;
	public UIShipItem activeShip;

	public List<UIShipItem> Ships{
		get{return _ships;}
	}
	void Start () {
		_ships=new List<UIShipItem>(gameObject.GetComponentsInChildren<UIShipItem>());
			foreach(var shipItem in _ships){
				shipItem.OnShipItemClick+=SetActiveShip;
			}
		LoadFakeShips();
	}
	void SetActiveShip(UIShipItem sender){
//		print("shipitemclick");
		foreach(var shipItem in _ships){
			if (shipItem==sender){
				shipItem.IsActive=true;
				activeShip=shipItem;
				UIShipSelectionPanel.Instance.SetShipMesh((int)activeShip.Type);
				continue;
			}
			shipItem.IsActive=false;
			shipItem.ReversTacticTweener();
		}
	}
    
	public override void OnTweenFinished(UITweener tween){
		
	}
	public override void Show(){
		//if(IsActive)return;
		base.Show();
		UITopPanel.Instance.leftPanelTitleLbl.text=  LocalizationConfig.getText("Your team");
	}
	void LoadFakeShips(){

		Debug.Log ("Read Config Ships"+ConfigShips.Ships.Count);

		//TODO:: need return Big Ship Setting
		_ships[0].Type=ShipsGlobal.ShipType.Small;
		_ships[0].shipTactic = AITactic.BaseDefence;
		
		
		_ships[1].Type=ShipsGlobal.ShipType.Small;
		_ships[1].shipTactic = AITactic.BaseDefence;
		
		
		_ships[2].Type=ShipsGlobal.ShipType.Middle;
		_ships[2].shipTactic = AITactic.BaseDefence;
		
		
		_ships[3].Type=ShipsGlobal.ShipType.Big;
		_ships[3].shipTactic = AITactic.BaseDefence;

	

		for(int i=0; i < 4 ;i++)
		{

			if(_ships[i].Type == ShipsGlobal.ShipType.Small)
			{
				_ships[i].Speed = ConfigShips.Ships[ShipType.Boat].Speed;
				_ships[i].Mines = ConfigShips.Ships[ShipType.Boat].BombCount;
				_ships[i].Rate = ConfigShips.Ships[ShipType.Boat].RotationSpeed;
				_ships[i].Armor = ConfigShips.Ships[ShipType.Boat].Health;
				Debug.Log(_ships[i].Armor);
			}
			else if(_ships[i].Type == ShipsGlobal.ShipType.Middle)
			{
				_ships[i].Speed = ConfigShips.Ships[ShipType.Submarine].Speed;
				_ships[i].Mines = ConfigShips.Ships[ShipType.Submarine].BombCount;
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
}
