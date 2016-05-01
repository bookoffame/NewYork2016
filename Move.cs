using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class Move : MonoBehaviour {
	public Transform myTransform;
	public float speed;
	private bool on;

	void Update () {
		if (on) {
			myTransform.Translate (speed * Input.GetAxis ("Horizontal"), speed * Input.GetAxis ("Vertical"), speed * Input.GetAxis ("Forward"));
		}
	}

	public void setActivated(bool isActivated){
		on = isActivated;
	}
}
