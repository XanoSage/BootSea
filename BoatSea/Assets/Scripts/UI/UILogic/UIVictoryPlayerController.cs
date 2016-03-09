using UnityEngine;
using System.Collections;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Ships;

public class UIVictoryPlayerController : MonoBehaviour {

	[SerializeField]
	private UISprite _ShipSprite;

	[SerializeField]
	private UILabel _flagLabel;
	[SerializeField]
	private UILabel _deathLabel;
	[SerializeField]
	private UILabel _killsLabel;
	[SerializeField]
	private UILabel _minesLabel;
	[SerializeField]
	private UILabel _advanceWeaponLabel;
	[SerializeField]
	private UILabel _basicWeaponLabel;


	// Use this for initialization
	void Start () {
	
	}

	public void Show(Player _player)
	{
		int kills = _player.Statistic_kills;
		int death = _player.Statistic_death;
		int flag = _player.Statistic_flag;
		int mines = _player.Statistic_mines;
		int shootAdvance = _player.Statistic_shootAdvance;
		int shoot = _player.Statistic_shoot;


		_flagLabel.text = flag.ToString ();
		_deathLabel.text = death.ToString ();
		_killsLabel.text = kills.ToString ();
		_minesLabel.text = mines.ToString ();
		_advanceWeaponLabel.text = shootAdvance.ToString ();
		_basicWeaponLabel.text = shoot.ToString ();

		_ShipSprite.spriteName = ShipName(_player.MyShip.Type);
	
	}

	private string ShipName(ShipType type)
	{
		string str = "";
		
		switch (type)
		{
		case ShipType.BigShip:
			str = "Linkor_Med_ico";
			break;
		case ShipType.Submarine:
			str = "SM_Med_ico" ;
			break;
		case ShipType.Boat:
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


}
