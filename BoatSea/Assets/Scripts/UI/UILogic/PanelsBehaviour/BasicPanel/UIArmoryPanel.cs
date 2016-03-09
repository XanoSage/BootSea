using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArmoryPanel : BasicPanel<UIArmoryPanel> {
	public int i=0;
	public UIArmoryPanel(){i=5;}

	public List<UIShopShipItem> ShipItems;

	public GameObject ships;
	public GameObject weapons;
	public GameObject upgrades;


	public override void Show(){
		//if(IsActive)return;
        //print("ArmoryShow");
		base.Show();
		if(UITopPanel.Instance.rightPanelTitle.activeSelf==false)UITopPanel.Instance.rightPanelTitle.SetActive(true);
		if (UIShipsTacticPanel.Instance.isCampaign) {
			UITopPanel.Instance.rightButtonCampaign.SetActive (true);
			UITopPanel.Instance.rightButtonCustomBatle.SetActive (false);
		} else {
			UITopPanel.Instance.rightButtonCampaign.SetActive (false);
			UITopPanel.Instance.rightButtonCustomBatle.SetActive (true);
		}

		UITopPanel.Instance.rightPanelTitleLbl.text="Tactic map";
	
		if (upgrades.activeSelf) {
			CheckIcons();
		}

		/*// Some Old Code Can delete?
		foreach (var shipItem in ShipItems)
		{
			shipItem.UpdateColor();
		}
*/
	}


	public void CheckIcons()
	{
		if (upgrades.activeSelf) {
			Transform child = upgrades.transform.FindChild ("table");
			for (int i=1; i<10; i++) {
				child.FindChild ("ship_block" + i).GetComponent<UIUpgradesItem> ().CheckInventory ();
			}
		}

	}

	public override void Hide(){
		//if(!IsActive)return;
        //print("ArmoryHIde");
		base.Hide();
	//	UITopPanel.Instance.rightPanelTitle.SetActive(false);
	}

	public override void SetType()
	{
		MenuType = UIMenuInterfaceControllsType.Armory;
	}
}