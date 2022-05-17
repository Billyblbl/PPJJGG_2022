using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


#if UNITY_EDITOR
using UnityEditor;

#endif

#nullable enable

public class FPController : MonoBehaviour {

	[Header("Components")]
	public Rigidbody?	rb;
	public PlayerInput? input;
	public Transform?	POV;
	public Perception? perception;

	[Header("Movement")]
	public Vector3 movementPlaneNormal = Vector3.up;
	public Vector2 speeds = Vector2.one;

	[Header("Aim")]
	public float mouseAimSensitivity = 1f;
	public float stickAimSensitivity = 10f;
	public float minPitch = -179;
	public float maxPitch = 179;

	[Header("Jumping")]
	public Vector3 feet;
	public float feetRadius = .5f;
	public float jumpForce = 10f;

	public Vector3 InputToWorldMovement(Vector2 input) => (
		Vector3.ProjectOnPlane(POV!.right, movementPlaneNormal).normalized * input.x * speeds.x +
		Vector3.ProjectOnPlane(POV!.forward, movementPlaneNormal).normalized * input.y * speeds.y
	);

	public Vector3 conservedVelocity { get => Vector3.Project(rb!.velocity, movementPlaneNormal); }

	public bool grounded { get => Physics.OverlapSphere(transform.TransformPoint(feet), feetRadius, LayerMask.GetMask("Physical", "Default")).Length > 0; }

	public void UpdateAim(Vector2 aim, float dt) {
		transform.Rotate(Vector3.up * aim.x * dt);
		var angle = Vector3.SignedAngle(transform.forward, POV!.forward, transform.right);
		var desiredAngle = Mathf.Clamp(angle - aim.y * dt, minPitch, maxPitch);
		var diff = desiredAngle - angle;
		POV.Rotate(Vector3.right * diff, Space.Self);
	}

	public void UpdateMovement(Vector2 movement) => rb!.velocity = conservedVelocity + InputToWorldMovement(movement);

	private void Update() {
		UpdateAim(input!.actions["Aim"].ReadValue<Vector2>(), input.currentControlScheme == "Gamepad" ? Time.deltaTime * stickAimSensitivity : mouseAimSensitivity);
		UpdateMovement(input!.actions["Movement"].ReadValue<Vector2>());
		if (input!.actions["Jump"].triggered && grounded) rb!.velocity += movementPlaneNormal * jumpForce;
	}

}


#if UNITY_EDITOR

[CustomEditor(typeof(FPController))] public class FPControllerEditor : Editor {

	private void OnSceneGUI() {
		var t = (target as FPController)!;
		var feet = t.transform.TransformPoint(t.feet);
		t.feet = t.transform.InverseTransformPoint(Handles.PositionHandle(feet, t.transform.rotation));
		t.feetRadius = Handles.RadiusHandle(t.transform.rotation, feet, t.feetRadius);
	}

}

#endif
