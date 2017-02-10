using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridCoord {
	public int x, y;

	public GridCoord(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static bool operator ==(GridCoord c1, GridCoord c2) {
		return c1.x == c2.x && c1.y == c2.y;
	}

	public static bool operator !=(GridCoord c1, GridCoord c2) {
		return !(c1 == c2);
	}
}

public class LevelGrid : MonoBehaviour {
	PlatformSquare[,] m_grid;
	public Vector2 cellSize = Vector2.one;

	public PlatformSquare[,] Grid {
		get { return m_grid; }
	}

	public void InitializeGrid(char[][] levelData, EmptyPlatformSquare emptyPrefab, SolidPlatformSquare solidPrefab, WinPlatformSquare winPrefab, Player player) {
		int gridWidth = levelData[0].Length;
		int gridHeight = levelData.Length;
		Debug.Log (gridWidth);
		Debug.Log (gridHeight);

		PlatformSquareData emptySquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Empty Square Prototype");
		PlatformSquareData solidPlatformData = Resources.Load<PlatformSquareData> ("Platform Squares/Solid Platform Prototype");
		PlatformSquareData winSquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Win Platform Prototype");

		GridCoord playerStart = new GridCoord(0, 0);
		m_grid = new PlatformSquare[gridWidth, gridHeight];
		for (int x = 0; x < gridWidth; ++x) {
			for (int y = 0; y < gridHeight; ++y) {
				if (levelData [y][x] == 'e') {
					m_grid [x, y] = Instantiate<WinPlatformSquare> (winPrefab, this.transform);
					m_grid [x, y].InitializeSquareData (winSquareData);
				} else if (levelData [y][x] == '-') {
					m_grid [x, y] = Instantiate<EmptyPlatformSquare> (emptyPrefab, this.transform);
					m_grid [x, y].InitializeSquareData (emptySquareData);
				} else if (levelData [y][x] == 'o') {
					m_grid [x, y] = Instantiate<SolidPlatformSquare> (solidPrefab, this.transform);
					m_grid [x, y].InitializeSquareData (solidPlatformData);
				} else if (levelData [y][x] == 's') {
					playerStart = new GridCoord (x, y);
					m_grid [x, y] = Instantiate<SolidPlatformSquare> (solidPrefab, this.transform);
					m_grid [x, y].InitializeSquareData (solidPlatformData);
				} else {
					Debug.Log (string.Format ("Unknown grid square type '{0}' at ({1}, {2})", levelData [x][y], x, y));
					Destroy (m_grid [x, y]);
					continue;
				}

				m_grid [x, y].name = "Grid (" + x + ", " + y + ")";
				m_grid [x, y].Grid = this;
				m_grid [x, y].GridPosition = new GridCoord (x, y);
			}
		}

		player.Grid = this;
		player.CurrentPosition = playerStart;
	}

	public Vector2 GetCoordInWorldSpace(GridCoord coords) {
		return (Vector2)transform.TransformPoint (new Vector3 (coords.x * cellSize.x, coords.y * cellSize.y, 0f));
	}

	public bool IsValidGridPosition(GridCoord coords) {
		return (coords.x >= 0 && coords.x < m_grid.GetLength (0) && coords.y >= 0 && coords.y < m_grid.GetLength (1));
	}
}
