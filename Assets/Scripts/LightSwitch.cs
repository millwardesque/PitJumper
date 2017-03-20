using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public override void InitializeFromStringAttributes(Dictionary<string, string> attributes) {
		base.InitializeFromStringAttributes (attributes);

		oneWaySwitch = attributes.ContainsKey ("oneway") && attributes ["oneway"].ToLower () == "y";

		string colorString = attributes.ContainsKey ("ambient") ? attributes ["ambient"] : "";
		if (colorString != "") {
			string[] components = colorString.Split (';');
			if (components.Length >= 3) {
				float r = float.Parse (components [0]);
				float g = float.Parse (components [1]);
				float b = float.Parse (components [2]);
				ambient = new Color (r, g, b);
			} else {
				Debug.LogError ("LightSwitch color string '" + colorString + "' doesn't match 'r;g;b' format");
			}
		}
	}

	public override string PlatformTypeString () {
		return "L";
	}
}
