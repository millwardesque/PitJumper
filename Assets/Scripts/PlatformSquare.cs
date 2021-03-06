﻿using System.Collections;
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

	string m_groupId;
	public string GroupId {
		get { return m_groupId; }
	}

	public void InitializeSquareData(PlatformSquareData data) {
		this.GetComponent<SpriteRenderer> ().sprite = data.sprite;
	}

	public abstract string GetPlatformTypeString ();
	public abstract bool CanPlayerLandHereNow();
	public abstract bool IsLandableSquare();

	public virtual void OnPlayerLandsHere(Player player) { }

	public virtual void OnAddToLevel(LevelGrid grid, GridCoord position) { }
	public virtual void OnRemoveFromLevel(LevelGrid grid, GridCoord position) { }

	public virtual void InitializeFromStringAttributes(Dictionary<string, string> attributes) {
		m_groupId = attributes.ContainsKey ("gid") ? attributes ["gid"].ToLower() : "";
	}

	public abstract string GetResourceName ();
}
