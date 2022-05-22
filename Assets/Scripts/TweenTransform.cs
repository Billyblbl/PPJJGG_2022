using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class TweenTransform : MonoBehaviour {

	public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public float time = 1f;
	[Range(0f, 1f)] public float current = 0f;

	public Transform? start;
	public Transform? end;

	public Space relativeTo = Space.World;

	float startTime;
	float endTime { get => startTime + time; }

	private void OnValidate() {
		if (start == null) start = transform;
		if (end != null) {
			UpdateTween();
		}
	}

	private void OnEnable() {
		var elapsed = Mathf.Lerp(0, time, current);
		startTime = Time.time - elapsed;
	}

	void UpdateTween() {
		var value = curve.Evaluate(current);
		if (relativeTo == Space.World) {
			transform.position = Vector3.Lerp(start!.position, end!.position, value);
			transform.rotation = Quaternion.Lerp(start!.rotation, end!.rotation, value);
			transform.localScale = Vector3.Lerp(start!.lossyScale, end!.lossyScale, value);
		} else {
			transform.localPosition = Vector3.Lerp(start!.localPosition, end!.localPosition, value);
			transform.localRotation = Quaternion.Lerp(start!.localRotation, end!.localRotation, value);
			transform.localScale = Vector3.Lerp(start!.localScale, end!.localScale, value);
		}
	}

	private void Update() {
		current = Mathf.InverseLerp(startTime, endTime, Time.time);
		UpdateTween();
	}


}
