using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinPlatformSquare : PlatformSquare {
	public override bool CanPlayerLandHereNow() {
		return true;
	}

	public override bool IsLandableSquare() {
		return true;
	}

	public override void OnPlayerLandsHere(Player player) {
		Debug.Log ("You Win!");

		LevelManager levelManager = FindObjectOfType <LevelManager> ();
		if (levelManager != null) {
			if (levelManager.ActiveLevel == levelManager.Levels.Count - 1) {
				Debug.Log ("Wow, you beat the whole game!");
				levelManager.ActiveLevel = 0;
			} else {
				levelManager.ActiveLevel++;
			}
		}
	}

	public override string GetPlatformTypeString () {
		return "e";
	}

	public override string GetResourceName () {
		return "Win Platform";
	}
}
