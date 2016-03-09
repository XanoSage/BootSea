using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Maps;

namespace Aratog.NavyFight.Models.Unity3D.Players {
	public class PathNode {

		// Координаты точки на карте.
		public Point Position { get; set; }

		// Длина пути от старта (G).
		public int PathLengthFromStart { get; set; }

		// Точка, из которой пришли в эту точку.
		public PathNode CameFrom { get; set; }

		// Примерное расстояние до цели (H).
		public int HeuristicEstimatePathLength { get; set; }

		// Ожидаемое полное расстояние до цели (F).
		public int EstimateFullPathLength {
			get { return this.PathLengthFromStart + this.HeuristicEstimatePathLength; }
		}

	}
}
