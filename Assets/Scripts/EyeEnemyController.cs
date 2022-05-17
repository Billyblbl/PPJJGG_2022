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
	public float eyeRotationSpring = 0f;
	public float lostPlayerTime = 1f;
	float lastSeenPlayer = 0f;
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
			var saw = anim.GetBool("SeePlayer");
			var sees = CanSeePlayer;
			if (sees) lastSeenPlayer = Time.time;
			var seeStatus = sees || (saw && Time.time - lastSeenPlayer < lostPlayerTime);
			anim.SetBool("SeePlayer", seeStatus);
			anim.SetBool("LockedOn", anim.GetBool("LockedOn") && sees);
		}
	}

	public Quaternion LookAtPlayer(float dt) => eye!.rotation = Quaternion.Lerp(eye.rotation, Quaternion.LookRotation(player!.transform.position - eye.position, Vector3.up), dt * eyeRotationSpring);

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
