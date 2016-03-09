using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Maps;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using Random = System.Random;

namespace Aratog.NavyFight.Models.Unity3D.Maps {
	[Serializable]
	public class Map {
		#region Variables

		// Имя карты
		public string Name = "New Map";

		// Максимальное кол-во игроков на карте
		public int MaxPlayers = 4;

		// Тип игрового режима
		public GameMode Type = GameMode.CaptureTheFlag;

		// Тип окружения
		public EnvironmentType Environment = EnvironmentType.Medieval;

		// Ширина и длина поля
		[HideInInspector] public int FieldHeight, FieldWidth;

		// Логический массив поля (это 2-мерная карта через одномерный массив; необходимо для сериализации)
		[HideInInspector] public Cell[] Cells;

		// Размер клеточки
		[HideInInspector] public float CellSize = 1;

		[HideInInspector] public Vector3[] BasicWayPoints;
		[HideInInspector] public Vector3[] AdvancedWayPoints;
		[HideInInspector] public bool[] AccessableWayPoints;

		public static Vector3 TopLeftBorder = Vector3.zero;
		public static Vector3 BottomRightBorder = Vector3.zero;

		// Словарь из реальных объектов на карте
		//public Dictionary<Vector3, Obstacle> Obstacles;

		public static List<Point> AvailableNavigatePoint;

		// Физический размер карты
		[HideInInspector] public Vector2 Size;

		#endregion


		#region Function and events

		// Способ получения клеточки из одномерного массива, как если бы это был двухмерный
		public Cell this [int x, int y] {
			get {
				if (Cells.Length < y * FieldWidth + x)
					return null;
				return Cells[y * FieldWidth + x];
			}
			set {
				try {
					Cells[y * FieldWidth + x] = value;
				}
				catch (ArgumentOutOfRangeException e) {
					Debug.LogError(e);
				}
			}
		}

		public bool IsPointOutOfTheBorder (Point point) {
			return (point.Y < 0 || point.Y >= FieldHeight || point.X < 0 || point.X >= FieldWidth);
		}

		public void InitWaypoints()
		{
			BasicWayPoints = new Vector3[Cells.Length];
			AdvancedWayPoints = new Vector3[Cells.Length];
			for (int i = 0; i < Cells.Length; i++)
			{
				int x = i/FieldWidth;
				int y = i%FieldWidth;
				BasicWayPoints[i] = GetWorldPosition(this, new Point(x, y));

				Vector3 advancedWaPoint = new Vector3(BasicWayPoints[i].x - CellSize * 0.5f, BasicWayPoints[i].y, BasicWayPoints[i].z - CellSize * 0.5f);
				AdvancedWayPoints[i] = advancedWaPoint;
			}

	//		Debug.Log("InitWaypoints: init complete");
		}

		public void InitAccessWayPoints()
		{
			AccessableWayPoints = new bool[Cells.Length];

			for (int i = 0; i < Cells.Length; i++)
			{
				AccessableWayPoints[i] = true;
			}

			for (int i = 0; i < Cells.Length; i++)
			{
				int x = i/FieldWidth;
				int y = i%FieldWidth;
				//AccessableWayPoints[i] = true;
				//AccessableWayPoints[i] = IsPointAccessable(new Point(x, y));
				Point point = new Point(x, y);

				//Debugger.Debugger.Log(string.Format("Сurrent point: {0}", point));
				
				SetPointAccessable(point);
			}

		//	Debug.Log("InitAccessWayPoints: init complete");
		}

		private void SetAccessableWayPoint(Point point, bool isAccess)
		{
			int i = point.X*FieldWidth + point.Y;
			if (i >=0 && i < Cells.Length)
						AccessableWayPoints[i] = isAccess;
		}

