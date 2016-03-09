using Aratog.NavyFight.Models.Maps;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Maps
{
	[System.Serializable]
	public class Obstacle
	{
		// Тип препятствия
		public ObstacleType Type;

		// Кол-во занемаемых им клеток
		public int Width, Height;

		// Физический размер (BoundingBox)
		public Rect Size;
	};
}