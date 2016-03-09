using LinqTools;
using UnityEngine;
using System.Collections.Generic;

public enum TopPanelState
{
    GameList,
    MultiPlayerBattleSettings,
    Lobby
};
public class UITopPanel : BasicPanel<UITopPanel> {

	public UILabel moneyLabel,vipMoneyLabel;

	public UIEventListener tactickMap,inventory,campaign;


	public UIEventListener moreCoinsBtn;
	public UIMoreCoinsPopup moreCoinsPopup;
	public GameObject closeShipSelectionPanelBtn;
	public GameObject closeArmoryPanelBtn;
	public UILabel leftPanelTitleLbl,rightPanelTitleLbl;
	public GameObject rightPanelTitle,rightButtonCampaign,rightButtonCustomBatle;
	public UIEventListener mainMenuBtn,worldMapBtn,backBtn;
    public List<List<IMenuInterface>> PrevPanels=new List<List<IMenuInterface>>();
    public List<IMenuInterface> CurrentPanels=new List<IMenuInterface>(); 
	void Start(){
		tactickMap.onClick += OnTacktick;
		inventory.onClick += OnInventory;
		campaign.onClick += OnCampaign;

		moreCoinsBtn.onClick+=OnMoreCoinsBtnClick;
		mainMenuBtn.onClick+=OnMainMenuBtnClick;
		worldMapBtn.onClick+=OnWorldMapBtnClick;
        backBtn.onClick += OnBackBtnClick;
	}

	void Update () {
		moneyLabel.text = PlayerInfo.Instance.inventory.Money.ToString ();
		vipMoneyLabel.text = PlayerInfo.Instance.inventory.VipMoney.ToString ();
	}


	void OnTacktick(GameObject sender)
	{
		UITacticMapPanel.Instance.Show ();
		UIArmoryPanel.Instance.Hide();
	}
	void OnCampaign(GameObject sender)
	{
		UIMissionDetails.Instance.Show(); 
		UIArmoryPanel.Instance.Hide();
		UIMissionDetails.Instance.buttonNext.SetActive (false);
		UIMissionDetails.Instance.buttonPlay.SetActive (true);
	}

	void OnInventory(GameObject sender)
	{
		UIMissionDetails.Instance.Hide(); 
		UITacticMapPanel.Instance.Hide ();
		UIArmoryPanel.Instance.Show();
	
	}

	void OnMoreCoinsBtnClick(GameObject sender){
		moreCoinsPopup.Show();
	}
	void OnMainMenuBtnClick(GameObject sender){
		UIMapPanel.Instance.Hide ();
		UIMissionDetails.Instance.Hide ();
		UITopPanel.Instance.Hide();
        UIGameSettingsPanel.Instance.Hide();
        UIGameList.Instance.Hide();
		UITitlePanel.Instance.Show();
       
	}
	void OnWorldMapBtnClick(GameObject sender){
		
		UIShipsTacticPanel.Instance.Hide();
		UIShipCustomizationPanel.Instance.Hide();
		UIArmoryPanel.Instance.Hide();
		UIMissionDetails.Instance.Hide();
		UIMapPanel.Instance.Show();

	}

	private bool IsCanBackButtonClick()
	{
		return !UIGameList.Instance.IsBlockAction;
	}

    void OnBackBtnClick(GameObject sender)
    {
	    if (!IsCanBackButtonClick())
		    return;

        foreach (var currentPanel in CurrentPanels)
        {
            currentPanel.Hide();
            //CurrentPanels.Remove(currentPanel);
        }
        CurrentPanels.Clear();

		if (PrevPanels.Count < 1)
			return;

        foreach (var prevPanel in PrevPanels[PrevPanels.Count()-1])
        {
            prevPanel.Show();
        }
		Debug.Log ("back btn click");
        //CurrentPanels = PrevPanels[PrevPanels.Count() - 1];
        PrevPanels.Remove(PrevPanels[PrevPanels.Count() - 1]);
        /* UIShipCustomizationPanel.Instance.Hide();
        UIArmoryPanel.Instance.Hide();
        UIShipsTacticPanel.Instance.Show();
        UIMissionDetails.Instance.Show();*/

    }
    public void HideAllLeftBtns()
    {
        mainMenuBtn.gameObject.SetActive(false);
        worldMapBtn.gameObject.SetActive(false);
        //backBtn.gameObject.SetActive(false);
        
    }

    public void OptionsState()
    {
        mainMenuBtn.gameObject.SetActive(true);
        worldMapBtn.gameObject.SetActive(false);
        //backBtn.gameObject.SetActive(false);
        leftPanelTitleLbl.text = "Options";
    }

    public void GameListState()
    {
        mainMenuBtn.gameObject.SetActive(false);
        worldMapBtn.gameObject.SetActive(false);
        backBtn.gameObject.SetActive(true);
        leftPanelTitleLbl.text = "Games list";
        CurrentPanels.Clear();
        CurrentPanels.Add(UIGameList.Instance);
        PrevPanels.Clear();
        //PrevPanels.Add(UITitlePanel.Instance);
    }

    public void MultiPlayerBattleSettingsState()
    {
		leftPanelTitleLbl.text =  LocalizationConfig.getText("Battle");
    }

    public void ChangeState(TopPanelState state)//, List<IMenuInterface> prevPanels=null)
    {
        HideAllLeftBtns();
        //PrevPanels.Clear();
        //CurrentPanels.Clear();
        switch (state)
        {
            case(TopPanelState.GameList):
            {
                mainMenuBtn.gameObject.SetActive(true);
                /*if (prevPanels != null)
                {
                    PrevPanels.Add(prevPanels);
                }*/
                //CurrentPanels.Add(UIGameList.Instance);
                leftPanelTitleLbl.text = "Games list";
                break;

            }
        }
    }

    public void AddPrevPanels(List<IMenuInterface> prevPanels)
    {
        PrevPanels.Add(prevPanels);
    }
    
	/*public void Hide(){
		GetComponent<UITweener>().Play(false);
	}*/
	
	public override void Show(){

        // base.Show();
		//gameObject.SetActive(true);
	    IsActive = true;
		GetComponent<UITweener>().enabled=true;
		GetComponent<UITweener>().Play(true);
        //UITopPanel.Instance.Hide();
	}

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.Top;
	}
}
