using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingSquare : PlatformSquare {
	float m_cycleDuration = 2f;
	float m_currentCycle = 0f;

	SpriteRenderer m_renderer;

	void Awake() {
		m_renderer = GetComponent<SpriteRenderer> ();
	}

	public override bool CanPlayerLandHereNow() {
		return (m_currentCycle / m_cycleDuration) > 0.5f;
	}

	public override bool IsLandableSquare() {
		return true;
	}

	void Update() {
		m_currentCycle += Time.deltaTime;
		while (m_currentCycle > m_cycleDuration) {
			m_currentCycle -= m_cycleDuration;
		}

		float a = CanPlayerLandHereNow() ? 1f : 0f;
		Color color = new Color (m_renderer.color.r, m_renderer.color.b, m_renderer.color.g, a);
		m_renderer.color = color;
	}
}
