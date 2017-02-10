using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlatformSquare : MonoBehaviour {
	LevelGrid m_grid;
	public LevelGrid Grid {
		get { return m_grid; }
		set {
			m_grid = value;
		}
	}

	GridCoord m_gridPosition;
	public GridCoord GridPosition {
		get { return m_gridPosition; }
		set {
			m_gridPosition = value;

			transform.localPosition = new Vector2 (m_gridPosition.x * m_grid.cellSize.x, m_gridPosition.y * m_grid.cellSize.y);
		}
	}

	public void InitializeSquareData(PlatformSquareData data) {
		this.GetComponent<SpriteRenderer> ().sprite = data.sprite;
	}

	public abstract bool CanPlayerLandHereNow();
	public abstract bool IsLandableSquare();
}
