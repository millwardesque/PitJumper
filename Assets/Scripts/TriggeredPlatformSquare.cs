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
		// @TODO Search for unlinked trigger and link.
	}

	public override void OnRemoveFromLevel(LevelGrid grid, GridCoord position) {
		// Unlink from other tile.
		if (toggleSquare != null) {
			toggleSquare.triggerSquare = null;
		}
	}
}
