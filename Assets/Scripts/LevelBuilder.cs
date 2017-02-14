using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {

	LevelGrid m_grid;
	public EmptyPlatformSquare emptyPlatformSquarePrefab;
	public SolidPlatformSquare solidPlatformSquarePrefab;
	public WinPlatformSquare winPlatformSquarePrefab;
	public ToggleTriggerPlatformSquare toggleTriggerPlatformSquare;
	public TriggeredPlatformSquare triggeredPlatformSquare;

	List<PlatformSquare> m_squareTypes;
	List<PlatformSquareData> m_squareData;

	// Use this for initialization
	void Start () {
		m_grid = FindObjectOfType<LevelGrid> ();
		if (!m_grid) {
			Debug.LogError ("Unable to start LevelBuilder properly: Couldn't find LevelGrid instance in the scene.");
		}

		m_squareTypes = new List<PlatformSquare> ();
		m_squareTypes.Add (emptyPlatformSquarePrefab);
		m_squareTypes.Add (solidPlatformSquarePrefab);
		m_squareTypes.Add (winPlatformSquarePrefab);
		m_squareTypes.Add (toggleTriggerPlatformSquare);
		m_squareTypes.Add (triggeredPlatformSquare);

		PlatformSquareData emptySquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Empty Square Prototype");
		PlatformSquareData solidPlatformData = Resources.Load<PlatformSquareData> ("Platform Squares/Solid Platform Prototype");
		PlatformSquareData winSquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Win Platform Prototype");
		PlatformSquareData toggleTriggerSquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Toggle Trigger Prototype");
		PlatformSquareData triggeredSquareData = Resources.Load<PlatformSquareData> ("Platform Squares/Triggered Platform Prototype");

		m_squareData = new List<PlatformSquareData> ();
		m_squareData.Add (emptySquareData);
		m_squareData.Add (solidPlatformData);
		m_squareData.Add (winSquareData);
		m_squareData.Add (toggleTriggerSquareData);
		m_squareData.Add (triggeredSquareData);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit.collider != null) {
				PlatformSquare square = hit.collider.GetComponent<PlatformSquare> ();
				if (square != null) {
					int typeIndex = FindTypeIndex (square);
					if (typeIndex == -1) {
						Debug.Log ("Couldn't find matching prefab for clicked on square '" + square.name + "' with type '" + square.GetType ());
					} else {
						int nextIndex = (typeIndex + 1 < m_squareTypes.Count ? typeIndex + 1 : 0);
						m_grid.ReplaceSquare (m_squareTypes [nextIndex], m_squareData [nextIndex], square.GridPosition.x, square.GridPosition.y);

						Debug.Log (m_grid.AsLevelDefinition ().ToString ());
					}
				}
			}
		}
	}

	int FindTypeIndex(PlatformSquare square) {
		for (int i = 0; i < m_squareTypes.Count; ++i) {
			if (square.GetType() == m_squareTypes[i].GetType()) {
				return i;
			}
		}

		return -1;
	}
}
