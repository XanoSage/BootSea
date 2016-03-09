using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;

namespace Aratog.NavyFight.Models.Unity3D.Weapons {
	public class BasicProjectileWeapon:Weapon {

		public BasicProjectileWeapon () {
			Type = WeaponsType.BasicProjectile;

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
	}
}
