using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveMinigameLens : MonoBehaviour
{
	/// <summary>
	/// The image of the lens.
	/// </summary>
	public Image lensImg;

	/// <summary>
	/// The image of the mask.
	/// </summary>
	public Image maskImg;

	void Update () {
		lensImg.enabled = ButtonControls.current.getSelected () == ButtonControls.MINI_GAME_LENS_TOOL;
		maskImg.enabled = ButtonControls.current.getSelected () == ButtonControls.MINI_GAME_LENS_TOOL;
		transform.GetChild (0).gameObject.SetActive (ButtonControls.current.getSelected () == ButtonControls.MINI_GAME_LENS_TOOL);
		transform.position = Input.mousePosition;
	}
}

