using Aratog.NavyFight.Models.Common;

namespace Aratog.NavyFight.Models.Maps
{
	public class CObstacleEvidence
	{
		// Положение препятствия на карте
		public CVector3 PositionOnMap;
		public CObstacle Obstacle;

		public CObstacleEvidence(CVector3 point, CObstacle obstacle)
		{
			PositionOnMap = point;
			Obstacle = obstacle;
		}
	}
}
