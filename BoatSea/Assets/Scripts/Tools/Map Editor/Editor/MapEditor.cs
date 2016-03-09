//#define RESET_SELECTION_ON_LAYER_SWITCHMENT

using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Maps;
using Aratog.NavyFight.Models.Unity3D.Extensions;

[CustomEditor(typeof(MapInfo))]
public class MapEditor : Editor
{
	const int BrushesNumber = 12;

    public static List<CellType> BrushesList = new List<CellType>()
        {
            CellType.None,
	        CellType.Static,
	        CellType.StaticGround,
	        CellType.Destructable,
	        CellType.SpawnPoint,
	        CellType.FlagPoint,
	        CellType.AINavigatePoint,
	        CellType.GunForwarding,
	        CellType.GunRotating,
	        CellType.GunMounted,
	        CellType.CamperPoint,
	        CellType.Tree,
	        CellType.Bonus,
	        CellType.Bomb,
	        CellType.Shell,
	        CellType.Ship
        };

    Vector3 _mouseHitPos;

	Vector2 tilePosOffset, offscreenOffset;

	bool _isMouseInField, _loadFold, _fieldFold, _saveFold, _createFold = true, _testDraw;

	int _brushToolbar
	{
		get
		{
			int value = 1;
			if (_editor.Layer == LayerType.Cells)
				value = 0;
#if RESET_SELECTION_ON_LAYER_SWITCHMENT
            _editor.Selection.Reset();
#endif
			return value;
		}
		set
		{
			if (value == 0)
				_editor.Layer = LayerType.Cells;
			else
				_editor.Layer = LayerType.Obstacles;
#if RESET_SELECTION_ON_LAYER_SWITCHMENT
            _editor.Selection.Reset();
#endif
		}
	}
	
    string[] toolbarStrings = new string[] { "Drawing", "Editing" };

	MapInfo _editor { get { return (MapInfo)target; } }

