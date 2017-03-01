using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyPlatformSquare : PlatformSquare {
	public override bool CanPlayerLandHereNow() {
		return false;
	}

	public override bool IsLandableSquare() {
		return false;
	}

	public override void OnPlayerLandsHere(Player player) {
		Debug.Log ("You Died!");

		LevelManager levelManager = FindObjectOfType <LevelManager> ();
		if (levelManager != null) {
			levelManager.ActiveLevel = levelManager.ActiveLevel; // Restart the level.
		}
	}
}
