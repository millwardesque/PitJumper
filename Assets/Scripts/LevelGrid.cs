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

	public static GridCoord EmptyCoord {
		get { return new GridCoord (-1, -1); }
	}

	public static bool operator ==(GridCoord c1, GridCoord c2) {
		return c1.x == c2.x && c1.y == c2.y;
	}

	public static bool operator !=(GridCoord c1, GridCoord c2) {
		return !(c1 == c2);
	}

	public override bool Equals(System.Object obj) {
		if (obj == null) {
			return false;
		}

		if (!(obj is GridCoord)) {
			return false;
		}
		GridCoord coord = (GridCoord)obj;
		return (this.x == coord.x && this.y == coord.y);
	}

	public bool Equals(GridCoord coord) {
		return (this.x == coord.x && this.y == coord.y);
	}

	public override int GetHashCode() {
		return x ^ y;
	}
}

public class LevelGrid : MonoBehaviour {
	PlatformSquare[,] m_grid;
	public Vector2 cellSize = Vector2.one;

	public PlatformSquare[,] Grid {
		get { return m_grid; }
	}

	public void InitializeGrid(string[][] levelData, PlatformSquare[] platformPrefabs, Player player) {
		int gridWidth = levelData[0].Length;
		int gridHeight = levelData.Length;

		// Load the available level grid prefabs and resources.
		Dictionary<string, PlatformSquare> platformMap = new Dictionary<string, PlatformSquare>();
		Dictionary<string, PlatformSquareData> platformData = new Dictionary<string, PlatformSquareData> ();
		foreach (PlatformSquare prefab in platformPrefabs) {
			platformMap [prefab.GetPlatformTypeString()] = prefab;
			platformData [prefab.GetPlatformTypeString()] = Resources.Load<PlatformSquareData> ("Platform Squares/" + prefab.GetResourceName() + " Prototype");

			if (!platformData.ContainsKey (prefab.GetPlatformTypeString()) || platformData [prefab.GetPlatformTypeString()] == null) {
				Debug.LogError ("Error: No platform resource data for platform type '" + prefab.GetPlatformTypeString() + "' was found (using resource prefix '" + prefab.GetResourceName() + "')");
			} else {
				Debug.Log("Loaded platform type '" + prefab.GetPlatformTypeString() + "'");
			}
		}

		// Clean up the old grid and create a new one.
		DeleteGrid ();
		m_grid = new PlatformSquare[gridWidth, gridHeight];

		GridCoord playerStart = GridCoord.EmptyCoord;
		Dictionary<string, ToggleTriggerPlatformSquare> triggers = new Dictionary<string, ToggleTriggerPlatformSquare> ();
		Dictionary<string, WarpSquare> warps = new Dictionary<string, WarpSquare> ();

		// Load the tiles.
		Regex elementPattern = new Regex(@"^\s*([a-zA-Z0-9\-]+)(\[(.+)\])?");
		for (int x = 0; x < gridWidth; ++x) {
			for (int y = 0; y < gridHeight; ++y) {
				Match match = elementPattern.Match (levelData [y] [x]);
				if (!match.Success) {
					continue;
				}

				// Find the correct tile.
				string tileType = match.Groups [1].Value;
				Dictionary<string, string> attributes = (match.Groups.Count == 4 ? ExtractTileAttributes(match.Groups [3].Value) : new Dictionary<string, string>());
				if (platformMap.ContainsKey(tileType) && platformMap.ContainsKey(tileType)) {
					ReplaceSquare (platformMap[tileType], platformData[tileType], x, y, attributes);
				}
				else {
					Debug.Log (string.Format ("Error loading grid square type '{0}' at ({1}, {2}): Either type is unknown or associated data wasn't found", tileType, x, y));
					Destroy (m_grid [x, y]);
					continue;
				}

				// Do any custom loading stuff here.
				if (m_grid [x, y] != null) {
					m_grid [x, y].InitializeFromStringAttributes (attributes);

					if (attributes.ContainsKey ("player") && attributes ["player"] == "1") {
						playerStart = new GridCoord (x, y);
					}

					if (m_grid [x, y] is WarpSquare && m_grid [x, y].GroupId != "") {
						warps [m_grid [x, y].GroupId] = m_grid [x, y] as WarpSquare;
					} else if (m_grid [x, y] is ToggleTriggerPlatformSquare && m_grid [x, y].GroupId != "") {
						triggers [m_grid [x, y].GroupId] = m_grid [x, y] as ToggleTriggerPlatformSquare;
					}
				} else {
					Debug.Log(string.Format("Error trying to process grid square type '{0}' at ({1}, {2}): Grid square is null", tileType, x, y));
				}
			}
		}

		// Attach the warp and trigger relationships.
		foreach (string key in triggers.Keys) {
			for (int x = 0; x < gridWidth; ++x) {
				for (int y = 0; y < gridHeight; ++y) {
					if (m_grid [x, y].GroupId == key && m_grid[x, y] is TriggeredPlatformSquare && m_grid [x, y] != triggers [key]) {
						triggers [key].triggerSquare = m_grid [x, y] as TriggeredPlatformSquare;
						(m_grid [x, y] as TriggeredPlatformSquare).toggleSquare = triggers [key];
					}
				}
			}

			if (triggers[key].triggerSquare == null) {
				Debug.Log ("Warning: Trigger square #" + key + " has no toggle square");
			}
		}

		foreach (string key in warps.Keys) {
			for (int x = 0; x < gridWidth; ++x) {
				for (int y = 0; y < gridHeight; ++y) {
					if (m_grid [x, y].GroupId == key && m_grid [x, y] != warps [key]) {
						warps [key].destination = m_grid [x, y];
					}
				}
			}

			if (warps[key].destination == null) {
				Debug.Log ("Warning: Warp square #" + key + " has no endpoint square");
			}
		}

		// Set the player-start position.
		if (playerStart != GridCoord.EmptyCoord) {
			player.Grid = this;
			player.TransportToCoord (playerStart);
		} else {
			Debug.LogError ("Error: Level has no player start square");
		}
	}

	public void ReplaceSquare(PlatformSquare prefab, PlatformSquareData squareData, int x, int y, Dictionary<string, string> attributes) {
		if (m_grid [x, y] != null) {
			m_grid [x, y].OnRemoveFromLevel (this, new GridCoord(x, y));
			Destroy (m_grid [x, y].gameObject);
		}

		m_grid [x, y] = Instantiate<PlatformSquare> (prefab, this.transform);
		m_grid [x, y].InitializeSquareData (squareData);
		m_grid [x, y].name = "Grid (" + x + ", " + y + ")";
		m_grid [x, y].Grid = this;
		m_grid [x, y].GridPosition = new GridCoord (x, y);
		m_grid [x, y].InitializeFromStringAttributes (attributes);

		m_grid [x, y].OnAddToLevel (this, new GridCoord (x, y));
	}

	public Vector2 GetCoordInWorldSpace(GridCoord coords) {
		return (Vector2)transform.TransformPoint (new Vector3 (coords.x * cellSize.x, coords.y * cellSize.y, 0f));
	}

	public bool IsValidGridPosition(GridCoord coords) {
		return (coords.x >= 0 && coords.x < m_grid.GetLength (0) && coords.y >= 0 && coords.y < m_grid.GetLength (1));
	}

	Dictionary<string, string> ExtractTileAttributes(string attributeString) {
		Dictionary<string, string> attributes = new Dictionary<string, string> ();
		if (attributeString == "") {
			return attributes;
		}
	
		string[] tileAttributes = attributeString.Split(',');
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
