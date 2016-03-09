using System.Collections.Generic;
using Aratog.NavyFight.Models.Maps;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;

public class MapGridView : MonoBehaviour {

	#region Variables

	public float width = 5f;
    public float height = 5f;

	private Map currentPlayingMap;

	private List<Player> players;

	public bool IsNeedDrawAiPath = false;

	public bool IsShowWayPoints = false;

	#endregion

	void Awake () {
		currentPlayingMap = null;
	}

	// Use this for initialization
	void Start ()
	{
		if (GameController.Instance != null) GameController.Instance.OnStartBattle += OnStartBattle;
		else if (AITestController.Instance != null) AITestController.Instance.OnStartBattle += OnStartBattle;
	}

	// Update is called once per frame
	void Update () {
	
	}

	private void OnDrawGizmos () {

		if (currentPlayingMap == null)
			return;

		Gizmos.color = Color.white;

		Vector3 pos = Camera.current.transform.position;


		for (float z = Map.TopLeftBorder.z * 0.5f; z <= Map.BottomRightBorder.z *0.5f; z ++) {
			Gizmos.DrawLine(new Vector3(- (currentPlayingMap.FieldHeight * 0.5f) * height - height * 0.5f, 0.0f, z * height - height * 0.5f),
				new Vector3((currentPlayingMap.FieldHeight * 0.5f) * height - height * 0.5f, 0.0f, z * height - height * 0.5f));
		}

		for (float x = Map.TopLeftBorder.x * 0.5f; x <= Map.BottomRightBorder.x *0.5f; x ++) {
			Gizmos.DrawLine(new Vector3(x * width - width * 0.5f, 0.0f, -(currentPlayingMap.FieldWidth * 0.5f) * width - width * 0.5f),
				new Vector3(x * width - width * 0.5f, 0.0f, (currentPlayingMap.FieldWidth * 0.5f) * width - width * 0.5f));
		}

		for (int i = 0; i != currentPlayingMap.Cells.Length; i++) {
			if (currentPlayingMap.Cells[i].Type == CellType.None)
				continue;

			Gizmos.color = GetColorByCellType(currentPlayingMap.Cells[i].Type);

			pos = new Vector3(-(currentPlayingMap.FieldWidth * 0.5f) * width - width * 0.5f, 0, - (currentPlayingMap.FieldHeight * 0.5f) * height - height * 0.5f);

			int k = i / currentPlayingMap.FieldWidth;
			int j = i % currentPlayingMap.FieldWidth;

			Gizmos.DrawCube(
				pos + new Vector3(width * 0.5f, 0, height * 0.5f) +
					new Vector3(j * width, 0, k * height),
				new Vector3(width, 0.01f, height));
		}

		if (IsShowWayPoints)
		{

			for (int i = 0; i < currentPlayingMap.BasicWayPoints.Length; i++)
			{
				Gizmos.color = currentPlayingMap.AccessableWayPoints[i] ? Color.green : Color.red;
				Gizmos.DrawWireSphere(currentPlayingMap.AdvancedWayPoints[i], 0.5f);
			}
		}

		if (players == null || !IsNeedDrawAiPath)
			return;

		foreach (Player player in players) {
			AIPlayer aiPlayer = player as AIPlayer;

			if (aiPlayer == null)
				continue;

			if (aiPlayer.TargetPath == null)
				return;

			foreach (Point point in aiPlayer.TargetPath) {

				pos = Map.GetWorldPosition(currentPlayingMap, point, aiPlayer.MyShip);

				Gizmos.color = Color.cyan;
				Gizmos.DrawSphere(pos /*+ new Vector3(width * 0.5f, 0, height * 0.5f)*/, 0.5f);

				Gizmos.color = Color.magenta;

				pos = Map.GetWorldPosition(currentPlayingMap, aiPlayer.TargetPath[aiPlayer.TargetPath.Count - 1], aiPlayer.MyShip);

				Gizmos.DrawSphere(pos /*+ new Vector3(width * 0.5f, 0, height * 0.5f)*/, 0.6f);
			}
		}
	}

	public Color GetColorByCellType(CellType type)
	{
		Color color = Color.white;
		switch (type)
		{
			case CellType.Static:
				color = Color.black;
				break;
			
			case CellType.StaticGround:
				color = new Color(0.245f, 0.245f, 0.245f, 1);
				break;

			case CellType.Destructable:
				color = Color.yellow;
				break;
			
			case CellType.SpawnPoint:
				color = Color.blue;				
				break;

			case CellType.FlagPoint:
				color = Color.green;				
				break;
		}
		return color;
	}

	public void OnStartBattle () {
		currentPlayingMap = GameSetObserver.Instance.CurrentBattle.Map;
		players = GameSetObserver.Instance.Players;
		width = currentPlayingMap.CellSize;
		height = currentPlayingMap.CellSize;
	}

}
