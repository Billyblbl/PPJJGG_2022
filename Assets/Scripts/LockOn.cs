using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#nullable enable

public class LockOn : MonoBehaviour {

	public Animator? anim;
	public EyeEnemyController? controller;
	public AudioSource? source;
	public AudioClip? sfx;

	public float lockOnTime = 3f;

	float startLockOn = 0;

	private void OnEnable() {
		startLockOn = Time.time;
		initialEyeRot = controller?.eye?.localRotation ?? Quaternion.identity;
		if (source != null) {
			source.enabled = true;
			source.clip = sfx;
			source.Play();
		}
	}

	Quaternion initialEyeRot;
	private void Update() {

		controller?.LookAtPlayer(Time.deltaTime);
		if (Time.time - startLockOn > lockOnTime) anim!.SetBool("LockedOn", true);
	}

	private void OnDisable() {
		if (source != null) {
			source.enabled = false;
		}
	}

}
