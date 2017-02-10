using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	LevelGrid m_grid;
	public LevelGrid Grid {
		get { return m_grid; }
		set {
			m_grid = value;
			SnapToGridPosition ();
		}			
	}

	GridCoord m_currentPosition;
	public GridCoord CurrentPosition {
		get { return m_currentPosition; }
		set {
			m_currentPosition = value;
			SnapToGridPosition ();
		}
	}

	void SnapToGridPosition() {
		if (m_grid) {
			transform.position = m_grid.GetCoordInWorldSpace (m_currentPosition);
		}
	}
}
