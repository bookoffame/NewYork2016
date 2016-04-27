using UnityEngine;
using System.Collections;

public class BookHandler : MonoBehaviour {

	public Animator animator;
	public Collider model;
	bool grabbing;
	float handPos;

	// Use this for initialization
	void Start () {
		grabbing = false;
		handPos = 0;
	}

	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if (model.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000)) {
			Debug.Log (hit.textureCoord.x.ToString ());
			animator.SetBool ("Grabbing", Input.GetMouseButton (0));
			animator.SetFloat ("HandPosition", hit.textureCoord.x);
		} 
		else {
			animator.SetBool ("Grabbing", false);
			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("StartState")) {
				animator.SetFloat ("HandPosition", 0);
			} else {
				animator.SetFloat ("HandPosition", 1);
			}
		}


	}
}
