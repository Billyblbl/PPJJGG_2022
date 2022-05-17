using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif

using UnityEngine.AI;

#nullable enable

public class Patrol : MonoBehaviour {

	public NavMeshAgent?	agent;
	public Vector3[] points = new Vector3[0];
	public bool cycle = true;
	int current = 0;

	public bool atDestination { get =>
		!agent!.pathPending &&
		agent.remainingDistance <= agent.stoppingDistance &&
		(!agent.hasPath || agent.velocity.sqrMagnitude <= float.Epsilon);
	}

	private void OnEnable() {
		agent!.destination = points[current];
	}

	private void OnDisable() {
		agent!.destination = transform.position;
	}

	bool reverse = false;
	int nextIndex { get {
		if (points.Length <= 1) return current;

		if (cycle) {
			return current+1 < points.Length ? current+1 : 0;
		} else {
			var start = reverse ? points.Length-1 : 0;
			var end = reverse ? -1 : points.Length;
			var delta = (int)Mathf.Sign(end - start);

			if (current + delta == end) {
				reverse = !reverse;
				return current - delta;
			} else return current + delta;
		}
	}}

	private void FixedUpdate() {
		if (points.Length > 1 && atDestination) agent!.destination = points[current = nextIndex];
	}

}


#if UNITY_EDITOR

[CustomEditor(typeof(Patrol))] public class PatrolEditor : Editor {
	ArcHandle arcHandle = new();
	private void OnSceneGUI() {

		var linesSize = 5f;

		EditorGUI.BeginChangeCheck();
		var t = (target as Patrol)!;

		for (int i = 0; i < t.points.Length; i++) if (i == 0) {
			if (t.cycle) Handles.DrawDottedLine(t.points[0], t.points[t.points.Length-1], linesSize);
		} else {
			Handles.DrawDottedLine(t.points[i-1], t.points[i], linesSize);
		}

		// this is disgusting
		Vector3[] newPoints = new Vector3[t.points.Length];
		System.Array.Copy(t.points, newPoints, t.points.Length);
		foreach (var (point, i) in t.points.Select((p,i) => (p,i))) {
			newPoints[i] = Handles.PositionHandle(point, Quaternion.identity);
		}
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(t, "Update points");
			t.points = newPoints;
		}
	}

}

#endif
