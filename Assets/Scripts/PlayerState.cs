using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState {
	protected Player m_player;

	public PlayerState(Player player) {
		m_player = player;
	}

	public virtual void Enter() { }
	public virtual void Update() { }
	public virtual void FixedUpdate() { }
	public virtual void Exit() { }
}

public class IdlePlayerState : PlayerState {
	public IdlePlayerState(Player player) : base(player) { }
}

public class MovingPlayerState : PlayerState {
	Vector2 m_start;
	Vector2 m_movement;
	float m_elapsed;
	float m_duration;

	Vector2 m_end;
	Vector2 End {
		get { return m_end; }
		set {
			m_start = m_player.transform.position;
			m_end = value;
			m_movement = m_end - m_start;
			m_elapsed = 0f;
		}
	}

	public MovingPlayerState(Player player, Vector2 destination) : base(player) {
		End = destination;
		m_duration = player.secondsPerSquareMovement * m_movement.magnitude;
	}

	public override void FixedUpdate() {
		m_elapsed += Time.deltaTime;
		m_elapsed = Mathf.Clamp (m_elapsed, 0, m_duration);

		Vector2 position = m_start + (m_elapsed / m_duration) * m_movement;
		m_player.transform.position = position;

		if (m_elapsed >= m_duration) {
			m_player.PopState ();
		}
	}
}
