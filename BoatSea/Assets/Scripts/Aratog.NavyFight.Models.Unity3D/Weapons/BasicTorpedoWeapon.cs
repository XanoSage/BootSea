using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;

namespace Aratog.NavyFight.Models.Unity3D.Weapons {
	public class BasicTorpedoWeapon: Weapon {

		public BasicTorpedoWeapon () {
			Type = WeaponsType.BasicTorpedo;

			FireCooldownCount = 0f;

			PriceInCoin = 0;
			PriceInGears = 0;

			Damage = 1;

			Speed = 7.0f;
			FlightClassic = 5f;
			FlightNewVawe = 5f;
			FireCooldown = 1f;
			AfterFireCooldown = 0.25f;
		}
	}
}
