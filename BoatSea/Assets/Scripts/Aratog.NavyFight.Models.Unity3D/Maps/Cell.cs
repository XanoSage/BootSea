using Aratog.NavyFight.Models.Maps;

namespace Aratog.NavyFight.Models.Unity3D.Maps
{
	[System.Serializable]
	public class Cell
	{
		// Тип клетки
		public CellType Type = CellType.None;
		
		// Цвет клетки
		public CellColor Color;
		
		// Указатель на реальный Obstacle в словаре из реальных объектов на карте
		public ObstacleEvidence Evidence;

		public Cell(CellType type, ObstacleEvidence evidence)
		{
			this.Type = type;
			this.Evidence = evidence;
		}
		
		public Cell()
		{
			this.Type = CellType.None;
		}
	}
}