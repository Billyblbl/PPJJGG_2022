using UnityEngine;
using System.Linq;

#nullable enable

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif

public class Perception : MonoBehaviour {
	public Cone[] perception = new Cone[0];
	public bool Perceived(Transform obj, int layerMask) => enabled && perception.Any(cone => cone.Contains(transform, obj.position)) && (
		!Physics.Raycast(
			transform.position,
			obj.transform.position - transform.position,
			out var hit,
			(obj.transform.position - transform.position).magnitude,
			layerMask
		) || hit.collider.gameObject == obj.gameObject
	);

	private void Update() {}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Perception))] public class PerceptionEditor : Editor {
	ArcHandle arcHandle = new();
	private void OnSceneGUI() {
		var t = (target as Perception)!;
		foreach (var cone in t.perception) cone.DrawHandle(arcHandle, t.transform);
	}

}
#endif