		private void SetNeigboursAccessableWay(Point point, bool isAccess)
		{
			if (point.X == 0 || point.X == FieldWidth - 1 || point.Y == 0 || point.Y == FieldHeight - 1)
			{
				//AccessableWayPoints[i] = false;
				//Debugger.Debugger.Log(string.Format("Border point: {0}", point));
				return;
			}

			int i = point.X*FieldWidth + point.Y;

			if (i >= 0 && i < Cells.Length)
				AccessableWayPoints[i] = isAccess;

			i = (point.X + 1)*FieldWidth + point.Y;

			if (i >= 0 && i < Cells.Length)
				AccessableWayPoints[i] = isAccess;

			i = (point.X + 1)*FieldWidth + point.Y + 1;

			if (i >= 0 && i < Cells.Length)
				AccessableWayPoints[i] = isAccess;

			i = point.X*FieldWidth + point.Y + 1;

			if (i >= 0 && i < Cells.Length)
				AccessableWayPoints[i] = isAccess;

		}

		private void SetPointAccessable(Point point)
		{
			if (this[point.X, point.Y] != null)
			{
				int i = point.X*FieldWidth + point.Y;

				if (point.X == 0 || point.X == FieldWidth - 1 || point.Y == 0 || point.Y == FieldHeight - 1)
				{
					AccessableWayPoints[i] = false;
					//Debugger.Debugger.Log(string.Format("Border point: {0}", point));
					return;
				}

				if (!IsPointAvailableToNavigate(this[point.X, point.Y].Type))
				{
					SetNeigboursAccessableWay(point, false);
				}
			}
		}

		private bool CheckPointToAccess(Point point)
		{
			return !IsPointOutOfTheBorder(point) && this[point.X, point.Y] != null &&
			       IsPointAvailableToNavigate(this[point.X, point.Y].Type);
		}

		#endregion


		#region Loading/Saving

		public bool Save (string path) {
			bool success = true;
			var serializer = new XmlSerializer(typeof (Map));
			try {
				using (var stream = new FileStream(path, FileMode.Create))
					serializer.Serialize(stream, this);
			}
			catch {
				success = false;
			}
			return success;
		}

		public static Map Load(string path)
		{
			var serializer = new XmlSerializer(typeof (Map));
			using (var stream = new FileStream(path, FileMode.Open))
				return serializer.Deserialize(stream) as Map;
		}

		// Загружает XML прямо из текста. Полезно в комбинации с WWW.text.
		public static Map LoadFromText (string text) {
			var serializer = new XmlSerializer(typeof (Map));
			return serializer.Deserialize(new StringReader(text)) as Map;
		}

		#endregion

		#region Useful map functions
		/// <summary>
		/// По мировым координатам в игре получить координату карты (ячейки)
		/// </summary>
		/// <param name="map">Карта в которой искать </param>
		/// <param name="fromPosition"></param>
		/// <returns></returns>
		public static Point GetMapPosition (Map map, Vector3 fromPosition) {
			Point point = new Point();

			point.X = Mathf.RoundToInt((fromPosition.x - TopLeftBorder.x) / map.CellSize);
			point.Y = Mathf.RoundToInt((fromPosition.z - TopLeftBorder.z) / map.CellSize);

			//Debug.Log(string.Format("fromPosition:{0}, pointPosition:{1}", fromPosition, point));

			return point;
		}

		/// <summary>
		/// По координатам ячейки, получить мировую координату в игре
		/// </summary>
		/// <param name="map"></param>
		/// <param name="point"></param>
		/// <returns></returns>
		public static Vector3 GetWorldPosition (Map map, Point point) {
			Vector3 pos = Vector3.zero;

			pos.x = point.X * map.CellSize + TopLeftBorder.x;
			pos.z = point.Y * map.CellSize + TopLeftBorder.z;

			return pos;
		}

		public static Vector3 GetWorldPosition(Map map, Point point, Ship.Ship ship)
		{
			Vector3 pos = Vector3.zero;

			int index = point.X*map.FieldWidth + point.Y;

			if (index >= map.AdvancedWayPoints.Length)
				return pos;

			switch (ship.Type)
			{
				case ShipType.BigShip:
					pos = map.AdvancedWayPoints[index];
					break;

				default:
					pos = map.BasicWayPoints[index];
					break;
			}

			return pos;
		}