    private void OnSceneGUI()
    {
		if (Application.isPlaying)
			return;

		Vector2 tilePos = default(Vector3);

        if (this.UpdateHitPosition())
            SceneView.RepaintAll();

		this.RecalculateMarkerPosition();

        Event current = Event.current;

        _isMouseInField = this.IsOnLayer();

        // Вычисляем тайл, над которым находится мышь, если событие связано с мышкой
        //if (current.type == EventType.MouseDown || current.type == EventType.MouseDrag || current.type == EventType.MouseUp)
            tilePos = this.GetArrayIndexFromTilePosition(this.GetTilePositionFromMouseLocation());

		if (_editor.State == EditorState.Loaded)
		{
			// TODO: try GUI.Window() with GUIStyle.none
			Handles.BeginGUI();
				GUILayout.BeginArea(new Rect(10, 8, 95, 300));
					GUILayout.BeginVertical();
						if (_editor.Layer == LayerType.Cells)
						{
								Color defaultColor = GUI.contentColor;
									// Кисти
									//for(int i=1; i<BrushesNumber; i++) {
                                    foreach(CellType brush in BrushesList)
                                    {
										CellType cType = (CellType)brush;
										GUI.contentColor = _editor.GetColorByCellType(cType);
										string buttonName = cType.ToString();
										if (cType == _editor.Brush)
											buttonName = "<" + buttonName + ">";
										if (GUILayout.Button(buttonName))
											_editor.Brush = cType;
									}
								GUI.contentColor = defaultColor;
						}
						else if (_editor.Layer == LayerType.Obstacles)
						{
							if (_editor.Selection != null && _editor.Selection.State == SelectionState.Selected)
							{
								if (_editor.Selection.IsCopyingCells)
								{
									if (GUILayout.Button("Paste"))
									{
										_editor.Selection.FillMap(ref _editor.Map);
									}
									else if (GUILayout.Button("Merge"))
									{
										_editor.Selection.FillMap(ref _editor.Map, true);
									}
									GUILayout.Space(5);
									if (GUILayout.Button("Flip by X -"))
									{
										_editor.Selection.FlipByX();
									}
									else if (GUILayout.Button("Flip by Y |"))
									{
										_editor.Selection.FlipByY();
									}
									GUILayout.Space(10);
									if (GUILayout.Button("Cancel"))
									{
										_editor.Selection.CancelCopying();
									}
								}
								else
								{
									if (GUILayout.Button("Set as obstacle"))
									{
										Debug.Log("Oops, haven't done yet =/");
									}
									GUILayout.Space(10);
									if (GUILayout.Button("Copy"))
									{
										_editor.Selection.CopyCells(_editor.Map);
									}
									else if (GUILayout.Button("Cut"))
									{
										Debug.Log("Oops, haven't done yet =/");
									}
									GUILayout.Space(5);
									if (GUILayout.Button("Clear"))
									{
										_editor.Selection.ClearCells(ref _editor.Map);
									}
									GUILayout.Space(15);
									GUI.color = GUI.backgroundColor = GUI.contentColor = Color.red;
									if (GUILayout.Button("Set as Red"))
									{
										_editor.Selection.SetCellsColor(ref _editor.Map, CellColor.Red);
									}
									GUI.color = GUI.backgroundColor = GUI.contentColor = Color.blue;
									if (GUILayout.Button("Set as Blue"))
									{
										_editor.Selection.SetCellsColor(ref _editor.Map, CellColor.Blue);
									}
									GUI.color = GUI.backgroundColor = GUI.contentColor = Color.white;
								}
							}
						}
					GUILayout.EndVertical();
				GUILayout.EndArea();

				_brushToolbar = GUI.Toolbar(new Rect(Screen.width * 0.5f - 70, 5, 140, 25), _brushToolbar, toolbarStrings);

                if (_isMouseInField) {
                    string tmp = string.Format("Tile: {0},{1} (type: {2}, ", tilePos.x, tilePos.y, _editor.GetTileType((int)tilePos.x, (int)tilePos.y));
                    //if (_editor.Selection != null && _editor.Selection.State != SelectionState.Nothing)
                        //tmp = string.Format("Tile: {0},{1} ({2})  ||  SelectionSize: {3},{4},{5},{6}", tilePos.x, tilePos.y, _editor.GetTileType((int)tilePos.x, (int)tilePos.y), _editor.Selection.StartPoint.X, _editor.Selection.StartPoint.Y, _editor.Selection.Width, _editor.Selection.Height);
			        GUI.Label(new Rect(5, Screen.height - 60, 320, 100), tmp + "color: " + _editor.GetTileColor((int)tilePos.x, (int)tilePos.y) +")");
				}

                Rect tipsPos = new Rect(Screen.width - 75, Screen.height - 60, 1000, 1000);

                if (_editor.Layer == LayerType.Cells)
                {
                    GUI.Label(tipsPos, "LMB: Draw");
                    tipsPos.y -= 15;
                    GUI.Label(tipsPos, "RMB: Erase");
                }
                else
                {
					if (_editor.Selection.State == SelectionState.Nothing || _editor.Selection.State == SelectionState.Selecting)
					{
						tipsPos.x -= 10;
	                    GUI.Label(tipsPos, "Select field");
					}
					else if (_editor.Selection.State == SelectionState.Selected)
					{
						tipsPos.x -= 55;
	                    GUI.Label(tipsPos, "RMB: Move selection");
					}
                    //tipsPos.y -= 15;
                    //GUI.Label(tipsPos, "RMB: Erase");
                }

	        Handles.EndGUI();
		}
		
		if (current.type == EventType.MouseDown)
		{
            if (!_editor.Selection.IsCopyingCells && current.button == 0 && _editor.Layer == LayerType.Obstacles && _isMouseInField && _editor.Selection.State != SelectionState.Selecting) {
				_editor.Selection.StartSelection((int)tilePos.x, (int)tilePos.y);
			}
		}
		else if (current.type == EventType.MouseUp)
		{
			if (_editor.Selection.State == SelectionState.Selecting) {
                _editor.Selection.EndSelection((int)tilePos.x, (int)tilePos.y);
			}
            _isMouseInField = false;
		}

        // Если мышь находится над полем и не зажата кнопка Alt
        if (current.alt == false)
        {
            if (current.type == EventType.MouseDown || current.type == EventType.MouseDrag)
            {
				if (_editor.Layer == LayerType.Cells && _isMouseInField)
				{
		            if (current.button == 1)
		            {
						_editor.Erase((int)tilePos.x, (int)tilePos.y);
		                current.Use();
		            }
		            else if (current.button == 0)
		            {
						_editor.Draw((int)tilePos.x, (int)tilePos.y);
						_editor.AnyChange = true;
		                current.Use();
		            }
				}
				else if (_editor.Layer == LayerType.Obstacles)
				{
					if (!_editor.Selection.IsCopyingCells && current.button == 0 && _isMouseInField)
		            {
                        _editor.Selection.UpdateSelection((int)tilePos.x, (int)tilePos.y);
		                current.Use();
		            }
					else if ((current.button == 1 && _editor.Selection.State != SelectionState.Nothing) || _editor.Selection.IsCopyingCells)
					{						
						if (current.type == EventType.MouseDown)
							tilePosOffset = _editor.Selection.StartPoint.ToVector() - tilePos;

						Point selectionStartPoint = _editor.Selection.StartPoint;
						float selectionWidth = _editor.Selection.Width;
						float selectionHeight = _editor.Selection.Height;
						float mapLimitWidth = _editor.Map.FieldWidth - (selectionWidth > 0 ? 1 : 0);
						float mapLimitHeight = _editor.Map.FieldHeight - (selectionHeight > 0 ? 1 : 0);

						Vector3 tileStartPoint = tilePos + tilePosOffset;

						// Ограничение 
					    if ((selectionStartPoint.X - (selectionWidth < 0 ? selectionWidth : 0)) > mapLimitWidth)
						{
							tileStartPoint.x = mapLimitHeight;
						}
					    else if ((selectionStartPoint.X - (selectionWidth < 0 ? 0 : selectionWidth)) < (selectionWidth > 0 ? -1 : 0))
						{
							tileStartPoint.x = (selectionWidth < 0 ? 0 : selectionWidth);
						}

					    if ((selectionStartPoint.Y - (selectionHeight < 0 ? selectionHeight : 0)) > mapLimitHeight)
						{
							tileStartPoint.y = mapLimitHeight;
						}
					    else if ((selectionStartPoint.Y - (selectionHeight < 0 ? 0 : selectionHeight)) < (selectionHeight > 0 ? -1 : 0))
						{
							tileStartPoint.y = (selectionHeight < 0 ? 0 : selectionHeight);
						}
						//

					    _editor.Selection.StartPoint = tileStartPoint.ToPoint();

						current.Use();
					}
				}
			}
		}

	}

