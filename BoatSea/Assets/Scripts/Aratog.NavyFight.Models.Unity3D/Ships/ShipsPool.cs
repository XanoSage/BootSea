using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Ships;

namespace Aratog.NavyFight.Models.Unity3D.Ship {
	public class ShipsPool: IInitable {

		#region Variables

		public static bool Inited = false;

		public static ShipsPool Instance { get; set; }

		public static List<Ship> PoolOfShips { get; private set; }

		public Ship this[ShipType ship] {
			get { return PoolOfShips == null ? null : GetShip(ship); }
		}

		#endregion


		#region Events

		public void Init () {
			if (Inited)
				return;

			Inited = true;

			Ship.InitPoolOfShipType();

			Instance = this;

			CreateShips();
		}

		public static void Restart () {
			Inited = false;
			(new ShipsPool()).Init();
		}

		private void CreateShips () {
			PoolOfShips = new List<Ship>();

			foreach (ShipType poolOfShip in Ship.PoolOfShipType) {
				PoolOfShips.Add(Ship.CreateShip(poolOfShip));
			}

		}

		public static ShipType GetRandomShipType()
		{
			int i = (new Random()).Next(0, PoolOfShips.Count);
			return PoolOfShips[i].Type;
		}

		public Ship GetShip (ShipType type) {
			return PoolOfShips.FirstOrDefault(ship => ship.Type == type);
		}
		#endregion
	}
}
