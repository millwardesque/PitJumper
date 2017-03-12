using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	Stack<PlayerState> m_state;
	public PlayerState State {
		get { return m_state.Peek(); }
	}

	LevelGrid m_grid;
	public LevelGrid Grid {
		get { return m_grid; }
		set {
			m_grid = value;
			SnapToGridPosition ();
		}			
	}

	GridCoord m_currentPosition;
	public GridCoord CurrentPosition {
		get { return m_currentPosition; }
	}

	public float secondsPerSquareMovement = 0.25f;

	void Awake() {
		m_state = new Stack<PlayerState> ();
	}

	void Start() {
		Reset ();
	}

	void Update() {
        if (m_state != null) {
            m_state.Peek().Update();
        }		
	}

	void FixedUpdate() {
        if (m_state != null) {
            m_state.Peek().FixedUpdate();
        }
	}

	void SnapToGridPosition() {
		if (m_grid) {
			transform.position = m_grid.GetCoordInWorldSpace (m_currentPosition);
		}
	}

	public void PushState(PlayerState state) {
		if (m_state.Count > 0 && m_state.Peek() != null) {
			m_state.Peek().Exit ();
		}

		m_state.Push (state);

		if (m_state.Peek() != null) {
			m_state.Peek().Enter ();
		}
	}

	public void SetState(PlayerState state) {
		if (m_state.Count > 0 && m_state.Peek() != null) {
			m_state.Peek().Exit ();
			m_state.Pop ();
		}

		m_state.Push (state);

		if (m_state.Peek() != null) {
			m_state.Peek().Enter ();
		}
	}

	public PlayerState PopState() {
		PlayerState oldState;
		if (m_state.Count == 0) {
			Debug.LogWarning("Unable to pop Player state: Stack is already empty.");
			return null;
		}

		if (m_state.Peek() != null) {
			m_state.Peek().Exit ();
		}

		oldState = m_state.Pop ();
		if (m_state.Count == 0) {
			Debug.LogWarning("Player state stack is now empty.");
		}

		if (m_state.Count > 0 && m_state.Peek() != null) {
			m_state.Peek().Enter ();
		}

		return oldState;
	}

	public void TransportToCoord(GridCoord location) {
		m_currentPosition = location;
		SnapToGridPosition ();
	}

	public bool CanMove() {
		return m_state.Count > 0 && !(m_state.Peek () is MovingPlayerState);
	}

	public bool MoveToCoord(GridCoord location, bool animate) {
		if (CanMove ()) {
			Vector2 destination = m_grid.GetCoordInWorldSpace (location);

			if (animate) {
				PushState (new MovingPlayerState (this, destination)); 
			} else {
				transform.position = destination;
			}

			m_currentPosition = location;
			return true;
		} else {
			return false;
		}
	}

	public void Reset() {
		while (m_state.Count > 0) {
			m_state.Peek ().Exit ();
			m_state.Pop ();
		}

		m_state.Push (new IdlePlayerState(this));
		m_state.Peek ().Enter ();
	}
}