    public override void OnInspectorGUI()
	{
		//EditorGUIUtility.LookLikeInspector();

		if (_editor.State == EditorState.CreateOrLoad)
		{
			GUILayout.Space(10);
			_createFold = EditorGUILayout.Foldout(_createFold, "Create");
			if (_createFold) {
				GUILayout.Label("Set map size and press 'Create' button.");
				GUILayout.Space(10);
				GUILayout.Label("Field width:");
				_editor.SharedFieldWidth = ParseInt(GUILayout.TextField(_editor.SharedFieldWidth.ToString(), 5));
				GUILayout.Label("Field height:");
				_editor.SharedFieldHeight = ParseInt(GUILayout.TextField(_editor.SharedFieldHeight.ToString(), 5));
				GUILayout.Label("Cell size:");
				_editor.CellSizeTextField = GUILayout.TextField(_editor.CellSizeTextField, 5);
				_editor.CellSize = ParseFloat(_editor.CellSizeTextField);
				if (GUI.changed) {
					SceneView.RepaintAll();
				}
				GUILayout.Space(10);
				if (GUILayout.Button("Create")) {
					if (_editor.SharedFieldWidth % 2 != 0 || _editor.SharedFieldHeight % 2 != 0) {
						Debug.LogError("MapInfo: can't create map. The field size should be a multiple of two.");
					} else {
						_editor.Create(_editor.SharedFieldWidth, _editor.SharedFieldHeight, _editor.CellSize);
					}
				}
				if (!_loadFold && !_createFold)
					GUILayout.Space(10);
			}
			GUILayout.Space(10);
			GUILayout.Label("or");
			GUILayout.Space(10);
			_loadFold = EditorGUILayout.Foldout(_loadFold, "Load");
			if (_loadFold) {
				GUILayout.Label("Set path and load existing map.");
				GUILayout.Space(10);
				GUILayout.Label("Maps directory path:");
				_editor.MapsPath = GUILayout.TextArea(_editor.MapsPath);
				GUILayout.Label("Map name:");
				_editor.SharedName = GUILayout.TextField(_editor.SharedName);
				GUILayout.Space(10);
				if (GUILayout.Button("Load")) {
					_editor.Load(_editor.SharedName);
				}
				GUILayout.Space(10);
			}
			GUILayout.Space(15);
		}
		else
		{
			DrawDefaultInspector();
			if (_editor.AnyChange) {
				_saveFold = EditorGUILayout.Foldout(_saveFold, "Save");
				if (_saveFold) {
					GUILayout.Label("Maps directory path:");
					_editor.MapsPath = GUILayout.TextArea(_editor.MapsPath);
					GUILayout.Label("Map file name:");
					_editor.SharedName = GUILayout.TextField(_editor.SharedName);
					if (_editor.AnyChange) {
						GUILayout.Space(5);
						if (GUILayout.Button("Save")) {
							if (_editor.Save(_editor.SharedName)) {
								_editor.AnyChange = false;
							}
						}
					}
				}
			}
			GUILayout.Space(3);
			_fieldFold = EditorGUILayout.Foldout(_fieldFold, "Edit field");
			if (_fieldFold) {
				GUILayout.Label("New field width:");
				_editor.SharedFieldWidth = ParseInt(GUILayout.TextField(_editor.SharedFieldWidth.ToString(), 5));
				GUILayout.Label("New field height:");
				_editor.SharedFieldHeight = ParseInt(GUILayout.TextField(_editor.SharedFieldHeight.ToString(), 5));
				GUILayout.Label("New cell size:");
				_editor.CellSizeTextField = GUILayout.TextField(_editor.CellSizeTextField, 5);
				_editor.CellSize = ParseFloat(_editor.CellSizeTextField);
				if (GUI.changed) {
					SceneView.RepaintAll();
				}
				if (GUILayout.Button("ReCreate")) {
					if (_editor.SharedFieldWidth % 2 != 0 || _editor.SharedFieldHeight % 2 != 0) {
						Debug.LogError("MapInfo: can't create map. The field size should be a multiple of two.");
					} else {
						_editor.Create(_editor.SharedFieldWidth, _editor.SharedFieldHeight, _editor.CellSize);
					}
				}
				GUILayout.Space(5);
				GUILayout.Label("or");
				GUILayout.Space(5);
				if (GUILayout.Button("Clear all")) {
					_editor.ClearAll();
					SceneView.RepaintAll();
				}
				GUILayout.Space(10);
			}
		}
    }

