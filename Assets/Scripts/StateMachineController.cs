using UnityEngine;

#nullable enable

public class StateMachineController : MonoBehaviour {

	public Animator? anim;
	public string targetLayer = "Decisions";

	[System.Serializable] public struct State {
		public string name;
		public MonoBehaviour? behaviour;
	}

	public State[] states = new State[0];

	private void Awake() {
		foreach (var state in states) if (state.behaviour != null) state.behaviour.enabled = false;
	}

	int decisionLayer;

	void Disable(string reason) {
		Debug.LogFormat("{2}, disabling state machine controller for {0} {1}", gameObject.name, gameObject.GetInstanceID(), reason);
		enabled = false;
	}

	private void OnEnable() {
		if (anim == null){
			Disable("No animator to derive state machine from");
			return;
		}

		try {
			decisionLayer = anim.GetLayerIndex(targetLayer);
		} catch (System.Exception e) {
			Disable("Failed to target animator layer");
			Debug.LogException(e);
		}
	}

	private void Update() {
		var info = anim!.GetCurrentAnimatorStateInfo(decisionLayer);
		foreach (var state in states) if (state.behaviour != null) state.behaviour.enabled = info.IsName(state.name);
	}

}
