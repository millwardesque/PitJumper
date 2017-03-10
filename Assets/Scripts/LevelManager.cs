using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class LevelDefinition { 
	public string[][] levelGrid;
	public string name;
	public Color backgroundColour;
	public Color ambientLightColour;
	public float playerLightSize;

    public LevelDefinition() { }

	public LevelDefinition(string[][] levelGrid) {
		this.levelGrid = levelGrid;
	}

	public override string ToString() {
		string level = "";
		for (int y = levelGrid.Length - 1; y >= 0; y--) {
			for (int x = 0; x < levelGrid [y].Length; x++) {
				level += levelGrid [y] [x] + (x < levelGrid[y].Length - 1 ? " " : "") ;
			}

			level += "\n";
		}
		return level;
	}
}

public class LevelManager : MonoBehaviour {
	public Player playerPrefab;
	public EmptyPlatformSquare emptyPlatformSquarePrefab;
	public SolidPlatformSquare solidPlatformSquarePrefab;
	public WinPlatformSquare winPlatformSquarePrefab;
	public ToggleTriggerPlatformSquare toggleTriggerPlatformSquare;
	public TriggeredPlatformSquare triggeredPlatformSquare;
	public DisappearingSquare disappearingSquare;
	public WarpSquare warpSquare;

	public string levelFilename;

	int m_activeLevel = -1;
	public int ActiveLevel {
		get { return m_activeLevel; }
		set {
			if (value >= 0 && m_levels.Count > value) {
				m_activeLevel = value;
				m_player.Reset ();
				m_grid.InitializeGrid (m_levels[value].levelGrid, emptyPlatformSquarePrefab, solidPlatformSquarePrefab, winPlatformSquarePrefab, toggleTriggerPlatformSquare, triggeredPlatformSquare, disappearingSquare, warpSquare, m_player);
				RenderSettings.ambientLight = m_levels [value].ambientLightColour;
				Camera.main.backgroundColor = m_levels [value].backgroundColour;
				m_player.GetComponentInChildren<Light> ().range = m_levels [value].playerLightSize;
			}
		}
	}

	List<LevelDefinition> m_levels;
	public List<LevelDefinition> Levels {
		get { return m_levels; }
	}

	LevelGrid m_grid;
	Player m_player;

	void Awake() {
		m_levels = new List<LevelDefinition> ();
	}

	// Use this for initialization
	void Start () {
		m_player = Instantiate<Player> (playerPrefab);
		m_player.name = "Player";

		m_grid = FindObjectOfType<LevelGrid> ();
		LoadLevels (levelFilename);

		ActiveLevel = 0;
	}
	
	// Update is called once per frame
	void Update () {
		GridCoord newPosition = m_player.CurrentPosition;
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			GridCoord oneSquareOver = new GridCoord (newPosition.x + 1, newPosition.y);
			GridCoord twoSquaresOver = new GridCoord (newPosition.x + 2, newPosition.y);

			if (m_grid.IsValidGridPosition (oneSquareOver) && m_grid.Grid [oneSquareOver.x, oneSquareOver.y].IsLandableSquare ()) {
				newPosition = oneSquareOver;
			} else if (m_grid.IsValidGridPosition (twoSquaresOver) && m_grid.Grid [twoSquaresOver.x, twoSquaresOver.y].IsLandableSquare ()) {
				newPosition = twoSquaresOver;
			}
			else if (m_grid.IsValidGridPosition (oneSquareOver)) {
				newPosition = oneSquareOver;
			}
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			GridCoord oneSquareOver = new GridCoord (newPosition.x - 1, newPosition.y);
			GridCoord twoSquaresOver = new GridCoord (newPosition.x - 2, newPosition.y);

			if (m_grid.IsValidGridPosition (oneSquareOver) && m_grid.Grid [oneSquareOver.x, oneSquareOver.y].IsLandableSquare ()) {
				newPosition = oneSquareOver;
			} else if (m_grid.IsValidGridPosition (twoSquaresOver)) {
				newPosition = twoSquaresOver;
			}
			else if (m_grid.IsValidGridPosition (oneSquareOver)) {
				newPosition = oneSquareOver;
			}
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			GridCoord oneSquareOver = new GridCoord (newPosition.x, newPosition.y + 1);
			GridCoord twoSquaresOver = new GridCoord (newPosition.x, newPosition.y + 2);

			if (m_grid.IsValidGridPosition (oneSquareOver) && m_grid.Grid [oneSquareOver.x, oneSquareOver.y].IsLandableSquare ()) {
				newPosition = oneSquareOver;
			} else if (m_grid.IsValidGridPosition (twoSquaresOver)) {
				newPosition = twoSquaresOver;
			}
			else if (m_grid.IsValidGridPosition (oneSquareOver)) {
				newPosition = oneSquareOver;
			}
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			GridCoord oneSquareOver = new GridCoord (newPosition.x, newPosition.y - 1);
			GridCoord twoSquaresOver = new GridCoord (newPosition.x, newPosition.y - 2);

			if (m_grid.IsValidGridPosition (oneSquareOver) && m_grid.Grid [oneSquareOver.x, oneSquareOver.y].IsLandableSquare ()) {
				newPosition = oneSquareOver;
			} else if (m_grid.IsValidGridPosition (twoSquaresOver)) {
				newPosition = twoSquaresOver;
			}
			else if (m_grid.IsValidGridPosition (oneSquareOver)) {
				newPosition = oneSquareOver;
			}
		}

		if (newPosition != m_player.CurrentPosition && m_grid.IsValidGridPosition(newPosition)) {
			m_player.MoveToCoord (newPosition, true);
		}

		if (!m_grid.Grid [m_player.CurrentPosition.x, m_player.CurrentPosition.y].CanPlayerLandHereNow ()) {
			Debug.Log ("You Died!");
			ActiveLevel = ActiveLevel; // Restart the level.
		}

		if (Input.GetKeyDown (KeyCode.LeftBracket)) {
			ActiveLevel--;
		}
		else if (Input.GetKeyDown (KeyCode.RightBracket)) {
			ActiveLevel++;
		}
	}

	void LoadLevels(string levelFilename) {
		Debug.Log ("Loading levels from '" + levelFilename + "'");
        m_levels = LevelReadWrite.ReadLevelDefinitions(levelFilename);
		Debug.Log ("Loaded " + m_levels.Count + " levels");
	}
}
