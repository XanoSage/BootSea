using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Flags;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using Assets.Scripts.Common.GameLogic.Multiplayer;
using UnityEngine;
using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Ship;
using Aratog.NavyFight.Models.Maps;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Extensions;

public class BaseBehaviour : MonoBehaviour {
	public int health;
	public TeamColor color;
	// Use this for initialization
	void Start () {
		Debug.Log ("Need Set Health");
		health = PlayerInfo.Instance.BasicBaseHealth;
	}


	public void AddDamage(int damage){
		health-= damage;
		if (health < 1) {
			if(color == TeamColor.BlueTeam)
			{
				BattleController.Instance.OnEndBattle(TeamColor.OrangeTeam);
			}
			else {
				BattleController.Instance.OnEndBattle(TeamColor.BlueTeam);
			}
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		ShipBehaviour shipBehaviour = other.gameObject.GetComponent<ShipBehaviour>();
	}
	
	private void OnCollisionStay(Collision other)
	{

	}
	
	private void OnCollisionExit(Collision other)
	{

	}
	
	private void OnTriggerStay(Collider other)
	{

	}

	// Update is called once per frame
	void Update () {
	
		HUDButtons.Instance.SetTeamScoreLabel (health,color);

	}
}
