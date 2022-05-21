using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#nullable enable

public class LockOn : MonoBehaviour {

	public Animator? anim;
	public EyeEnemyController? controller;
	public AudioSource? sfx;

	public float lockOnTime = 3f;

	float startLockOn = 0;

	private void OnEnable() {
		startLockOn = Time.time;
		initialEyeRot = controller?.eye?.localRotation ?? Quaternion.identity;
		if (sfx != null) {
			sfx.enabled = true;
			sfx.Play();
		}
	}

	Quaternion initialEyeRot;
	private void Update() {

		controller?.LookAtPlayer(Time.deltaTime);
		if (Time.time - startLockOn > lockOnTime) anim!.SetBool("LockedOn", true);
	}

	private void OnDisable() {
		if (sfx != null) {
			sfx.enabled = false;
		}
	}

}
