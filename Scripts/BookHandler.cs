using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the opening and closing of the book.
/// </summary>
public class BookHandler : MonoBehaviour {
	/// <summary>
	/// The animator of the book.
	/// </summary>
	public Animator animator;

	/// <summary>
	/// The pages of the book.
	/// </summary>
	public GameObject pages;

	/// <summary>
	/// The Colliders for the front and back covers of the book.
	/// </summary>
	public Collider[] models;

	/// <summary>
	/// The Toolbar UI.
	/// </summary>
	public UIPopUp myUI;

	void Update () {
		RaycastHit hit;
		bool isHit = false;
		if (ButtonControls.current.getSelected() == ButtonControls.SELECTION_TOOL && myUI.IsShowing()) {
			foreach (Collider model in models) {
				if (!isHit && model.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000)) {
					if (Input.GetMouseButtonDown (0)) {
						animator.SetTrigger ("Grabbed");
					}
				}
			}
		}

		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("OpenState") &&
			animator.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.999) {
			pages.SetActive (true);
		}

		else if (animator.GetCurrentAnimatorStateInfo (0).IsName ("CloseState")) {
			pages.SetActive (false);
		}
	}
}
