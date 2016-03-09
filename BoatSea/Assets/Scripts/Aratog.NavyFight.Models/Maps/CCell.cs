namespace Aratog.NavyFight.Models.Maps
{
	public class CCell
	{
		// Тип клетки
		public CellType Type;

		// Указатель на реальный Obstacle в словаре из реальных объектов на карте
		public CObstacleEvidence Evidence;
	}
}
