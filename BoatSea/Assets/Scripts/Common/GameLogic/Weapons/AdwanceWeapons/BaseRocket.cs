using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Weapons;


public class BaseRocket : Weapon {


	public BaseRocket(){
		Type = WeaponsType.Missile;

		FireCooldownCount = 0;
		
		PriceInCoin = 0;
		PriceInGears = 0;
		
		Damage = 1;
		
		Speed = 9.0f;
		FlightClassic = 5f;
		FlightNewVawe = 5f;
		FireCooldown = 1f;
		AfterFireCooldown = 0.2f;


	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
