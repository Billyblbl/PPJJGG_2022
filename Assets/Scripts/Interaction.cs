using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

#nullable enable

public class Interaction : MonoBehaviour {

	public UnityEvent	OnInteract = new();
	public string prompt = string.Empty;
}
