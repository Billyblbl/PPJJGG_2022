using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;

#endif

#nullable enable

public class EyeEnemyController : MonoBehaviour {

	public NavMeshAgent?	agent;
	public Cone viewPerception = new();
	FPController?	player;
	private void Start() {
		player = FindObjectOfType<FPController>();
	}

	public bool CanSeePlayer { get {
		if (player == null) return false;

		var playerInCone = viewPerception.Contains(transform, player.transform.position);
		if (!playerInCone) return false;
		// else Debug.Log("playerInCone");
		var raycastHits = Physics.Raycast(transform.position, player.transform.position - transform.position, out var hit, float.MaxValue, ~LayerMask.GetMask("Enemy"));
		if (!raycastHits) return false;
		// else Debug.Log("raycastHits");
		var colliderIsPlayer = hit.collider.gameObject == player.gameObject;
		if (!colliderIsPlayer) return false;
		// else Debug.Log("colliderIsPlayer");

		return true;
	} }

	private void FixedUpdate() {
		if (agent != null && CanSeePlayer) {
			agent.destination = player!.transform.position;
		}
	}

}



#if UNITY_EDITOR

[CustomEditor(typeof(EyeEnemyController))] public class EyeEnemyControllerEditor : Editor {
	ArcHandle arcHandle = new();

	private void OnSceneGUI() {
		var t = (target as EyeEnemyController)!;
		t.viewPerception.DrawHandle(arcHandle, t.transform);
	}

}

#endif
