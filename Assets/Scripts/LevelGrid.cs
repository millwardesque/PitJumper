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

	public void InitializeGrid(char[][] levelData, EmptyPlatformSquare emptyPrefab, SolidPlatformSquare solidPrefab, WinPlatformSquare winPrefab, ToggleTriggerPlatformSquare toggleTriggerPlatformSquare, TriggeredPlatformSquare triggeredPlatformSquare, Player player) {
		int gridWidth = levelData[0].Length;
		int gridHeight = levelData.Length;

		PlatformSquareData emptySquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Empty Square Prototype");
		PlatformSquareData solidPlatformData = Resources.Load<PlatformSquareData> ("Platform Squares/Solid Platform Prototype");
		PlatformSquareData winSquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Win Platform Prototype");
		PlatformSquareData toggleTriggerSquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Toggle Trigger Prototype");
		PlatformSquareData triggeredSquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Triggered Platform Prototype");

		DeleteGrid ();

		GridCoord playerStart = new GridCoord(0, 0);
		m_grid = new PlatformSquare[gridWidth, gridHeight];
		ToggleTriggerPlatformSquare toggleTriggerSquare1 = null;
		TriggeredPlatformSquare triggeredSquare1 = null;

		ToggleTriggerPlatformSquare toggleTriggerSquare2 = null;
		TriggeredPlatformSquare triggeredSquare2 = null;
		for (int x = 0; x < gridWidth; ++x) {
			for (int y = 0; y < gridHeight; ++y) {
				if (levelData [y][x] == 'e') {
					ReplaceSquare (winPrefab, winSquareData, x, y);
				} else if (levelData [y][x] == '-') {
					ReplaceSquare (emptyPrefab, emptySquareData, x, y);
				} else if (levelData [y][x] == 'o') {
					ReplaceSquare (solidPrefab, solidPlatformData, x, y);
				} else if (levelData [y][x] == 's') {
					playerStart = new GridCoord (x, y);
					ReplaceSquare (solidPrefab, solidPlatformData, x, y);
				} else if (levelData [y][x] == 'T') {
					ReplaceSquare (toggleTriggerPlatformSquare, toggleTriggerSquareData, x, y);
					toggleTriggerSquare1 = m_grid [x, y] as ToggleTriggerPlatformSquare;
				} else if (levelData [y][x] == 't') {
					ReplaceSquare (triggeredPlatformSquare, triggeredSquareData, x, y);
					triggeredSquare1 = m_grid [x, y] as TriggeredPlatformSquare;
				} else if (levelData [y][x] == 'U') {
					ReplaceSquare (toggleTriggerPlatformSquare, toggleTriggerSquareData, x, y);
					toggleTriggerSquare2 = m_grid [x, y] as ToggleTriggerPlatformSquare;
				} else if (levelData [y][x] == 'u') {
					ReplaceSquare (triggeredPlatformSquare, triggeredSquareData, x, y);
					triggeredSquare2 = m_grid [x, y] as TriggeredPlatformSquare;
				}
				else {
					Debug.Log (string.Format ("Unknown grid square type '{0}' at ({1}, {2})", levelData [x][y], x, y));
					Destroy (m_grid [x, y]);
					continue;
				}
			}
		}

		if (triggeredSquare1 || toggleTriggerSquare1) {
			if (triggeredSquare1 && toggleTriggerSquare1) {
				toggleTriggerSquare1.triggerSquare = triggeredSquare1;
				triggeredSquare1.toggleSquare = toggleTriggerSquare1;
			} else {
				Debug.Log ("Warning: Trigger square #1 has no toggle square #1, or vice-versa");
			}
		}

		if (triggeredSquare2 || toggleTriggerSquare2) {
			if (triggeredSquare2 && toggleTriggerSquare2) {
				toggleTriggerSquare2.triggerSquare = triggeredSquare2;
				triggeredSquare2.toggleSquare = toggleTriggerSquare2;
			} else {
				Debug.Log ("Warning: Trigger square #2 has no toggle square #2, or vice-versa");
			}
		}

		player.Grid = this;
		player.CurrentPosition = playerStart;
	}

	public void ReplaceSquare(PlatformSquare prefab, PlatformSquareData squareData, int x, int y) {
		if (m_grid [x, y] != null) {
			Destroy (m_grid [x, y].gameObject);
		}

		m_grid [x, y] = Instantiate<PlatformSquare> (prefab, this.transform);
		m_grid [x, y].InitializeSquareData (squareData);
		m_grid [x, y].name = "Grid (" + x + ", " + y + ")";
		m_grid [x, y].Grid = this;
		m_grid [x, y].GridPosition = new GridCoord (x, y);
	}

	public Vector2 GetCoordInWorldSpace(GridCoord coords) {
		return (Vector2)transform.TransformPoint (new Vector3 (coords.x * cellSize.x, coords.y * cellSize.y, 0f));
	}

	public bool IsValidGridPosition(GridCoord coords) {
		return (coords.x >= 0 && coords.x < m_grid.GetLength (0) && coords.y >= 0 && coords.y < m_grid.GetLength (1));
	}

	void DeleteGrid() {
		PlatformSquare[] squares = GetComponentsInChildren<PlatformSquare> ();
		for (int i = 0; i < squares.Length; ++i) {
			GameObject.Destroy (squares[i].gameObject);
		}
	}

	public LevelDefinition AsLevelDefinition() {
		LevelDefinition level = new LevelDefinition ();
		char[][] levelData = new char[m_grid.GetLength (1)][];

		int triggerCount = 0;
		int toggleCount = 0;
		for (int y = 0; y < m_grid.GetLength(1); ++y) {
			levelData[y] = new char [m_grid.GetLength(0)];
			for (int x = 0; x < m_grid.GetLength(0); ++x) {
				if (m_grid[x, y] is EmptyPlatformSquare) {
					levelData [y] [x] = '-';
				}
				else if (m_grid[x, y] is SolidPlatformSquare) {
					levelData [y] [x] = 'o';
				}
				else if (m_grid[x, y] is WinPlatformSquare) {
					levelData [y] [x] = 'e';
				}
				else if (m_grid[x, y] is ToggleTriggerPlatformSquare) {
					if (levelData [y] [x] == 0) {

						char triggerChar = 'T';
						char toggleChar = 't';
						levelData [y] [x] = triggerChar;
						ToggleTriggerPlatformSquare toggleSquare = m_grid [x, y] as ToggleTriggerPlatformSquare;
						if (toggleSquare.triggerSquare != null) {
							levelData [toggleSquare.triggerSquare.GridPosition.y] [toggleSquare.triggerSquare.GridPosition.x] = toggleChar;
						}
					}
				}
				else if (m_grid[x, y] is TriggeredPlatformSquare) {
					if (levelData [y] [x] == 0) {

						char triggerChar = 'T';
						char toggleChar = 't';

						levelData [y] [x] = toggleChar;
						TriggeredPlatformSquare square = m_grid [x, y] as TriggeredPlatformSquare;
						if (square.toggleSquare != null) {
							levelData [square.toggleSquare.GridPosition.y] [square.toggleSquare.GridPosition.x] = triggerChar;
						}
					}
				}
				else {
					Debug.Log (string.Format ("Unknown grid square type '{0}' at ({1}, {2})", m_grid[x, y].GetType(), x, y));
				}
			}
		}
		level.levelGrid = levelData;

		return level;
	}
}
