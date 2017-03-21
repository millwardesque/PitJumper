using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpSquare : PlatformSquare {
	public PlatformSquare destination;
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

	public override string GetPlatformTypeString () {
		return "W";
	}

	public override void InitializeFromStringAttributes (Dictionary<string, string> attributes) {
		base.InitializeFromStringAttributes (attributes);
		if (GroupId == "") {
			Debug.LogWarning("Error loading warp square: No Group ID was supplied");
		}
	}

	public override string GetResourceName () {
		return "Warp Square";
	}
}
