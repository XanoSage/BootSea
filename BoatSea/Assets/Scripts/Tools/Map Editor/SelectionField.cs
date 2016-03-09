using UnityEngine;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Maps;
using System;

public enum SelectionState
{
	Nothing,
	Selecting,
	Selected
}

public class SelectionField
{
    public Point StartPoint, EndPoint, RightPoint;
    public int Width, Height;

    public SelectionState State = SelectionState.Nothing;

    public void StartSelection(int x, int y)
    {
		CancelCopying();

        StartPoint = new Point(x, y);
    }

    public void UpdateSelection(int x, int y)
    {
        Width = StartPoint.X - x;
        Height = StartPoint.Y - y;

		if (Width < 0)
			Width -= 1;
		else if (Width > 0)
			Width += 1;
		else
			Width = -1;

		if (Height < 0)
			Height -= 1;
		else if (Height > 0)
			Height += 1;
		else
			Height = -1;

		State = SelectionState.Selecting;
    }

    public void EndSelection(int x, int y)
    {
        EndPoint = new Point(x, y);

		float w = Width;
		float h = Height;

		if (w < 0)
			w -= 1;
		else if (w > 0)
			w += 1;
		else
			w = -1;

		if (h < 0)
			h -= 1;
		else if (h > 0)
			h += 1;
		else
			h = -1;

		Vector2 sign = (new Vector2(w, h)).normalized;

		sign.x = Mathf.Round(sign.x);
		if (sign.x == -1) {
			int storedX = (int)StartPoint.X;
			StartPoint = new Point((int)EndPoint.X, StartPoint.Y);
			EndPoint = new Point(storedX, EndPoint.Y);
			Width = -Width;
		}

		sign.y = Mathf.Round(sign.y);
		if (sign.y == -1) {
			int storedY = (int)StartPoint.Y;
			StartPoint = new Point(StartPoint.X, (int)EndPoint.Y);
			EndPoint = new Point(EndPoint.X, storedY);
			Height = -Height;
		}

		State = SelectionState.Selected;
    }

    public void Reset()
    {
        State = SelectionState.Nothing;
    }

	[HideInInspector] public Cell[] CopyingCells;
	
	public Cell this [int x, int y] {
		get {
			if (CopyingCells.Length < y * Width + x)
				return null;
			return CopyingCells[y * Width + x];
		}
		set {
			try {
				CopyingCells[y * Width + x] = value;
			}
			catch (ArgumentOutOfRangeException e) {
				Debug.LogError(e);
			}
		}
	}

	public bool IsCopyingCells;
	
	public void CopyCells(Map map)
	{
		try {
			CopyingCells = new Cell[Width * Height];
			for(int j=0; j<Width; j++) {
				for(int k=0; k<Height; k++) {
					CopyingCells[k*Width + j] = map[StartPoint.X - j, StartPoint.Y - k];
				}
			}
		} catch {
			Debug.LogError("Bug: " + Width * Height + "  |  " + EndPoint.X + " , " + EndPoint.Y);
		} finally {
			IsCopyingCells = true;
		}
	}
	
	public void FlipByX()
	{
		for (int i = 0; i != Width / 2; i++) {
			for (int j = 0; j != Height; j++) {
				Cell temp = this[i, j];
				this[i, j] = this[Width - i - 1, j];
				this[Width - i - 1, j] = temp;
			}
		}
	}

	public void FlipByY()
	{
		for (int i = 0; i != Width; i++) {
			for (int j = 0; j != Height / 2; j++) {
				Cell temp = this[i, j];
				this[i, j] = this[i, Height - j - 1];
				this[i, Height - j - 1] = temp;
			}
		}
	}
	
	public void SetCellsColor(ref Map map, CellColor color)
	{
		for(int j=0; j<Width; j++) {
			for(int k=0; k<Height; k++) {
				map[StartPoint.X - j, StartPoint.Y - k].Color = color;
			}
		}
	}
	
	public void ClearCells(ref Map map)
	{
		for(int j=0; j<Width; j++) {
			for(int k=0; k<Height; k++) {
				map[StartPoint.X - j, StartPoint.Y - k] = new Cell(CellType.None, new ObstacleEvidence());
			}
		}
	}
	
	public void FillMap(ref Map map, bool merge = false)
	{
		for(int j=0; j<Width; j++) {
			for(int k=0; k<Height; k++) {
				if (merge && CopyingCells[k*Width + j].Type != CellType.None) {
					map[StartPoint.X - j, StartPoint.Y - k] = CopyingCells[k*Width + j];
				} else if (!merge) {
					map[StartPoint.X - j, StartPoint.Y - k] = CopyingCells[k*Width + j];
				}
			}
		}

		CancelCopying();
	}
	
/*
private void HorizontalArrayReflection () {
	for (int i = 0; i != N / 2; i++) {
		for (int j = 0; j != M; j++) {
			int temp = someIntArray[i][j];
			someIntArray[i][j] = someIntArray[N - i - 1][j];
			someIntArray[N - i - 1][j] = temp;
		}
	}
}

private void VerticalArrayReflection () {
	for (int i = 0; i != N; i++) {
		for (int j = 0; j != M/2; j++) {
			int temp = someIntArray[i][j];
			someIntArray[i][j] = someIntArray[i][M - j - 1];
			someIntArray[i][M - j - 1] = temp;
		}
	}
}
*/
	
	public void CancelCopying()
	{
		CopyingCells = null;
		IsCopyingCells = false;
	}
}