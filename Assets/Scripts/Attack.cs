using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

#nullable enable

public class Attack : MonoBehaviour {

	public CinemachineVirtualCamera?	vcam;
	public PlayableDirector? dir;

	private void OnEnable() {
		dir?.Play();
	}

	private void OnDisable() {
		dir?.Stop();
		vcam?.gameObject.SetActive(false);
	}

}
