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
}
