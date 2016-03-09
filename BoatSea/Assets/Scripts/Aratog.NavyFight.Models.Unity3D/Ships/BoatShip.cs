using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using Aratog.NavyFight.Models.Unity3D.Base;

namespace Aratog.NavyFight.Models.Unity3D.Ship {
	public class BoatShip: Ship {
		//private const int BasicHealthPoint = 1;

		//private const int BasicBombCount = 5;

		public BoatShip () {
			Type = ShipType.Boat;
			Speed = 0.07f;
            MaxSpeed = 2.7f;
			
			BasicHealthPoint = 10;

			MaxVelocity = 10f;

			Acceleration = 10.5f;
			AccelerationDown = -8.5f;
          

			BasicBombCount = 3;
			
			HealthPoint = BasicHealthPoint;
			BombCount = BasicBombCount;
			
			RotationSpeed = 8f; //6.5f;

			CameraFOVFrom = 54;
            CameraFOVTo = 75;

			//CameraFOVFrom = 48;
			//CameraFOVTo = 58;

			Size = 1.0f;

			BasicWeapon = Weapon.CreateWeapon(WeaponsType.BasicProjectile);
		}

		public override void OnMove()
		{
			throw new NotImplementedException();
		}

		public override void OnRespawn (bool invulnerability = false) {
			base.OnRespawn(invulnerability);
            Armor = BaseArmor;
			HealthPoint = BasicHealthPoint;
		}
	}
}
