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
}