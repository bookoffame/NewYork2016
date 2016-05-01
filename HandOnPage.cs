using UnityEngine;
using System.Collections;

public class HandOnPage : MonoBehaviour {
	public Animator animator;
	public Collider page;
	public PageImages pageImages;
	public float pageWidth;
	public bool isRight;

	private float lastPos;
	private bool released;

	void Start()
	{
		lastPos = 0;
		released = false;
	}
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if (!released)
		    animator.SetFloat ("HandMovement", -pageWidth*(Input.mousePosition.x - lastPos)/Screen.width);
		lastPos = Input.mousePosition.x;

		if (Input.GetMouseButtonDown (0) &&
		    animator.GetCurrentAnimatorStateInfo (0).IsName ("Default")) {
			if (page.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000)) {
				animator.SetTrigger ("Grabbed");
				released = false;
			}
		} else if (Input.GetMouseButtonUp (0) &&
		           animator.GetCurrentAnimatorStateInfo (0).IsName ("MovePages")) {
			released = true;
			if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime < 0.35)
				animator.SetFloat ("HandMovement", -5);
			else
				animator.SetFloat ("HandMovement", 5);
		} else if (!Input.GetMouseButton (0) &&
		         animator.GetCurrentAnimatorStateInfo (0).IsName ("MovePages")) {
			if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.95) {
				if (isRight)
					StartCoroutine(pageImages.TurnPageLeft ());
				else
					StartCoroutine(pageImages.TurnPageRight());
				animator.SetTrigger ("Released");
			} else if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime < 0.01) {
				animator.SetTrigger ("Released");
			}
		}
	}
}
