﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidPlatformSquare : PlatformSquare {
	public override bool CanPlayerLandHereNow() {
		return true;
	}

	public override bool IsLandableSquare() {
		return true;
	}

	public override string GetPlatformTypeString () {
		return "o";
	}

	public override string GetResourceName () {
		return "Solid Platform";
	}
}