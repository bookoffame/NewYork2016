using UnityEngine;
using System.Collections;
using AssemblyCSharp;

/// <summary>
/// The movement logic for the book mode camera.
/// </summary>
public class Move : MonoBehaviour {
	/// <summary>
	/// The position of the book mode camera
	/// </summary>
	public Transform myTransform;

	/// <summary>
	/// The speed of the book mode camera.
	/// </summary>
	public float speed;

	private bool on;

	void Update () {
		if (on) {
			myTransform.Translate (speed * Input.GetAxis ("Horizontal"), speed * Input.GetAxis ("Vertical"), speed * Input.GetAxis ("Forward"));
		}
	}

	/// <summary>
	/// Enable/Disable movement.
	/// </summary>
	/// <param name="isActivated">If set to <c>true</c> enables movement. Else, disables movement.</param>
	public void setActivated(bool isActivated){
		on = isActivated;
	}
}
