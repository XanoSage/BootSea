using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;

namespace Aratog.NavyFight.Models.Unity3D.Weapons {
	public class BasicBombWeapon: Weapon {

		public BasicBombWeapon () {
			Type = WeaponsType.BasicBomb;
			
			FireCooldownCount = 0;
			
			PriceInCoin = 0;
			PriceInGears = 0;

			Damage = 1;

			Speed = 0f;
			FlightNewVawe = 15f;
			FlightClassic = 15f;
			FireCooldown = 1.0f;
			AfterFireCooldown = 0.2f;

			IsCanShoot = true;

		}
	}
}
