using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveMinigameLens : MonoBehaviour
{
	/// <summary>
	/// The image of the lens.
	/// </summary>
	public Image lensImg;

	void Update () {
		lensImg.enabled = ButtonControls.current.getSelected () == ButtonControls.MINI_GAME_LENS_TOOL;
		transform.position = Input.mousePosition;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (lensImg.enabled) {
			Bug bug = other.gameObject.GetComponent<Bug> ();
			if (bug != null) {
				bug.Show ();
			}
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		Bug bug = other.gameObject.GetComponent<Bug> ();
		if (bug != null) {
			bug.Hide ();
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		Bug bug = other.gameObject.GetComponent<Bug> ();
		if (bug != null) {
			if (!lensImg.enabled) {
				bug.Hide ();
			}
		}
	}
}

