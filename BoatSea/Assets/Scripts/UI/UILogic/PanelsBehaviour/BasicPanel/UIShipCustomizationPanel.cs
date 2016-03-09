using UnityEngine;
using System.Collections;
using ShipsGlobal;
public class UIShipCustomizationPanel : BasicPanel<UIShipCustomizationPanel>
{
    [SerializeField]
    private UILabel _speedLabel;
    [SerializeField]
    private UILabel _rateLabel;
    [SerializeField]
    private UILabel _armorLabel;
    [SerializeField]
    private UILabel _minesLabel;

    [SerializeField]
    private UILabel _nameLabel;

    [SerializeField]
    private GameObject _captainIco;
    [SerializeField]
    private GameObject _anchorIco;


	[SerializeField]
	UIEventListener nextShipBtn;
	[SerializeField]
	UIEventListener prevShipBtn;

    [SerializeField]
	GameObject[] shipsMeshes;

    [SerializeField]
    private UIShipItem _currentShip;
	void Start () {
	//	nextShipBtn.onClick+=OnNextShipBtnClick;
	//	prevShipBtn.onClick+=OnPrevShipBtnClick;

	}

	void OnNextShipBtnClick(GameObject sender){
		/*var currShipType=UIShipsTacticPanel.Instance.activeShip.Type;
		UIShipsTacticPanel.Instance.activeShip.Type=(ShipType)(((int)currShipType+1<3)?(int)currShipType+1:0);
		SetShipMesh((int)UIShipsTacticPanel.Instance.activeShip.Type);*/
	    UIShipsTacticPanel.Instance.NextShip();
        UpdateView();


	}
	void OnPrevShipBtnClick(GameObject sender){
		/*var currShipType=UIShipsTacticPanel.Instance.ActiveShip.Type;
		UIShipsTacticPanel.Instance.ActiveShip.Type=(ShipType)(((int)currShipType-1>=0)?(int)currShipType-1:2);
		SetShipMesh((int)UIShipsTacticPanel.Instance.ActiveShip.Type);*/
        UIShipsTacticPanel.Instance.PreviousShip();
        UpdateView();
	}
    void OnSelectShipBtnClick(GameObject sender)
    {
        
    }
	public void SetShipMesh(int i){
		foreach(var ship in shipsMeshes){
			ship.SetActive(false);
		}
	//	shipsMeshes[i].SetActive(true);
	}

    public void UpdateView()
    {
        if (UIShipsTacticPanel.Instance.ActiveShip.IsHumanControls)
        {
            _anchorIco.SetActive(false);
            _captainIco.SetActive(true);
            _nameLabel.text = "Captain";
        }
        else
        {
            _anchorIco.SetActive(true);
            _captainIco.SetActive(false);
            _nameLabel.text = "Support";
        }
        _rateLabel.text = UIShipsTacticPanel.Instance.ActiveShip.Rate.ToString();
        _speedLabel.text = UIShipsTacticPanel.Instance.ActiveShip.Speed.ToString();
        SetShipMesh((int)UIShipsTacticPanel.Instance.ActiveShip.Type);

    }

	public override void Show(){
		//if(IsActive)return;
		base.Show();
        /*if(!UITopPanel.Instance.rightPanelTitle.activeSelf)UITopPanel.Instance.rightPanelTitle.SetActive(true);
		UITopPanel.Instance.rightPanelTitleLbl.text="Ships";*/
        UpdateView();

        SoundController.PlayMenuOpen();
	}
	public override void Hide(){
		//if(!IsActive)return;
		base.Hide ();
		//UITopPanel.Instance.rightPanelTitleLbl.text="";
		/*UITopPanel.Instance.rightPanelTitle.SetActive(false);*/
	}

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.ShipSelection;
	}
}
