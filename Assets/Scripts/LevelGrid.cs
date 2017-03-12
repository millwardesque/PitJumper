using System.Collections;
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

		Regex elementPattern = new Regex(@"^\s*([a-zA-Z0-9\-]+)(\[(.+)\])?");

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
				Dictionary<string, string> attributes = (match.Groups.Count == 4 ? ExtractTileAttributes(match.Groups [3].Value) : null);

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

					bool isOneWay = attributes.ContainsKey ("oneway") && attributes ["oneway"].ToLower () == "y";
					(m_grid [x, y] as ToggleTriggerPlatformSquare).oneWayToggle = isOneWay;

					string triggerID = attributes.ContainsKey("id") ? attributes["id"] : "";
					if (triggerID != "") {
						toggleTriggerSquares[triggerID] = m_grid [x, y] as ToggleTriggerPlatformSquare;
					}
					else {
						Debug.LogWarning("Error loading toggle-trigger square: No trigger ID was supplied");
					}
				} else if (tileType == "t") {
					ReplaceSquare (triggeredPlatformSquare, triggeredSquareData, x, y);

					string triggerID = attributes.ContainsKey("id") ? attributes["id"] : "";
					if (triggerID != "") {
						triggeredSquares[triggerID] = m_grid [x, y] as TriggeredPlatformSquare;
					}
					else {
						Debug.LogWarning("Error loading triggerable square: No trigger index was supplied");
					}

				} else if (tileType == "W") {
					ReplaceSquare (warpSquare, warpSquareData, x, y);

					string warpID = attributes.ContainsKey("id") ? attributes["id"] : "";
					if (warpID != "") {
						warpSquares1[warpID] = m_grid [x, y] as WarpSquare;
					}
					else {
						Debug.LogWarning("Error loading warp square: No warp ID was supplied");
					}
				} else if (tileType == "w") {
					ReplaceSquare (warpSquare, warpSquareData, x, y);

					string warpID = attributes.ContainsKey("id") ? attributes["id"] : "";
					if (warpID != "") {
						warpSquares2[warpID] = m_grid [x, y] as WarpSquare;
					}
					else {
						Debug.LogWarning("Error loading warp square: No warp ID was supplied");
					}
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
			m_grid [x, y].OnRemoveFromLevel (this, new GridCoord(x, y));
			Destroy (m_grid [x, y].gameObject);
		}

		m_grid [x, y] = Instantiate<PlatformSquare> (prefab, this.transform);
		m_grid [x, y].InitializeSquareData (squareData);
		m_grid [x, y].name = "Grid (" + x + ", " + y + ")";
		m_grid [x, y].Grid = this;
		m_grid [x, y].GridPosition = new GridCoord (x, y);

		m_grid [x, y].OnAddToLevel (this, new GridCoord (x, y));
	}

	public Vector2 GetCoordInWorldSpace(GridCoord coords) {
		return (Vector2)transform.TransformPoint (new Vector3 (coords.x * cellSize.x, coords.y * cellSize.y, 0f));
	}

	public bool IsValidGridPosition(GridCoord coords) {
		return (coords.x >= 0 && coords.x < m_grid.GetLength (0) && coords.y >= 0 && coords.y < m_grid.GetLength (1));
	}

	Dictionary<string, string> ExtractTileAttributes(string attributeString) {
		if (attributeString == "") {
			return null;
		}
	
		string[] tileAttributes = attributeString.Split(',');
		Dictionary<string, string> attributes = new Dictionary<string, string> ();

		foreach (string attr in tileAttributes) {
			string[] attrData = attr.Split ('=');
			if (attrData != null && attrData.Length == 2) {
				attributes [attrData [0]] = attrData [1];
			} else {
				Debug.LogWarning ("Unsupported string attribute: '" + attr + "' from string '" + attributeString + "'");
			}
		}

		return attributes;
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
            levelData[y] = new string[m_grid.GetLength(0)];
        }

		int triggerID = 1;
        int warpID = 1;
		for (int y = 0; y < m_grid.GetLength(1); ++y) {
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
					if (string.IsNullOrEmpty(levelData[y][x])) {
						string triggerAttributes = "id=" + triggerID + ",oneway=" + ((m_grid [x, y] as ToggleTriggerPlatformSquare).oneWayToggle ? "y" : "n");
						string triggerString = "T[" + triggerAttributes + "]";
						levelData [y] [x] = triggerString;
						ToggleTriggerPlatformSquare toggleSquare = m_grid [x, y] as ToggleTriggerPlatformSquare;
						if (toggleSquare.triggerSquare != null && string.IsNullOrEmpty(levelData [toggleSquare.triggerSquare.GridPosition.y] [toggleSquare.triggerSquare.GridPosition.x])) {
							string toggleString = "t[id=" + triggerID + "]";
							levelData [toggleSquare.triggerSquare.GridPosition.y] [toggleSquare.triggerSquare.GridPosition.x] = toggleString;
						}
						triggerID++;
					}
				}
				else if (m_grid[x, y] is TriggeredPlatformSquare) {
					if (string.IsNullOrEmpty(levelData[y][x])) {
						string toggleString = "t[id=" + triggerID + "]";
						levelData [y] [x] = toggleString;
						TriggeredPlatformSquare square = m_grid [x, y] as TriggeredPlatformSquare;
						if (square.toggleSquare != null && string.IsNullOrEmpty(levelData [square.toggleSquare.GridPosition.y] [square.toggleSquare.GridPosition.x])) {
							string triggerAttributes = "id=" + triggerID + ",oneway=" + ((m_grid [square.toggleSquare.GridPosition.x, square.toggleSquare.GridPosition.y] as ToggleTriggerPlatformSquare).oneWayToggle ? "y" : "n");
							string triggerString = "T[" + triggerAttributes + "]";
							levelData [square.toggleSquare.GridPosition.y] [square.toggleSquare.GridPosition.x] = triggerString;
						}
						triggerID++;
					}
				}
                else if (m_grid[x, y] is WarpSquare) {
                    if (string.IsNullOrEmpty(levelData[y][x])) {
                        string attributes = "id=" + warpID;
                        string warpString = "W[" + attributes + "]";
                        levelData[y][x] = warpString;
                        WarpSquare warpSquare = m_grid[x, y] as WarpSquare;
                        if (warpSquare.destination != null && string.IsNullOrEmpty(levelData[warpSquare.destination.GridPosition.y][warpSquare.destination.GridPosition.x])) {
                            string destinationString = "w[id=" + warpID + "]";
                            levelData[warpSquare.destination.GridPosition.y][warpSquare.destination.GridPosition.x] = destinationString;
                        }
                        warpID++;
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
