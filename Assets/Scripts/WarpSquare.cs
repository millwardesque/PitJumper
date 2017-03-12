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
        // Try to link to an unused warp.
        WarpSquare unusedWarp = FindUnusedWarpSquare(grid);
        if (unusedWarp != null) {
            destination = unusedWarp;
            destination.destination = this;
        }
    }

	public override void OnRemoveFromLevel(LevelGrid grid, GridCoord position) {
		// Unlink from other tile.
		if (destination != null) {
			destination.destination = null;
		}
	}

    WarpSquare FindUnusedWarpSquare(LevelGrid grid) {
        for (int y = 0; y < grid.Grid.GetLength(1); ++y) {
            for (int x = 0; x < grid.Grid.GetLength(0); ++x) {
                // Don't include myself.
                if (x == GridPosition.x && y == GridPosition.y) {
                    continue;
                }

                if (grid.Grid[x, y] is WarpSquare && (grid.Grid[x, y] as WarpSquare).destination == null) {
                    return grid.Grid[x, y] as WarpSquare;
                }
            }
        }

        return null;
    }
}
