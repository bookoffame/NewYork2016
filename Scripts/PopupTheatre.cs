using UnityEngine;
using System.Collections;

public class PopupTheatre : MonoBehaviour {
	public GameObject otherStuff;
	public GameObject theatre;
	public GameObject myCamera,light;
	public Move cameraControls;
	public Transform normalPos, popupPos, popupCameraPos;
	public Popup[] popups;
	private Vector3 oldCameraPos = Vector3.zero;
	private Quaternion oldCameraRot = Quaternion.identity;

	public void SetupPopupTheatre(){
		otherStuff.SetActive (false);
		light.SetActive (true);
		StartCoroutine(MyUtils.SmoothMove (transform, normalPos, popupPos, 5f));
		oldCameraPos = myCamera.transform.position;
		oldCameraRot = myCamera.transform.rotation;
		cameraControls.setActivated (false);
		StartCoroutine(MyUtils.SmoothMove (myCamera.transform, popupCameraPos, 5f));
		StartCoroutine(PopupSprites());
	}

	public void LeavePopupTheatre(){
		foreach (Popup p in popups)
			p.Reset ();
		light.SetActive (false);
		otherStuff.SetActive (true);
		theatre.SetActive (false);
		cameraControls.setActivated (true);
		StartCoroutine (MyUtils.SmoothMove (transform, popupPos, normalPos, 5f));
		StartCoroutine (MyUtils.SmoothMove (myCamera.transform, oldCameraPos, oldCameraRot, 5f));
	}

	private IEnumerator PopupSprites()
	{
		yield return new WaitForSeconds (5f);
		theatre.SetActive (true);
		foreach (Popup p in popups)
			p.PopupObject ();
	}
		
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.P))
			SetupPopupTheatre ();
		else if (Input.GetKeyDown(KeyCode.Q))
		    LeavePopupTheatre();
	}
}