		public static Vector3 GetRandomPositionOnMap(Map map)
		{
			Vector3 pos = Vector3.zero;

			Random random = new Random();

			Point point = AvailableNavigatePoint[random.Next(0, AvailableNavigatePoint.Count - 1)];

			CellType type = map[point.X, point.Y].Type;

			if (!IsPointAvailableToNavigate(type))
			{
				Debug.LogError(String.Format("GetRandomPositionOnMap: This point ({0}) is not suitable to navigate, type:{1}", point,
				                             type));
			}

			pos = GetWorldPosition(map, point);
			return pos;
		}

		public static Point GetNearestPoint (Map currentMap, Vector3 currentPosition, Vector3 direction) {
			Point nearestPoint;
			Point currentPoint = GetMapPosition(currentMap, currentPosition);
			nearestPoint = new Point(currentPoint.X + Mathf.CeilToInt(direction.x), currentPoint.Y + Mathf.CeilToInt(direction.z));

			if (nearestPoint.X < 0)
				nearestPoint.X = 0;
			if (nearestPoint.X > currentMap.FieldWidth -1)
				nearestPoint.X = currentMap.FieldWidth -1;

			if (nearestPoint.Y < 0)
				nearestPoint.Y = 0;
			if (nearestPoint.Y > currentMap.FieldHeight -1)
				nearestPoint.Y = currentMap.FieldHeight -1;

			return nearestPoint;
		}

		private const float EpsMagnitude = 0.06125f;

		public static Vector3 GetNearestPosition(Map currentMap, Vector3 forPoint, Vector3 currentPos, Ship.Ship ship)
		{
			Vector3 pos = Vector3.zero;

			if (forPoint == currentPos)
			{
				return forPoint;
			}

			float minDistance = float.MaxValue;

			int index = -1;

			Debug.Log(string.Format("GetNearestPosition: currentPosition: {0}, positionTo: {1}, ship: {2}", forPoint,
			                             currentPos, ship.Type));

			if (ship.Type == ShipType.BigShip)
			{
				for (int i = 0; i < currentMap.AdvancedWayPoints.Length; i++)
				{
					//if (currentPosition == currentMap.AdvancedWayPoints[i])
					//	continue;

					float currentDistance = (currentMap.AdvancedWayPoints[i] - forPoint).sqrMagnitude;
					if (currentDistance < minDistance)
					{
						minDistance = currentDistance;
						index = i;
					}
				}
			}
			else
			{
				for (int i = 0; i < currentMap.BasicWayPoints.Length; i++)
				{
					//if (currentPosition == currentMap.BasicWayPoints[i])
					//	continue;

					float currentDistance = (currentMap.BasicWayPoints[i] - forPoint).sqrMagnitude;
					if (currentDistance < minDistance)
					{
						minDistance = currentDistance;
						index = i;
					}
				}
			}

			if (currentPos != default(Vector3) && (currentPos - currentMap.BasicWayPoints[index]).sqrMagnitude <= EpsMagnitude)
				return currentPos;

			if (index > -1)
				pos = currentMap.BasicWayPoints[index];

			return pos;
		}

		private const int AttemptCountMax = 20;

		public static Point GetRandomPointOnRadius (Map currentMap, Point center, Player player, int radius = 5) {
			Point randomPoint = null;

			Point minPoint = new Point(center.X - radius, center.Y - radius);

			if (minPoint.X < 0)
				minPoint.X = 0;

			if (minPoint.Y < 0)
				minPoint.Y = 0;

			Point maxPoint = new Point(center.X + radius, center.Y + radius);

			if (maxPoint.X >= currentMap.FieldHeight)
				maxPoint.X = currentMap.FieldHeight - 1;

			if (maxPoint.Y >= currentMap.FieldWidth)
				maxPoint.Y = currentMap.FieldWidth - 1;

			Random random = new Random();

			int attemptCount = 0;

			while (true)
			{
				randomPoint = new Point(random.Next(minPoint.X, maxPoint.X), random.Next(minPoint.Y, maxPoint.Y));
				CellType type = currentMap[randomPoint.X, randomPoint.Y].Type;

				attemptCount++;

				if (attemptCount >= AttemptCountMax)
				{
					attemptCount = 0;
					return GetRandomPointOnRadius(currentMap, center, player, radius + 1);
				}

				if (!IsPointAvailableToNavigate(type))
				{
					//Debug.LogError(string.Format("GetRandomPositionOnMap: This point ({0}) is not suitable to navigate, type:{1}",
					//							 randomPoint,
					//							 type));
					continue;
				}


				// Check if way accessable way for big ship
				if (player != null && player.MyShip.Type == ShipType.BigShip)
				{
					int ind = randomPoint.X*currentMap.FieldWidth + randomPoint.Y;
					if (!currentMap.AccessableWayPoints[ind])
						continue;
				}

				break;
			}

			//randomPoint = new Point(random.Next(minPoint.X, maxPoint.X), random.Next(minPoint.Y, maxPoint.Y));

			//	CellType type = currentMap[randomPoint.X, randomPoint.Y].Type;
			return randomPoint;
		}