    private void OnEnable()
    {
        Tools.current = Tool.View;
        Tools.viewTool = ViewTool.FPS;
    }
	
	int ParseInt(string value)
	{
		int number = 0;
		bool result = Int32.TryParse(value, out number);
		if (!result) {
			if (value == null)
				value = "";
		}
		return number;
	}
	
	float ParseFloat(string value)
	{
		float number = 0;
		//if (value.Contains(".")) {
			bool result = float.TryParse(value, out number);
			if (!result) {
				if (value == null)
					value = "";
			}
		//}
		return number;
	}

	Vector2 GetArrayIndexFromTilePosition(Vector2 tiles)
	{
		return tiles + new Vector2(_editor.SharedFieldWidth * 0.5f, _editor.SharedFieldHeight * 0.5f);;
	}

    private Vector2 GetTilePositionFromMouseLocation()
    {
        // Вычисляем столбец и строку по координата мыши
        var pos = new Vector3(_mouseHitPos.x / _editor.CellSize, _editor.transform.position.y, _mouseHitPos.z / _editor.CellSize);

		// Костыль для корректного определения тайла с отрицательным индексом
		if (pos.x < 0)
			pos.x -= 1;
		if (pos.z < 0)
			pos.z -= 1;

		// Понятия не имею зачем это и как оно работает
        pos = new Vector3((int)Math.Round(pos.x, 1, MidpointRounding.ToEven), 0, (int)Math.Round(pos.z, 1, MidpointRounding.ToEven));

		// Clamp нужен для того, чтобы возвращаемый тайл не выходил за края поля
        return new Vector2(
				Mathf.Clamp((int)pos.x, -_editor.SharedHalfFieldWidth, _editor.SharedHalfFieldWidth-1),
				Mathf.Clamp((int)pos.z, -_editor.SharedHalfFieldHeight, _editor.SharedHalfFieldHeight-1)
			);
    }

