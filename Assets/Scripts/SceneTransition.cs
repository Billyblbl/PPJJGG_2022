using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#nullable enable

public class SceneTransition : MonoBehaviour {

	public Image? fade;
	public Gradient gradient = new();
	public float fadeOutTime;
	public string targetScene = string.Empty;

	float startTime;
	private void OnEnable() {
		startTime = Time.time;
	}

	private void Update() {
		if (fade != null) fade.color = gradient.Evaluate(Mathf.InverseLerp(startTime, startTime + fadeOutTime, Time.time));
		if (Time.time > startTime + fadeOutTime) SceneManager.LoadScene(targetScene);
	}

}