		public static Vector3 GetRandomPointOnRadius(Map currentMap, Vector3 center, Player player, int radius = 5)
		{
			Point centerPoint = GetMapPosition(currentMap, center);

			return  GetWorldPosition(currentMap, GetRandomPointOnRadius(currentMap, centerPoint, player, radius));
		}

		public static void CorrectPoint(Map currentMap, ref Point point, Player player, bool isNavigate = false)
		{
			if (point.X < 0)
				point.X = 0;
			if (point.X > currentMap.FieldWidth -1)
				point.X = currentMap.FieldWidth -1;

			if (point.Y < 0)
				point.Y = 0;
			if (point.Y > currentMap.FieldHeight -1)
				point.Y = currentMap.FieldHeight -1;

			if (player != null && player.MyShip.Type == ShipType.BigShip && isNavigate)
			{
				int ind = point.X*currentMap.FieldWidth + point.Y;
				if (!currentMap.AccessableWayPoints[ind])
				{
					if (point.X + 1 < currentMap.FieldWidth - 1)
					{
						ind = (point.X + 1)*currentMap.FieldWidth + point.Y;
						if (!currentMap.AccessableWayPoints[ind])
						{
							point.X += 1;
							CorrectPoint(currentMap, ref point, player);
						}
					}

					if (point.Y + 1 < currentMap.FieldHeight - 1)
					{
						ind = point.X*currentMap.FieldWidth + point.Y + 1;

						if (!currentMap.AccessableWayPoints[ind])
						{
							point.Y += 1;
							CorrectPoint(currentMap, ref point, player);
						}
					}

					
					if (point.X - 1 < currentMap.FieldWidth - 1)
					{
						ind = (point.X - 1)*currentMap.FieldWidth + point.Y;
						if (!currentMap.AccessableWayPoints[ind])
						{
							point.X += 1;
							CorrectPoint(currentMap, ref point, player);
						}
					}

					if (point.Y - 1 < currentMap.FieldHeight - 1)
					{
						ind = point.X*currentMap.FieldWidth + point.Y + 1;

						if (!currentMap.AccessableWayPoints[ind])
						{
							point.Y += 1;
							CorrectPoint(currentMap, ref point, player);
						}
					}
				}
			}
		}

		public static List<Point> GetEdgePointOnRadius(Map currentMap, Point center, int radius, Player player = null)
		{
			List<Point> extremePoints = new List<Point>();
			
			//Get top-left point
			Point point = new Point(center.X - radius, center.Y - radius);
			
			CorrectPoint(currentMap, ref point, player, true);
			extremePoints.Add(point);

			//Get top-right 
			point = new Point(center.X + radius, center.Y - radius);

			CorrectPoint(currentMap, ref point, player, true);
			extremePoints.Add(point);

			//Get bottom-right point
			point = new Point(center.X + radius, center.Y + radius);

			CorrectPoint(currentMap, ref point, player, true);
			extremePoints.Add(point);
			
			//Get bottom-left point
			point = new Point(center.X - radius, center.Y + radius);

			CorrectPoint(currentMap, ref point, player, true);
			extremePoints.Add(point);

			return extremePoints;
		}

		public static List<Vector3> GetEdgePointOnRadius(Map currentMap, Vector3 center, int radius)
		{
			List<Vector3> positions = new List<Vector3>();

			Point centerPoint = GetMapPosition(currentMap, center);

			List<Point> points = GetEdgePointOnRadius(currentMap, centerPoint, radius);

			foreach (Point point in points)
			{
				Vector3 pos = GetWorldPosition(currentMap, point);
				positions.Add(pos);
			}

			return positions;
		}