    // Возвращает true если мышь находится над полем
    private bool IsOnLayer(Vector3 coords = default(Vector3))
    {
		if (coords == default(Vector3))
			coords = _mouseHitPos;
        return coords.x > (-_editor.SharedHalfFieldWidth * _editor.CellSize) && coords.x < (_editor.SharedHalfFieldWidth * _editor.CellSize) &&
               coords.z > (-_editor.SharedHalfFieldHeight * _editor.CellSize) && coords.z < (_editor.SharedHalfFieldHeight * _editor.CellSize);
    }

    /// <summary>
    /// Recalculates the position of the marker based on the location of the mouse pointer.
    /// </summary>
    private void RecalculateMarkerPosition()
    {
        // store the tile location (Column/Row) based on the current location of the mouse pointer
        var tilepos = this.GetTilePositionFromMouseLocation();

        // store the tile position in world space
        var pos = _editor.transform.position + new Vector3(tilepos.x * _editor.CellSize, 0, tilepos.y * _editor.CellSize);

        // set the TileMap.MarkerPosition value
        _editor.MarkerPosition = new Vector3(pos.x + (_editor.CellSize / 2), 0, pos.z + (_editor.CellSize / 2));
    }

    /// <summary>
    /// Calculates the position of the mouse over the tile map in local space coordinates.
    /// </summary>
    /// <returns>Returns true if the mouse is over the tile map.</returns>
    private bool UpdateHitPosition()
    {
        // get reference to the tile map component
        var map = (MapInfo)this.target;

        // build a plane object that 
        var p = new Plane(map.transform.TransformDirection(Vector3.up), map.SharedPosition);

        // build a ray type from the current mouse position
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // stores the hit location
        var hit = new Vector3();

        // stores the distance to the hit location
        float dist;

        // cast a ray to determine what location it intersects with the plane
        if (p.Raycast(ray, out dist))
        {
            // the ray hits the plane so we calculate the hit location in world space
            hit = ray.origin + (ray.direction.normalized * dist);
        }

        // convert the hit location from world space to local space
        var value = map.transform.InverseTransformPoint(hit);
		
        // if the value is different then the current mouse hit location set the 
        // new mouse hit location and return true indicating a successful hit test
        if (value != this._mouseHitPos)
        {
            this._mouseHitPos = value;
            return true;
        }

        // return false if the hit test failed
        return false;
    }
}