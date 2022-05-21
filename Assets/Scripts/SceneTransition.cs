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
	[HideInInspector] public int targetSceneIndex = -1;

	private void OnValidate() {
		var scene = SceneManager.GetSceneByName(targetScene);
		if (scene.IsValid()) {
			targetSceneIndex = scene.buildIndex;
		} else {
			targetSceneIndex = -1;
			Debug.LogWarningFormat("Scene transition for {0} on {1} doesn't target a valid scene, scene will not transition", targetScene, gameObject.name);
		}
	}

	float startTime;
	private void OnEnable() {
		if (targetSceneIndex < 0) {
			Debug.LogWarningFormat("Scene transition for {0} on {1} doesn't target a valid scene, scene will not transition", targetScene, gameObject.name);
			enabled = false;
		}
		startTime = Time.time;
	}

	private void Update() {
		if (fade != null) fade.color = gradient.Evaluate(Mathf.InverseLerp(startTime, startTime + fadeOutTime, Time.time));
		if (Time.time > startTime + fadeOutTime) SceneManager.LoadScene(targetSceneIndex);
	}

}
