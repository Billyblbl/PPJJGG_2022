using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#nullable enable

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTransition : MonoBehaviour {

	public const string NoScene = "None";

	public Image? fade;
	public Gradient gradient = new();
	public float fadeOutTime;
	[HideInInspector] public string targetScene = NoScene;

	float startTime;
	private void OnEnable() {
		if (targetScene == "None") enabled = false;
		startTime = Time.time;
	}

	private void Update() {
		if (fade != null) fade.color = gradient.Evaluate(Mathf.InverseLerp(startTime, startTime + fadeOutTime, Time.time));
		if (Time.time > startTime + fadeOutTime) SceneManager.LoadScene(targetScene);
	}

}



#if UNITY_EDITOR

[CustomEditor(typeof(SceneTransition))] [CanEditMultipleObjects] public class SceneTransitionEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		EditorGUI.BeginChangeCheck();
		var t = (target as SceneTransition)!;
		var buildScenes = new List<string> {SceneTransition.NoScene};
		buildScenes.AddRange(EditorBuildSettings.scenes.Select(scene => (scene.path)));
		var index = (int)Mathf.Clamp(buildScenes.IndexOf(t.targetScene), 0, float.MaxValue);
		var selected = EditorGUILayout.Popup("Target Scene", index, buildScenes.ToArray());
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(t, "Update target scene");
			t.targetScene = buildScenes[selected];
		}
	}

}


#endif
