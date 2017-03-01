﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

	public void InitializeGrid(string[][] levelData, EmptyPlatformSquare emptyPrefab, SolidPlatformSquare solidPrefab, WinPlatformSquare winPrefab, ToggleTriggerPlatformSquare toggleTriggerPlatformSquare, TriggeredPlatformSquare triggeredPlatformSquare, DisappearingSquare disappearingSquare, WarpSquare warpSquare, Player player) {
		int gridWidth = levelData[0].Length;
		int gridHeight = levelData.Length;

		PlatformSquareData emptySquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Empty Square Prototype");
		PlatformSquareData solidPlatformData = Resources.Load<PlatformSquareData> ("Platform Squares/Solid Platform Prototype");
		PlatformSquareData winSquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Win Platform Prototype");
		PlatformSquareData toggleTriggerSquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Toggle Trigger Prototype");
		PlatformSquareData triggeredSquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Triggered Platform Prototype");
		PlatformSquareData disappearingSquareData = Resources.Load<PlatformSquareData>("Platform Squares/Disappearing Square Prototype");
		PlatformSquareData warpSquareData = Resources.Load<PlatformSquareData>("Platform Squares/Warp Square Prototype");

		DeleteGrid ();

		Regex elementPattern = new Regex(@"^([a-zA-Z0-9\-]+)(\[(.+)\])?");

		GridCoord playerStart = new GridCoord(0, 0);
		m_grid = new PlatformSquare[gridWidth, gridHeight];

		Dictionary<string, ToggleTriggerPlatformSquare> toggleTriggerSquares = new Dictionary<string, ToggleTriggerPlatformSquare> ();
		Dictionary<string, TriggeredPlatformSquare> triggeredSquares = new Dictionary<string, TriggeredPlatformSquare> ();
		Dictionary<string, WarpSquare> warpSquares1 = new Dictionary<string, WarpSquare> ();
		Dictionary<string, WarpSquare> warpSquares2 = new Dictionary<string, WarpSquare> ();
		for (int x = 0; x < gridWidth; ++x) {
			for (int y = 0; y < gridHeight; ++y) {
				Match match = elementPattern.Match (levelData [y] [x]);
				if (!match.Success) {
					continue;
				}

				string tileType = match.Groups [1].Value;
				string tileAttributes = (match.Groups.Count == 4 ? match.Groups [3].Value : "");
				if (tileType == "e") {
					ReplaceSquare (winPrefab, winSquareData, x, y);
				} else if (tileType == "-") {
					ReplaceSquare (emptyPrefab, emptySquareData, x, y);
				} else if (tileType == "o") {
					ReplaceSquare (solidPrefab, solidPlatformData, x, y);
				} else if (tileType == "d") {
					ReplaceSquare (disappearingSquare, disappearingSquareData, x, y);
				} else if (tileType == "s") {
					playerStart = new GridCoord (x, y);
					ReplaceSquare (solidPrefab, solidPlatformData, x, y);
				} else if (tileType == "T") {
					ReplaceSquare (toggleTriggerPlatformSquare, toggleTriggerSquareData, x, y);
					toggleTriggerSquares[tileAttributes] = m_grid [x, y] as ToggleTriggerPlatformSquare;
				} else if (tileType == "t") {
					ReplaceSquare (triggeredPlatformSquare, triggeredSquareData, x, y);
					triggeredSquares[tileAttributes] = m_grid [x, y] as TriggeredPlatformSquare;
				} else if (tileType == "W") {
					ReplaceSquare (warpSquare, warpSquareData, x, y);
					warpSquares1[tileAttributes] = m_grid [x, y] as WarpSquare;
				} else if (tileType == "w") {
					ReplaceSquare (warpSquare, warpSquareData, x, y);
					warpSquares2[tileAttributes] = m_grid [x, y] as WarpSquare;
				}
				else {
					Debug.Log (string.Format ("Unknown grid square type '{0}' at ({1}, {2})", tileType, x, y));
					Destroy (m_grid [x, y]);
					continue;
				}
			}
		}

		foreach (string key in toggleTriggerSquares.Keys) {
			if (triggeredSquares[key] && toggleTriggerSquares[key]) {
				toggleTriggerSquares[key].triggerSquare = triggeredSquares[key];
				triggeredSquares[key].toggleSquare = toggleTriggerSquares[key];
			} else {
				Debug.Log ("Warning: Trigger square #" + key + " has no toggle square, or vice-versa");
			}
		}

		foreach (string key in warpSquares1.Keys) {
			if (warpSquares1[key] && warpSquares2[key]) {
				warpSquares1[key].destination = warpSquares2[key];
				warpSquares2[key].destination = warpSquares1[key];
			} else {
				Debug.Log ("Warning: Warp square #" + key + " has no endpoint square, or vice-versa");
			}
		}

		player.Grid = this;
		player.TransportToCoord(playerStart);
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
		string[][] levelData = new string[m_grid.GetLength (1)][];

		for (int y = 0; y < m_grid.GetLength(1); ++y) {
			levelData[y] = new string [m_grid.GetLength(0)];
			for (int x = 0; x < m_grid.GetLength(0); ++x) {
				if (m_grid[x, y] is EmptyPlatformSquare) {
					levelData [y] [x] = "-";
				}
				else if (m_grid[x, y] is SolidPlatformSquare) {
					levelData [y] [x] = "o";
				}
				else if (m_grid[x, y] is DisappearingSquare) {
					levelData [y] [x] = "d";
				}
				else if (m_grid[x, y] is WinPlatformSquare) {
					levelData [y] [x] = "e";
				}
				else if (m_grid[x, y] is ToggleTriggerPlatformSquare) {
					if (levelData [y] [x] == "") {

						string triggerString = "T";
						string toggleString = "t";
						levelData [y] [x] = triggerString;
						ToggleTriggerPlatformSquare toggleSquare = m_grid [x, y] as ToggleTriggerPlatformSquare;
						if (toggleSquare.triggerSquare != null) {
							levelData [toggleSquare.triggerSquare.GridPosition.y] [toggleSquare.triggerSquare.GridPosition.x] = toggleString;
						}
					}
				}
				else if (m_grid[x, y] is TriggeredPlatformSquare) {
					if (levelData [y] [x] == "") {

						string triggerString = "T";
						string toggleString = "t";

						levelData [y] [x] = toggleString;
						TriggeredPlatformSquare square = m_grid [x, y] as TriggeredPlatformSquare;
						if (square.toggleSquare != null) {
							levelData [square.toggleSquare.GridPosition.y] [square.toggleSquare.GridPosition.x] = triggerString;
						}
					}
				}
				else {
					Debug.Log (string.Format ("Unknown grid square type '{0}' at ({1}, {2})", m_grid[x, y].GetType(), x, y));
					continue;
				}
			}
		}
		level.levelGrid = levelData;

		return level;
	}
}
