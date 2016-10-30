using UnityEngine;
using System.Collections;

/// <summary>
/// Controls page movement animation
/// </summary>
public class HandOnPage : MonoBehaviour {
	/// <summary>
	/// The animator of the page.
	/// </summary>
	public Animator animator;

	/// <summary>
	/// The Collider of the page.
	/// </summary>
	public Collider page;

	/// <summary>
	/// The presenter of the IIIF images.
	/// </summary>
	public PageImages pageImages;

	/// <summary>
	/// The width of the page.
	/// </summary>
	public float pageWidth;

	/// <summary>
	/// Is this the right page?
	/// </summary>
	public bool isRight;

	/// <summary>
	/// Pages to hide when this page is over them
	/// </summary>
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
		if (!hand || !released){
		    if (isRight)
			    animator.SetFloat ("HandMovement", -pageWidth * (Input.mousePosition.x - lastPos) / Screen.width);
			else
				animator.SetFloat ("HandMovement", pageWidth * (Input.mousePosition.x - lastPos) / Screen.width);
		}
		
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
			} //OLD

			//New
			/*if (hand && Input.GetMouseButtonDown (0) &&
				animator.GetCurrentAnimatorStateInfo (0).IsName ("Opened")) {
				if (page.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000)) {
					if (isRight)
					    animator.SetTrigger ("TurnRight");
					else
						animator.SetTrigger ("TurnLeft");
					released = false;
				}
			} else if (Input.GetMouseButtonUp (0) && isPageTurning()) {
				released = true;
				if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime < 0.5)
					animator.SetFloat ("HandMovement", -5);
				else
					animator.SetFloat ("HandMovement", 5);
			} else if (!Input.GetMouseButton (0) && isPageTurning()) {
				if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.95) {
					loadPages ();
					animator.SetTrigger ("Released");
				} else if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime < 0.01) {
					animator.SetTrigger ("Released");
				}
			}*/
		}
	}

	private void loadPages(){
		foreach (Renderer r in others)
			r.enabled = true;
		if (isRight) {
			StartCoroutine (pageImages.TurnPageLeft ());
		} else {
			StartCoroutine (pageImages.TurnPageRight ());
		}
	}

	private bool isPageTurning(){
		if (isRight)
			return animator.GetCurrentAnimatorStateInfo (0).IsName ("TurnPageRight");
		else
			return animator.GetCurrentAnimatorStateInfo (0).IsName ("TurnPageLeft");
	}
}
