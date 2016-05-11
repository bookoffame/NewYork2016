using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveTranscriptionLens : MonoBehaviour {

	public Image lensImg, maskImg;

	void Update () {
		lensImg.enabled = ButtonControls.current.getSelected () == ButtonControls.LENS_TOOL;
		maskImg.enabled = ButtonControls.current.getSelected () == ButtonControls.LENS_TOOL;
		transform.GetChild (0).gameObject.SetActive (ButtonControls.current.getSelected () == ButtonControls.LENS_TOOL);
		transform.position = Input.mousePosition;
	}
}
