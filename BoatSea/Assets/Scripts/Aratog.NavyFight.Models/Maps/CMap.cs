using System.Collections.Generic;
using Aratog.NavyFight.Models.Common;
using Aratog.NavyFight.Models.Games;

namespace Aratog.NavyFight.Models.Maps
{
	public class CMap
	{
		// Имя карты
		public string Name;

		public int MaxPlayers;

		public GameMode Type;

		// Тип окружения
		public EnvironmentType Environment;

		// Ширина и длина поля
		public int FieldHeight;
		public int FieldWidth;

		// Логический массив поля
		public CCell[,] Cells;

		// Словарь из реальных объектов на карте
		public Dictionary<CVector3, CObstacle> Obstacles;

		// Физический размер карты
		public CRect Size;
	}
}
