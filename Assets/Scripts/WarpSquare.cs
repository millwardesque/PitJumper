using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpSquare : PlatformSquare {
	public WarpSquare destination;
	public bool canUse = true;

	public override bool CanPlayerLandHereNow() {
		return true;
	}

	public override bool IsLandableSquare() {
		return true;
	}

	public override void OnPlayerLandsHere(Player player) {
		if (destination != null && canUse) {
			player.MoveToCoord (destination.GridPosition, false);
		}
	}

	public override void OnAddToLevel(LevelGrid grid, GridCoord position) {
		// @TODO Search for unlinked trigger and link.
	}

	public override void OnRemoveFromLevel(LevelGrid grid, GridCoord position) {
		// Unlink from other tile.
		if (destination != null) {
			destination.destination = null;
		}
	}
}
