using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Maps;

public enum EditorState
{
	CreateOrLoad,
	Loaded
}

public enum LayerType
{
	Cells,
	Obstacles
}

public class MapInfo : MonoBehaviour
{
	[SerializeField]
	bool _drawGizmosInPlayMode = true;

	public Map Map;

	// Переменные для редактора. Объявлены в этом классе (а не в EditorForMapInfo.cs), чтобы их данные сериализовались (и, соответственно, сохранялись)
	[HideInInspector] public CellType Brush = CellType.Static;
	[HideInInspector] public EditorState State = EditorState.CreateOrLoad;
	[HideInInspector] public LayerType Layer = LayerType.Cells;
	[HideInInspector] public float CellSize = 1;
	[HideInInspector] public string MapsPath, CellSizeTextField = "2.048", SharedName = "Tutorial";
	[HideInInspector] public bool AnyChange;
	[HideInInspector] public Vector3 MarkerPosition, SharedPosition, SharedHalfPosition;
	[HideInInspector] public int SharedFieldWidth = 20, SharedFieldHeight = 40, SharedHalfFieldWidth, SharedHalfFieldHeight;
    [System.NonSerialized] public SelectionField Selection = new SelectionField();

	void Awake()
	{
		UpdateSharedInfo();
	}

	// Метод для обновления информации о карте
	void UpdateSharedInfo()
	{
		if (string.IsNullOrEmpty(MapsPath)) {
			MapsPath = System.IO.Path.Combine(Application.dataPath, "Maps");
		
		}
		Debug.Log(MapsPath);
		if (State == EditorState.Loaded) {
			SharedFieldWidth = Map.FieldWidth;
			SharedFieldHeight = Map.FieldHeight;
			SharedHalfFieldWidth = (int)(Map.FieldWidth / 2);
			SharedHalfFieldHeight = (int)(Map.FieldHeight / 2);
		}

		SharedHalfPosition = new Vector3(SharedFieldWidth * CellSize * 0.5f, 0, SharedFieldHeight * CellSize * 0.5f) * -1;
		SharedPosition = this.transform.position + SharedHalfPosition;
	}

/*
	public Cell[] copyingCells;
	
	void CopySelected(int M, int N)
	{
		copyingCells = new Cell[M * N];
		int k = 0;
		for (int i = Selection.StartPoint.X; i != Selection.StartPoint.X + Selection.Width; i++)
		{
			for (int j = Selection.StartPoint.Y; j != Selection.StartPoint.Y + Selection.Height; j++)
			{
				copyingCells[k] = map[i,j];
			}
		}
	}
*/

    void OnDrawGizmosSelected()
    {
		if (!_drawGizmosInPlayMode && Application.isPlaying)
			return;

		// Обновляем информацию о карте
		UpdateSharedInfo();

		// Сохраним позиции для более удобного обращения в дальнейшем
        float mapWidth = SharedFieldWidth * CellSize;
        float mapHeight = SharedFieldHeight * CellSize;
        Vector3 position = SharedPosition;

        // Рисуем границы поля
        Gizmos.color = Color.white;
        Gizmos.DrawLine(position, position + new Vector3(mapWidth, 0, 0));
        Gizmos.DrawLine(position, position + new Vector3(0, 0, mapHeight));
        Gizmos.DrawLine(position + new Vector3(mapWidth, 0, 0), position + new Vector3(mapWidth, 0, mapHeight));
        Gizmos.DrawLine(position + new Vector3(0, 0, mapHeight), position + new Vector3(mapWidth, 0, mapHeight));

        // Рисуем линии, которые образуют клетки
        Gizmos.color = Color.grey;
        for (float i = 1; i < SharedFieldWidth; i++) {
            Gizmos.DrawLine(position + new Vector3(i * CellSize, 0, 0), position + new Vector3(i * CellSize, 0, mapHeight));
        }
        for (float i = 1; i < SharedFieldHeight; i++) {
            Gizmos.DrawLine(position + new Vector3(0, 0, i * CellSize), position + new Vector3(mapWidth, 0, i * CellSize));
        }

		if (State == EditorState.Loaded && Map.Cells != null)
		{
			for(int j=0; j<SharedFieldWidth; j++)
			{
				for(int k=0; k<SharedFieldHeight; k++)
				{
					var cell = Map.Cells[k*SharedFieldWidth + j];
					if (cell.Type != CellType.None)
					{
						Gizmos.color = GetColorByCellType(cell.Type);
						Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, (Layer == LayerType.Cells) ? 1 : 0.6f);
		        		Gizmos.DrawCube(position + new Vector3(CellSize * 0.5f, 0, CellSize * 0.5f) + new Vector3(j * CellSize, 0, k * CellSize),
							new Vector3(CellSize * 0.91f, 0.01f, CellSize * 0.91f));
					}
				}
			}

	        // Рисуем маркер выделения, если мы не в Playmode
			if (!Application.isPlaying)
			{
				if (Layer == LayerType.Cells)
				{
					Gizmos.color = Color.green;
			        Gizmos.DrawWireCube(this.MarkerPosition + new Vector3(0, 0, 0),
	                    new Vector3(CellSize * 0.86f, 0.01f, CellSize * 0.86f) * 1.1f);
			        Gizmos.color = GetColorByCellType(Brush);
			        Gizmos.DrawCube(this.MarkerPosition, new Vector3(CellSize * 0.86f, 0.01f, CellSize * 0.86f) * 1.1f);
				}
				else if (Selection.State != SelectionState.Nothing)
				{
                    Gizmos.color = new Color(0.2f, 0.2f, 0.8f, 0.35f);
                    Vector3 selectionFieldSize = new Vector3(Selection.Width * Map.CellSize, 0.1f, Selection.Height * Map.CellSize);
					Vector3 selectionFieldPos = position
						+ new Vector3((Selection.StartPoint.X + ((Selection.Width > 0) ? 1 : 0)) * Map.CellSize, 1,
							(Selection.StartPoint.Y + ((Selection.Height > 0) ? 1 : 0)) * Map.CellSize)
								- new Vector3(selectionFieldSize.x * 0.5f, 0, selectionFieldSize.z * 0.5f);
                    Gizmos.DrawCube(selectionFieldPos, selectionFieldSize);
					Gizmos.color = Color.green;
			        Gizmos.DrawWireCube(selectionFieldPos, selectionFieldSize);
					if (Selection.IsCopyingCells)
					{
						for(int j=0; j<Selection.Width; j++) {
							for(int k=0; k<Selection.Height; k++) {
								var cell = Selection.CopyingCells[k*Selection.Width + j];
								Gizmos.color = GetColorByCellType(cell.Type);
								Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, (Layer == LayerType.Cells) ? 1 : 0.6f);
				        		Gizmos.DrawCube(position + new Vector3(CellSize * 0.5f, 0, CellSize * 0.5f)
									+ new Vector3((Selection.StartPoint.X - j) * CellSize, 0, (Selection.StartPoint.Y - k) * CellSize),
									new Vector3(CellSize * 0.91f, 0.01f, CellSize * 0.91f));
							}
						}
					}
                }
				else
				{
					Gizmos.color = Color.green;
			        Gizmos.DrawWireCube(this.MarkerPosition + new Vector3(0, 0, 0),
	                    new Vector3(CellSize * 0.86f, 0.01f, CellSize * 0.86f) * 1.1f);
				}
			}
		}
    }

    float GetNotZero(float t)
    {
        return (t >= -0.1f && t <= 0.1f) ? 1 : t;
    }
	
	public CellColor GetTileColor(int x, int y)
	{
		CellColor t = CellColor.None;
		if (Map[x, y] != null)
			t = Map[x, y].Color;
		return t;
	}
	
	public CellType GetTileType(int x, int y)
	{
		CellType t = CellType.None;
		if (Map[x, y] != null)
			t = Map[x, y].Type;
		return t;
	}

