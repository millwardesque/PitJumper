using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredPlatformSquare : PlatformSquare {
	bool m_isTriggered = false;
	public ToggleTriggerPlatformSquare toggleSquare = null;

	void Start() {
		GetComponent<SpriteRenderer> ().enabled = false;
	}

	public override bool CanPlayerLandHereNow() {
		return m_isTriggered;
	}

	public override bool IsLandableSquare() {
		return m_isTriggered;
	}

	public void Trigger() {
		if (!m_isTriggered) {
			m_isTriggered = true;
			GetComponent<SpriteRenderer> ().enabled = true;
		} else {
			m_isTriggered = false;
			GetComponent<SpriteRenderer> ().enabled = false;
		}
	}

	public override void OnAddToLevel(LevelGrid grid, GridCoord position) {
        // Try to link to an unused toggle.
        ToggleTriggerPlatformSquare toggle = FindUnusedToggleTriggerPlatformSquare(grid);
        if (toggle != null) {
            toggleSquare = toggle;
            toggleSquare.triggerSquare = this;
        }
    }

	public override void OnRemoveFromLevel(LevelGrid grid, GridCoord position) {
		// Unlink from other tile.
		if (toggleSquare != null) {
			toggleSquare.triggerSquare = null;
		}
	}

    ToggleTriggerPlatformSquare FindUnusedToggleTriggerPlatformSquare(LevelGrid grid) {
        for (int y = 0; y < grid.Grid.GetLength(1); ++y) {
            for (int x = 0; x < grid.Grid.GetLength(0); ++x) {
                if (grid.Grid[x, y] is ToggleTriggerPlatformSquare && (grid.Grid[x, y] as ToggleTriggerPlatformSquare).triggerSquare == null) {
                    return grid.Grid[x, y] as ToggleTriggerPlatformSquare;
                }
            }
        }

        return null;
    }
}
