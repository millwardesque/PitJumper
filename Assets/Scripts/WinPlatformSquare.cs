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
}