#region Map Editor API (public methods)
	
	public void Create(int width, int height, float cellSize)
	{
		Map.CellSize = cellSize;
		ReCreateCells(width, height);
		State  = EditorState.Loaded;
	}
	
	public void ReCreateCells(int width, int height)
	{
		if (Map.Cells != null)
		{
			// Вот тут надо сформировать новый массив, чтобы не затереть предыдущие данные
		}
		
		Map.FieldWidth = width;
		Map.FieldHeight = height;
		
		Map.Cells = new Cell[Map.FieldWidth * Map.FieldHeight];
		for(int i=0; i<Map.FieldWidth; i++) {
			for(int j=0; j<Map.FieldHeight; j++) {
				Map.Cells[j*Map.FieldWidth + i] = new Cell(CellType.None, new ObstacleEvidence());
			}
		}
	}

	public void Draw(int x, int y)
	{
		Map[x, y] = new Cell(Brush, new ObstacleEvidence());
	}

	public void Erase(int x, int y) 
	{
		//Map[x, y] = new Cell(CellType.None, new ObstacleEvidence());
		Map[x, y].Type = CellType.None;
		Map[x, y].Evidence = null;
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
				color = Color.cyan;
				break;

			case CellType.Destructable:
				color = Color.yellow;
				break;
			
			case CellType.SpawnPoint:
				color = Color.blue;				
				break;

			case CellType.FlagPoint:
				color = Color.red;			
				break;

            case CellType.AINavigatePoint:
				color = Color.white;
				break;
			
			case CellType.GunForwarding:
				color = new Color(0.35f, 0.5f, 1.0f, 1);
				break;
				
			case CellType.GunRotating:
				color = new Color(0.55f, 0.7f, 1.0f, 1);
				break;
				
			case CellType.GunMounted:
				color = new Color(0.4f, 0.25f, 1.0f, 1);
				break;

			case CellType.CamperPoint:
				color = Color.magenta;
				break;

			case CellType.Tree:
				color = Color.green;
				break;			
		}
		return color;
	}

	public Vector3 GetTilePos(int x, int y)
	{
		return (SharedPosition + new Vector3(CellSize * 0.5f, 0, CellSize * 0.5f) + new Vector3(x * CellSize, 0, y * CellSize));
	}

	public void ClearAll()
	{
		for(int i=0; i<Map.FieldWidth; i++) {
			for(int j=0; j<Map.FieldHeight; j++) {
				Map.Cells[j*Map.FieldWidth + i].Type = CellType.None;//new Cell(CellType.None, new ObstacleEvidence());
				Map.Cells[j*Map.FieldWidth + i].Evidence = new ObstacleEvidence();
			}
		}
	}

	public void Load(string name = "Map")
	{
		string filename = System.IO.Path.Combine(MapsPath, name + ".xml");
		Debug.Log (filename);
		Map = Map.Load(filename);
	}

	public bool Save(string name = "Map")
	{
		bool success = false;
		string filename = System.IO.Path.Combine(Application.persistentDataPath, name + ".xml");
		success = Map.Save(filename);
#if UNITY_EDITOR
		AssetDatabase.Refresh();
#endif
		Debug.Log("Saved to " + filename);
		return success;
	}
#endregion

}