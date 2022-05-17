using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#nullable enable

public class Chase : MonoBehaviour {
	public NavMeshAgent?	agent;
	public EyeEnemyController? controller;

	private void Update() {
		controller?.eye?.LookAt(controller!.player!.transform, Vector3.up);
		agent!.destination = controller?.player?.transform.position ?? transform.position;
	}

	private void OnDisable() {
		agent!.destination = transform.position;
	}
}
