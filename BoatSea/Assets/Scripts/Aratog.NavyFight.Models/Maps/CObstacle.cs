using Aratog.NavyFight.Models.Common;

namespace Aratog.NavyFight.Models.Maps
{
	public class CObstacle
	{
		// Тип препятствия
		public ObstacleType Type;

		// Кол-во занемаемых им клеток
		public int Width, Height;

		// Физический размер (BoundingBox)
		public CRect Size;
	};
}
