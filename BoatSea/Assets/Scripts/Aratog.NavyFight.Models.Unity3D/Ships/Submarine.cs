using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Weapons;

namespace Aratog.NavyFight.Models.Unity3D.Ship {
	public class Submarine : Ship {

		//private const int BasicHealthPoint = 2;

		//private const int BasicBombCount = 5;

		public enum MotionState
		{
			Stop,
			EmmersionAndMove,
			Move,
			AfterMoveImmersion
		}

		public enum DivingDirection
		{
			Down = -1,
			Up = 0,
		}

		public MotionState StateOfMotion;

		public Submarine () {
			Type = ShipType.Submarine;
			Speed = 0.07f;
		    MaxSpeed = 2.1f;
			
			BasicHealthPoint = 2;
			BasicBombCount = 5;
			
			HealthPoint = BasicHealthPoint;
			BombCount = BasicBombCount;

			RotationSpeed = 4.5f; //5;
			
            CameraFOVFrom = 54;
            CameraFOVTo = 75;


			Acceleration = 10.5f;
			AccelerationDown = -8.5f;

			MaxVelocity = 10f;


			//CameraFOVFrom = 44;
			//CameraFOVTo = 52;

			Size = 1.25f;

			BasicWeapon = Weapon.CreateWeapon(WeaponsType.BasicTorpedo);

			StateOfMotion = MotionState.AfterMoveImmersion;
		}

		public override void OnMove () {
			throw new NotImplementedException();
		}

		public override void OnRespawn (bool invulnerability = false) {
			base.OnRespawn(invulnerability);
            Armor = BaseArmor;
			HealthPoint = BasicHealthPoint;
		}
	}
}