		private static List<Point> GetAvailableNeighbourPoints(Map currentMap, Vector3 pos)
		{
			List<Point> availablePoints = new List<Point>();

			Point currentPoint = GetMapPosition(currentMap, pos);

			Point neighbiure = new Point(currentPoint.X - 1, currentPoint.Y);

			CellType type = currentMap[neighbiure.X, neighbiure.Y].Type;

			if (IsPointAvailableToNavigate(type))
				availablePoints.Add(neighbiure);

			neighbiure = new Point(currentPoint.X + 1, currentPoint.Y);

			type = currentMap[neighbiure.X, neighbiure.Y].Type;

			if (IsPointAvailableToNavigate(type))
				availablePoints.Add(neighbiure);

			neighbiure = new Point(currentPoint.X, currentPoint.Y - 1);

			type = currentMap[neighbiure.X, neighbiure.Y].Type;

			if (IsPointAvailableToNavigate(type))
				availablePoints.Add(neighbiure);

			neighbiure = new Point(currentPoint.X, currentPoint.Y + 1);

			type = currentMap[neighbiure.X, neighbiure.Y].Type;

			if (IsPointAvailableToNavigate(type))
				availablePoints.Add(neighbiure);

			return availablePoints;
		}

		public static Point GetRandomAvailableNeighbourPoints(Map currentMap, Vector3 pos)
		{
			List<Point> points = GetAvailableNeighbourPoints(currentMap, pos);
			Point point = null;

			Random random = new Random();

			point = points[random.Next(0, points.Count - 1)];

			return point;
		}

		public static void OnDestruction(Map currentMap, Vector3 destructionPosition)
		{
			//Debug.Log("Map.OnDestruction: set on the map the none cell of type pos: " + destructionPosition);
			Point point = GetMapPosition(currentMap, destructionPosition);

			//Debug.Log(string.Format("Map.OnDestruction: was currentMap[{0}, {1}].Type = {2}", point.X, point.Y, currentMap[point.X, point.Y].Type.ToString()));

			currentMap[point.X, point.Y].Type = CellType.None;

			//currentMap.AccessableWayPoints[point.X*currentMap.FieldWidth + point.Y] = true;

			currentMap.CorrectAccessableWayPointsOnDestruction(point);

			//Debug.Log(string.Format("Map.OnDestruction: currentMap[{0}, {1}].Type = {2}", point.X, point.Y, currentMap[point.X, point.Y].Type.ToString()));
		}

		private void CorrectAccessableWayPointsOnDestruction(Point point)
		{
			SetNeigboursAccessableWay(point, true);

			SetPointAccessable(point);

			Point newPoint = new Point(point.X +1 , point.Y);

			SetPointAccessable(newPoint);

			newPoint = new Point(point.X, point.Y + 1);

			SetPointAccessable(newPoint);

			newPoint = new Point(point.X - 1, point.Y);

			SetPointAccessable(newPoint);

			newPoint = new Point(point.X, point.Y - 1);

			SetPointAccessable(newPoint);

		}

		public static bool IsPointAvailableToNavigate(CellType type)
		{
			return (type != CellType.Static && type != CellType.StaticGround && type != CellType.Destructable &&
			        type != CellType.Destructable && type != CellType.FlagPoint);
		}

		public static bool IsPointAccessableToNavigate(Map currentMap, Point point, Player player)
		{
			if (player == null)
				return true;

			//Debugger.Debugger.Log(string.Format("IsPointAccessableToNavigate - Player: {0}, ship Type: {1}", player.Id, player.MyShip.Type));

			if (player.MyShip.Type != ShipType.BigShip)
				return true;

			int i = point.X*currentMap.FieldWidth + point.Y;

			//Debugger.Debugger.Log(string.Format("IsPointAccessableToNavigate - Player: {0}, point: {1}, is access: {2}", player.Id, point, currentMap.AccessableWayPoints[i]));
			return currentMap.AccessableWayPoints[i];
		}
	}

	#endregion
}
