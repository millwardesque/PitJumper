using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTriggerPlatformSquare : PlatformSquare {
	public TriggeredPlatformSquare triggerSquare;

	public bool oneWayToggle = false;
	bool m_canToggle = true;

	public override bool CanPlayerLandHereNow() {
		return true;
	}

	public override bool IsLandableSquare() {
		return true;
	}

	public override void OnPlayerLandsHere(Player player) {
		if (triggerSquare != null && m_canToggle) {
			triggerSquare.Trigger ();

			if (oneWayToggle) {
				m_canToggle = false;
			}
		}
	}

	public override void OnAddToLevel(LevelGrid grid, GridCoord position) {
		// Try to link to an unused trigger.
		TriggeredPlatformSquare trigger = FindUnusedTriggeredPlatformSquare(grid);
		if (trigger != null) {
			triggerSquare = trigger;
			triggerSquare.toggleSquare = this;
		}
	}

	public override void OnRemoveFromLevel(LevelGrid grid, GridCoord position) {
		// Unlink from other tile.
		if (triggerSquare != null) {
			triggerSquare.toggleSquare = null;
		}
	}

	TriggeredPlatformSquare FindUnusedTriggeredPlatformSquare(LevelGrid grid) {
		for (int y = 0; y < grid.Grid.GetLength (1); ++y) {
			for (int x = 0; x < grid.Grid.GetLength (0); ++x) {
				if (grid.Grid [x, y] is TriggeredPlatformSquare && (grid.Grid [x, y] as TriggeredPlatformSquare).toggleSquare == null) {
					return grid.Grid [x, y] as TriggeredPlatformSquare;
				}
			}
		}

		return null;
	}
}