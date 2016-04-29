using UnityEngine;
using System.Collections;

public class BookHandler : MonoBehaviour {

	public Animator animator;
	public Collider model;


	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if (model.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000)) {
			if (Input.GetMouseButton(0))
				animator.SetTrigger ("Grabbed");
			animator.SetFloat ("HandPosition", hit.textureCoord.x);
		} 
		else {
			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("StartState")) {
				animator.SetFloat ("HandPosition", 0);
			} else {
				animator.SetFloat ("HandPosition", 1);
			}
		}


	}
}
