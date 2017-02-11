﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class LevelDefinition { 
	public char[][] levelGrid;

	public LevelDefinition(char[][] levelGrid) {
		this.levelGrid = levelGrid;
	}
}

public class LevelManager : MonoBehaviour {
	public Player playerPrefab;
	public EmptyPlatformSquare emptyPlatformSquarePrefab;
	public SolidPlatformSquare solidPlatformSquarePrefab;
	public WinPlatformSquare winPlatformSquarePrefab;
	public ToggleTriggerPlatformSquare toggleTriggerPlatformSquare;
	public TriggeredPlatformSquare triggeredPlatformSquare;

	public string levelFilename;

	int m_activeLevel = -1;
	List<LevelDefinition> m_levels;
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

		SetLevel (0);
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
			m_player.CurrentPosition = newPosition;
			m_grid.Grid [newPosition.x, newPosition.y].OnPlayerLandsHere (m_player);

			if (m_grid.Grid [newPosition.x, newPosition.y] is WinPlatformSquare) {
				Debug.Log ("You Win!");
				if (m_activeLevel == m_levels.Count - 1) {
					Debug.Log ("Wow, you beat the whole game!");
					SetLevel (0);
				} else {
					SetLevel (m_activeLevel + 1);
				}
			}
			else if (!m_grid.Grid [newPosition.x, newPosition.y].CanPlayerLandHereNow ()) {
				Debug.Log ("You Died!");
				SetLevel (m_activeLevel);
			}
		}

		if (Input.GetKeyDown (KeyCode.LeftBracket)) {
			SetLevel (m_activeLevel - 1);
		}
		else if (Input.GetKeyDown (KeyCode.RightBracket)) {
			SetLevel (m_activeLevel + 1);
		}
	}

	void LoadLevels(string levelFilename) {
		Debug.Log ("Loading levels from '" + levelFilename + "'");

		List<char[]> levelData = new List<char[]>();

		Regex endLevelPattern = new Regex(@"^###");
		Regex commentPattern = new Regex(@"^#");
		TextAsset levelText = Resources.Load<TextAsset> (levelFilename);
		if (!levelText) {
			Debug.LogError ("Error: Unable to load level from '" + levelFilename + "': Resource doesn't exist");
		}
		string rawLevelString = levelText.text;
		string[] rows = rawLevelString.Split(new string[]{"\n"}, System.StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < rows.Length; ++i) {
			string row = rows [i];
			if (endLevelPattern.IsMatch (row)) {
				m_levels.Add (new LevelDefinition (levelData.ToArray ()));
				levelData = new List<char[]> ();
			}
			else if (commentPattern.IsMatch (row)) {
				// Do nothing.
			} else {
				char[] levelRow = row.ToCharArray();
				levelData.Insert(0, levelRow);
			}
		}

		Debug.Log ("Loaded " + m_levels.Count + " levels");
	}

	void SetLevel(int levelIndex) {
		if (levelIndex >= 0 && m_levels.Count > levelIndex) {
			m_activeLevel = levelIndex;
			m_grid.InitializeGrid (m_levels[levelIndex].levelGrid, emptyPlatformSquarePrefab, solidPlatformSquarePrefab, winPlatformSquarePrefab, toggleTriggerPlatformSquare, triggeredPlatformSquare, m_player);
			Debug.Log ("Loaded level " + levelIndex);
		}
	}
}
