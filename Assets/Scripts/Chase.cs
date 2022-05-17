using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#nullable enable

public class Chase : MonoBehaviour {
	public NavMeshAgent?	agent;
	public EyeEnemyController? controller;

	private void Update() {
		controller?.LookAtPlayer(Time.deltaTime);
		agent!.destination = controller?.player?.transform.position ?? transform.position;
	}

	private void OnDisable() {
		agent!.destination = transform.position;
	}
}
