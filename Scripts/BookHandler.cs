using UnityEngine;
using System.Collections;

public class BookHandler : MonoBehaviour {

	public Animator animator;
	public GameObject pages;
	public Collider[] models;
	public UIPopUp myUI;
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		bool isHit = false;
		if (ButtonControls.current.getSelected() == ButtonControls.SELECTION_TOOL && myUI.IsShowing()) {
			foreach (Collider model in models) {
				if (!isHit && model.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000)) {
					if (Input.GetMouseButtonDown(0))
						animator.SetTrigger ("Grabbed");
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
