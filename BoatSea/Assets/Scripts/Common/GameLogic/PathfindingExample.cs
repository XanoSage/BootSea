#pragma warning disable

using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LinqTools;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Maps;

public class PathfindingExample : MonoBehaviour
{
	[SerializeField]
	Point _startPoint, _endPoint;
	
	MapInfo _editor;
	
	void Start()
	{
		_editor = GetComponent<MapInfo>();
	}

	void OnDrawGizmosSelected()
	{
		if (!_editor)
			_editor = GetComponent<MapInfo>();

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(_editor.GetTilePos(_startPoint.X, _startPoint.Y), 0.7f);

		Gizmos.color = Color.green;
		Gizmos.DrawSphere(_editor.GetTilePos(_endPoint.X, _endPoint.Y), 0.7f);

		Gizmos.color = Color.yellow;
		List<Point> points =new List<Point>();// Pathfinding.FindPath(_editor.Map, _startPoint, _endPoint);
		if (points != null) {
			foreach(Point p in points) {
				Gizmos.DrawSphere(_editor.GetTilePos(p.X, p.Y), 0.15f);
			}
		} else {
			Debug.LogError(points);
		}
	}
}