using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour {

	public MovePlayer player;
	public Move book;
	public Camera playerCam;
	public Camera bookCam;
	public GameObject canvas;
	// Use this for initialization

	void Start () {
		playerCam.enabled = true;
		bookCam.enabled = false;
		player.ChangeControl (true);
		book.setActivated (false);
		canvas.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.B)) {
			playerCam.enabled = !playerCam.enabled;
			bookCam.enabled = !bookCam.enabled;
			player.ChangeControl (playerCam.enabled);
			book.setActivated (bookCam.enabled);
			canvas.SetActive (bookCam.enabled);
		}
	}
}
