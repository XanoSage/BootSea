using System;
using LinqTools;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Maps;

namespace Aratog.NavyFight.Models.Unity3D.Players
{
//	public static class Pathfinding
//	{

//		public static int iteratorCount = 0;

//		public static List<Point> FindPath(Map map, Point start, Point goal, Player player = null)
//		{
//			// Шаг 1.
//			iteratorCount = 0;
//			var closedSet = new Collection<PathNode>();
//			var openSet = new Collection<PathNode>();
//			// Шаг 2.
//			PathNode startNode = new PathNode()
//				{
//					Position = start,
//					CameFrom = null,
//					PathLengthFromStart = 0,
//					HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
//				};
//			openSet.Add(startNode);
//			while (openSet.Count > 0)
//			{
//				// Шаг 3.
//				var currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();
//				// Шаг 4.
//				if (currentNode.Position == goal)
//					return GetPathForNode(currentNode);
//				// Шаг 5.
//				openSet.Remove(currentNode);
//				closedSet.Add(currentNode);

//				Collection<PathNode> neighboursNode = GetNeighbours(currentNode, goal, map, player);
//				// Шаг 6.
//				for (int i = 0; i < neighboursNode.Count; i++)
//				{
//					iteratorCount ++;

//					var neighbourNode = neighboursNode[i];
//// Шаг 7.
//					if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
//					{
//						continue;
//					}
//					PathNode openNode = null; // = openSet.FirstOrDefault(node => node.Position == neighbourNode.Position);

//					for (int j = 0; j < openSet.Count; j++)
//					{
//						iteratorCount ++;
//						if (openSet[j].Position == neighbourNode.Position)
//						{
//							openNode = openSet[j];
//							break;
//						}
//						//openNode = null;
//					}
//					// Шаг 8.
//					if (openNode == null)
//					{
//						openSet.Add(neighbourNode);
//					}
//					else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
//					{
//						// Шаг 9.
//						openNode.CameFrom = currentNode;
//						openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
//					}
//				}
//			}
//			// Шаг 10.
//			return null;
//		}

//		public static Collection<PathNode> GetNeighbours(PathNode pathNode, Point goal, Map map, Player player,
//														 bool isClassicMode = true)
//		{
//			var result = new Collection<PathNode>();

//			int neighbourCount = isClassicMode ? 4 : 8;

//			// Соседними точками являются соседние по стороне клетки.
//			Point[] neighbourPoints = new Point[neighbourCount];
//			neighbourPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
//			neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
//			neighbourPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
//			neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);

//			if (!isClassicMode)
//			{
//				neighbourPoints[4] = new Point(pathNode.Position.X + 1, pathNode.Position.Y + 1);
//				neighbourPoints[5] = new Point(pathNode.Position.X - 1, pathNode.Position.Y + 1);
//				neighbourPoints[6] = new Point(pathNode.Position.X + 1, pathNode.Position.Y - 1);
//				neighbourPoints[7] = new Point(pathNode.Position.X - 1, pathNode.Position.Y - 1);
//			}

//			for (int i = 0; i < neighbourPoints.Length; i++)
//			{
//				var point = neighbourPoints[i];
//				iteratorCount ++;
//				// Проверяем, что не вышли за границы карты.
//				if (map.IsPointOutOfTheBorder(point))
//					continue;


//				// Проверяем, что по клетке можно ходить.
//				//if (map[point.X, point.Y].Type == CellType.Static ||
//				//	map[point.X, point.Y].Type == CellType.StaticGround ||
//				//	map[point.X, point.Y].Type == CellType.Destructable ||
//				//	map[point.X, point.Y].Type == CellType.Destructable)
//				if (!Map.IsPointAvailableToNavigate(map[point.X, point.Y].Type) ||
//					!Map.IsPointAccessableToNavigate(map, point, player))
//					continue;


//				// Заполняем данные для точки маршрута.
//				var neighbourNode = new PathNode()
//					{
//						Position = point,
//						CameFrom = pathNode,
//						PathLengthFromStart = pathNode.PathLengthFromStart +
//											  GetDistanceBetweenNeighbours(),
//						HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
//					};
//				result.Add(neighbourNode);
//			}
//			return result;
//		}

//		public static List<Point> GetPathForNode(PathNode pathNode)
//		{
//			var result = new List<Point>();
//			var currentNode = pathNode;
//			while (currentNode != null)
//			{
//				iteratorCount ++;
//				result.Add(currentNode.Position);
//				currentNode = currentNode.CameFrom;
//			}
//			result.Reverse();
//			return result;
//		}

//		public static int GetDistanceBetweenNeighbours()
//		{
//			return 1;
//		}

//		public static int GetHeuristicPathLength(Point from, Point to)
//		{
//			return Abs(from.X - to.X) + Abs(from.Y - to.Y);
//		}

//		public static int Abs(int num)
//		{
//			if (num < 0)
//				num = - num;
//			return num;
//		}
//	}

}
