using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

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
	public PlayableDirector? dir;
	public GameObject? prompt;
	public TextMeshProUGUI?	promptText;
	public Image? promptIcon;

	[Header("Movement")]
	public Vector3 movementPlaneNormal = Vector3.up;
	public Vector2 speeds = Vector2.one;
	public float stepHeight = .1f;
	public float stepSpring = 0.5f;
	public Vector3 stepFeet;

	[Header("Aim")]
	public float mouseAimSensitivity = 1f;
	public float stickAimSensitivity = 10f;
	public float minPitch = -179;
	public float maxPitch = 179;

	[Header("Interaction")]
	public float interactionRange = 1f;
	public Sprite? gamepadBinding;
	public Sprite? KBMouseBinding;

	[Header("Jumping")]
	public Vector3 groundingFeet;
	public float feetRadius = .5f;
	public float jumpForce = 10f;

	[Header("Peek a boo")]
	public float peekABooDuration = 4f;
	public float peekABooCooldown = 5f;
	float lastPeekABoo = 0f;


	public Vector3 InputToWorldMovement(Vector2 input) => (
		Vector3.ProjectOnPlane(POV!.right, movementPlaneNormal).normalized * input.x * speeds.x +
		Vector3.ProjectOnPlane(POV!.forward, movementPlaneNormal).normalized * input.y * speeds.y
	);

	public Vector3 conservedVelocity { get => Vector3.Project(rb!.velocity, movementPlaneNormal); }

	public bool grounded { get => Physics.OverlapSphere(transform.TransformPoint(groundingFeet), feetRadius, LayerMask.GetMask("Physical", "Default")).Length > 0; }

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
		if (input!.actions["HideEyes"].triggered && (lastPeekABoo < float.Epsilon || lastPeekABoo + peekABooDuration + peekABooCooldown < Time.time)) {
			perception!.enabled = false;
			lastPeekABoo = Time.time;
			//@Note fixed time animation, will need to change the animation if we want to tweak the peek a boo time
			// anim?.Play("PeekABoo_Player");
			dir?.Play();
		}

		if (perception!.enabled == false && lastPeekABoo + peekABooDuration < Time.time) perception!.enabled = true;

		UpdateInteractionPrompt(input!.actions["Interact"].triggered);
	}

	void UpdateStep() {
		if (grounded && Physics.OverlapSphere(transform.TransformPoint(stepFeet), stepHeight, LayerMask.GetMask("Physical", "Default")).Length > 0)
			transform.Translate(movementPlaneNormal.normalized * stepSpring * stepHeight, Space.Self);
	}

	private void FixedUpdate() {
		UpdateStep();
	}

	string promptTemplate = string.Empty;

	private void Start() {
		promptTemplate = promptText?.text ?? string.Empty;
	}

	void UpdateInteractionPrompt(bool interact) {
		if (POV == null || prompt == null || promptText == null || promptIcon == null) return;

		if (
			Physics.Raycast(POV.position, POV.forward, out var hit, interactionRange) &&
			hit.collider.TryGetComponent<Interaction>(out var interaction)
		) {
			promptText.text = string.Format(promptTemplate, interaction.prompt);
			promptIcon.sprite = input?.currentControlScheme == "Gamepad" ? gamepadBinding : KBMouseBinding;
			prompt.SetActive(true);
			if (interact)
				interaction.OnInteract?.Invoke();
		} else prompt.SetActive(false);
	}

}


#if UNITY_EDITOR

[CustomEditor(typeof(FPController))] public class FPControllerEditor : Editor {

	(Vector3, float) DrawSphereArea(Transform transform, Vector3 position, float radius) {
		var worldPos = transform.TransformPoint(position);
		return (
			transform.InverseTransformPoint(Handles.PositionHandle(worldPos, transform.rotation)),
			Handles.RadiusHandle(transform.rotation, worldPos, radius)
		);
	}

	private void OnSceneGUI() {
		var t = (target as FPController)!;

		{
			EditorGUI.BeginChangeCheck();
			var newSphere = DrawSphereArea(t.transform, t.groundingFeet, t.feetRadius);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(t, "Update grounding feet");
				(t.groundingFeet, t.feetRadius) = newSphere;
			}
		}

		{
			EditorGUI.BeginChangeCheck();
			var newSphere = DrawSphereArea(t.transform, t.stepFeet, t.stepHeight);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(t, "Update step feet");
				(t.stepFeet, t.stepHeight) = newSphere;
			}
		}

		if (t.POV != null) {
			EditorGUI.BeginChangeCheck();
			Handles.DrawLine(t.POV.position, t.POV.position + t.POV.forward * t.interactionRange);
			var newRange = Handles.ScaleValueHandle(t.interactionRange, t.POV.position + t.POV.forward * t.interactionRange, t.POV.rotation, 1f, Handles.SphereHandleCap, 1f);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(t, "Update interaction range");
				t.interactionRange = newRange;
			}
		}
	}

}

#endif
