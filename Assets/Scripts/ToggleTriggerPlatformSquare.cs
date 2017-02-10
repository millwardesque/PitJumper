using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTriggerPlatformSquare : PlatformSquare {
	public TriggeredPlatformSquare triggerSquare;

	public override bool CanPlayerLandHereNow() {
		return true;
	}

	public override bool IsLandableSquare() {
		return true;
	}

	public override void OnPlayerLandsHere(Player player) {
		if (triggerSquare != null) {
			triggerSquare.Trigger ();
		}
	}
}