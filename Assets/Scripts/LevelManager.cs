using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
	public Player playerPrefab;
	public EmptyPlatformSquare emptyPlatformSquarePrefab;
	public SolidPlatformSquare solidPlatformSquarePrefab;
	public WinPlatformSquare winPlatformSquarePrefab;

	LevelGrid m_grid;
	Player m_player;

	// Use this for initialization
	void Start () {
		m_grid = FindObjectOfType<LevelGrid> ();
		LoadLevel ("Blah");
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
			} else if (m_grid.IsValidGridPosition (twoSquaresOver) && m_grid.Grid [twoSquaresOver.x, twoSquaresOver.y].IsLandableSquare ()) {
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
			} else if (m_grid.IsValidGridPosition (twoSquaresOver) && m_grid.Grid [twoSquaresOver.x, twoSquaresOver.y].IsLandableSquare ()) {
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
			} else if (m_grid.IsValidGridPosition (twoSquaresOver) && m_grid.Grid [twoSquaresOver.x, twoSquaresOver.y].IsLandableSquare ()) {
				newPosition = twoSquaresOver;
			}
			else if (m_grid.IsValidGridPosition (oneSquareOver)) {
				newPosition = oneSquareOver;
			}
		}

		if (newPosition != m_player.CurrentPosition && m_grid.IsValidGridPosition(newPosition)) {
			m_player.CurrentPosition = newPosition;

			if (m_grid.Grid [newPosition.x, newPosition.y] is WinPlatformSquare) {
				Debug.Log ("You Win!");
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			}
			else if (!m_grid.Grid [newPosition.x, newPosition.y].CanPlayerLandHereNow ()) {
				Debug.Log ("Game Over!");
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			}
		}
	}

	void LoadLevel(string levelResource) {
		Debug.Log ("Loading level '" + levelResource + "'");

		m_player = Instantiate<Player> (playerPrefab);
		m_player.name = "Player";

		string[,] levelString = new string[,] {
			{ "e", "-", "o", "-", "o", "-", "o", "-" },
			{ "-", "o", "-", "o", "-", "o", "-", "o" },
			{ "o", "-", "o", "-", "o", "-", "o", "o" },
			{ "-", "o", "-", "o", "-", "o", "-", "o" },
			{ "o", "-", "o", "-", "o", "-", "o", "-" },
			{ "-", "o", "-", "o", "-", "o", "-", "o" },
			{ "o", "-", "o", "-", "o", "-", "o", "-" },
			{ "-", "s", "-", "o", "-", "o", "-", "o" },
		};

		m_grid.InitializeGrid (levelString, emptyPlatformSquarePrefab, solidPlatformSquarePrefab, winPlatformSquarePrefab, m_player);
	}
}
