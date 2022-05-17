using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;

#endif

#nullable enable

public class EyeEnemyController : MonoBehaviour {

	public NavMeshAgent?	agent;
	public Animator? anim;
	public Transform? eye;
	public Cone[] perception = new Cone[0];
	[System.NonSerialized] public FPController?	player;

	private void Start() {
		player = FindObjectOfType<FPController>();
		if (eye == null) eye = transform;
	}


	bool PlayerInPerceptionArea { get => player != null && perception.Any(cone => cone.Contains(eye, player.transform.position)); }

	bool LineOfSightToPlayer { get => player != null && Physics.Raycast(
		transform.position,
		player.transform.position - transform.position,
		out var hit,
		float.MaxValue,
		~LayerMask.GetMask("Enemy")
	) && hit.collider.gameObject == player.gameObject; }

	public bool CanSeePlayer { get => PlayerInPerceptionArea && LineOfSightToPlayer; }

	private void FixedUpdate() {
		if (anim != null) {
			anim.SetBool("SeePlayer", CanSeePlayer);
			anim.SetBool("LockedOn", anim.GetBool("LockedOn") && CanSeePlayer);
		}
	}

}

#if UNITY_EDITOR

[CustomEditor(typeof(EyeEnemyController))] public class EyeEnemyControllerEditor : Editor {
	ArcHandle arcHandle = new();
	private void OnSceneGUI() {
		var t = (target as EyeEnemyController)!;
		foreach (var cone in t.perception) cone.DrawHandle(arcHandle, t.eye);
	}

}

#endif
