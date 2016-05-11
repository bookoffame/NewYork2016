using UnityEngine;
using System.Collections;

public class HandOnPage : MonoBehaviour {
	public Animator animator;
	public Collider page;
	public PageImages pageImages;
	public float pageWidth;
	public bool isRight;
	public Renderer[] others;

	private float lastPos;
	private bool released;

	void Start()
	{
		lastPos = 0;
		released = false;
	}
	// Update is called once per frame
	void Update () {
		bool hand = ButtonControls.current.getSelected () == ButtonControls.HAND_TOOL;
		RaycastHit hit;
		if (!hand || !released)
			animator.SetFloat ("HandMovement", -pageWidth * (Input.mousePosition.x - lastPos) / Screen.width);
		
		if (!((pageImages.IsLoadingLeft () && isRight) || (pageImages.IsLoadingRight () && !isRight))) {
			lastPos = Input.mousePosition.x;

			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("MovePages")) {
				if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime < 0.7) {
					foreach (Renderer r in others)
						r.enabled = true;
				} else {
					foreach (Renderer r in others)
						r.enabled = false;
				}
			}
			if (hand && Input.GetMouseButtonDown (0) &&
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
					loadPages ();
					animator.SetTrigger ("Released");
				} else if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime < 0.01) {
					animator.SetTrigger ("Released");
				}
			}
		}
	}

	private void loadPages(){
		if (isRight) {
			StartCoroutine (pageImages.TurnPageLeft ());
		} else {
			StartCoroutine (pageImages.TurnPageRight ());
		}
	}
}
