using UnityEngine;
using System.Collections;

public class PopupTheatre : MonoBehaviour {
	public GameObject otherStuff;
	public GameObject myCamera;
	public Transform normalPos, popupPos, popupCameraPos;
	private Vector3 oldCameraPos = Vector3.zero;
	private Quaternion oldCameraRot = Quaternion.identity;

	public void SetupPopupTheatre(){
		otherStuff.SetActive (false);
		StartCoroutine(MyUtils.SmoothMove (transform, normalPos, popupPos, 5f));
		oldCameraPos = camera.transform.position;
		oldCameraRot = camera.transform.rotation;
		StartCoroutine(MyUtils.SmoothMove (myCamera.transform, popupCameraPos, 5f));
	}

	public void LeavePopupTheatre(){
		otherStuff.SetActive (true);
		StartCoroutine (MyUtils.SmoothMove (transform, popupPos, normalPos, 5f));
		StartCoroutine (MyUtils.SmoothMove (myCamera.transform, oldCameraPos, oldCameraRot, 5f));
	}
		
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.P))
			SetupPopupTheatre ();
		else if (Input.GetKeyDown(KeyCode.Q))
		    LeavePopupTheatre();
	}
}
