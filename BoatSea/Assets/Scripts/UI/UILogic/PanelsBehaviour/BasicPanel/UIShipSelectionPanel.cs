using UnityEngine;
using System.Collections;
using ShipsGlobal;
public class UIShipSelectionPanel : BasicPanel<UIShipSelectionPanel> {

	[SerializeField]
	UIEventListener nextShipBtn;
	[SerializeField]
	UIEventListener prevShipBtn;
    [SerializeField]
    UIEventListener selectShipBtn;
    [SerializeField]
	GameObject[] shipsMeshes;
	void Start () {
		nextShipBtn.onClick+=OnNextShipBtnClick;
		prevShipBtn.onClick+=OnPrevShipBtnClick;
	   // selectShipBtn.onClick += OnSelectShipBtnClick;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnNextShipBtnClick(GameObject sender){
		var currShipType=UIShipsSettingsPanel.Instance.activeShip.Type;
		UIShipsSettingsPanel.Instance.activeShip.Type=(ShipType)(((int)currShipType+1<3)?(int)currShipType+1:0);
		SetShipMesh((int)UIShipsSettingsPanel.Instance.activeShip.Type);

		
	}
	void OnPrevShipBtnClick(GameObject sender){
		var currShipType=UIShipsSettingsPanel.Instance.activeShip.Type;
		UIShipsSettingsPanel.Instance.activeShip.Type=(ShipType)(((int)currShipType-1>=0)?(int)currShipType-1:2);
		SetShipMesh((int)UIShipsSettingsPanel.Instance.activeShip.Type);
		
	}
    void OnSelectShipBtnClick(GameObject sender)
    {
        
    }
	public void SetShipMesh(int i){
		foreach(var ship in shipsMeshes){
			ship.SetActive(false);
		}
		shipsMeshes[i].SetActive(true);
	}
	public override void Show(){
		if(IsActive)return;
		base.Show();
        if(!UITopPanel.Instance.rightPanelTitle.activeSelf)UITopPanel.Instance.rightPanelTitle.SetActive(true);
		UITopPanel.Instance.rightPanelTitleLbl.text="Ships";
	}
	public override void Hide(){
		if(!IsActive)return;
		base.Hide ();
		//UITopPanel.Instance.rightPanelTitleLbl.text="";
		UITopPanel.Instance.rightPanelTitle.SetActive(false);
	}

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.ShipSelection;
	}
}
