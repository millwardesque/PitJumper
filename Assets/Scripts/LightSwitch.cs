using UnityEngine;
using System.Collections;

public class LightSwitch : PlatformSquare
{
	public bool oneWaySwitch = false;
	bool m_canSwitch = true;
	public Color ambient;

	public override bool CanPlayerLandHereNow() {
		return true;
	}

	public override bool IsLandableSquare() {
		return true;
	}

	public override void OnPlayerLandsHere(Player player) {
		if (m_canSwitch) {
			RenderSettings.ambientLight = ambient;

			if (oneWaySwitch) {
				m_canSwitch = false;
			}
		}
	}
}

