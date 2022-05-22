using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

	private void Start() {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	public void OnQuit() {
		Application.Quit();
	}
}
