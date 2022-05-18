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
	public Perception? perception;
	public float eyeRotationSpring = 0f;
	public float lostPlayerTime = 1f;
	public float attackRange = .2f;
	float lastSeenPlayer = 0f;
	[System.NonSerialized] public FPController?	player;

	private void Start() {
		player = FindObjectOfType<FPController>();
		if (eye == null) eye = transform;
	}

	private void FixedUpdate() {
		if (anim != null) {
			var saw = anim.GetBool("SeePlayer");
			var sees = player != null && (
				perception!.Perceived(player.transform, ~LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer))) ||
				player.perception!.Perceived(transform, ~LayerMask.GetMask(LayerMask.LayerToName(player.gameObject.layer)))
			);
			if (sees) lastSeenPlayer = Time.time;
			var seeStatus = sees || (saw && Time.time - lastSeenPlayer < lostPlayerTime);
			anim.SetBool("SeePlayer", seeStatus);
			anim.SetBool("LockedOn", anim.GetBool("LockedOn") && sees);
			anim.SetBool("PlayerInAttackRange", player != null && Vector3.Distance(player.transform.position, transform.position) < attackRange);
		}
	}

	public Quaternion LookAtPlayer(float dt) => eye!.rotation = Quaternion.Lerp(eye.rotation, Quaternion.LookRotation(player!.transform.position - eye.position, Vector3.up), dt * eyeRotationSpring);

}

#if UNITY_EDITOR
[CustomEditor(typeof(EyeEnemyController))] public class EyeEnemyControllerEditor : Editor {
	private void OnSceneGUI() {
		var t = (target as EyeEnemyController)!;

		EditorGUI.BeginChangeCheck();
		var newRange = Handles.RadiusHandle(t.transform.rotation, t.transform.position, t.attackRange);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(t, "Update attack range");
			t.attackRange = newRange;
		}
	}

}
#endif
