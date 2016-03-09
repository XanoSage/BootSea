using System;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Games;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Weapons {
	public abstract class Weapon : IPositionable
	{

		#region Variables

		/// <summary>
		/// Цена оружия в монетках
		/// </summary>
		public int PriceInCoin, PriceInGears;

		/// <summary>
		/// Тип оружия
		/// </summary>
		public WeaponsType Type;

		/// <summary>
		/// Время полета снаряда
		/// </summary>

		protected float FlightClassic;

		protected float FlightNewVawe;

		public float Flight
		{
			get
			{
				if (Owner != null)
				{
					if (Owner.CurrentBattle.Mechanics == MechanicsType.NewWave)
						return FlightNewVawe;
				}

				return FlightClassic;
			}
		}

		/// <summary>
		/// Скорость полета снаряда
		/// </summary>
		public float Speed;

		public float FireCooldown;

		public float FireCooldownCount;

		public int Damage;

		public float AfterFireCooldown;

		public float TimeToBlowUp;

		public float BlowUpTimer;

		public bool IsCanShoot;

		public Player Owner;

		public Vector3 Position { get; set; }

		#endregion

		#region Constructors

		protected Weapon()
		{
			FireCooldownCount = 0;
			Owner = null;
			IsCanShoot = true;
		}

		public static Weapon CreateWeapon(WeaponsType type)
		{
			Weapon weapon = null;

			switch (type)
			{
				case WeaponsType.BasicBomb:
					weapon = new BasicBombWeapon();
					break;
				case WeaponsType.BasicProjectile:
                    Debug.Log("create basic Weapon");
					weapon = new BasicProjectileWeapon();
					break;
				case WeaponsType.BasicTorpedo:
					weapon = new BasicTorpedoWeapon();
					break;
                case WeaponsType.DeepBomb:
                    weapon = new DeepBombWeapon();
                    break;
               default:
                    Debug.Log("create Advance Weapon");
                    weapon = new MissileWeapon();
                    break;

			}

			return weapon;
		}

		#endregion

		public const float epsilon = 0.01f;

		#region Events

		public virtual void OnFire()
		{
			if (IsCanShoot)
			{
				IsCanShoot = false;
				FireCooldownCount = 0;
			}
		}

		public virtual void Reload()
		{
			IsCanShoot = true;
			FireCooldownCount = 0;
		}

		public virtual void Update(float dt)
		{
			if (FireCooldownCount > 0f)
			{
				FireCooldownCount -= dt;
			}
		}

		#endregion
	}
}
