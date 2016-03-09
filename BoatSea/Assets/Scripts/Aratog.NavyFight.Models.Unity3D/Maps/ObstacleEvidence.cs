using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Maps
{
	[System.Serializable]
	public class ObstacleEvidence
	{
		// Положение препятствия на карте
		public Vector3 PositionOnMap;
		
		// Собственно, само препятствие
		public Obstacle Obstacle;

		// Родитель препятствия
		public ObstacleEvidence Parent;

		/// <summary>
	        /// Часть от целого. Если obstacle width ==1 , height == 1, то parent = this;
	        /// part.X = 1; part.Y = 1;
		/// Для остальных просчитывается зарание
		/// </summary>
		public Point Part;

		public ObstacleEvidence(Vector3 point, Obstacle obstacle)
		{
			PositionOnMap = point;
			Obstacle = obstacle;
			Parent = null;
		}
		
		public ObstacleEvidence()
		{
			
		}
	}
}
