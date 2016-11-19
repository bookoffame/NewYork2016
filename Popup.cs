using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour {
	public void PopupObject(){
		StartCoroutine (MyUtils.SmoothMove (transform, transform.position, Quaternion.Euler (0, 0, 0), 4f));
	}

	public void Reset(){
		transform.Rotate (-45f, 0f, 0f);
	}
}
