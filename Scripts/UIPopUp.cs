using UnityEngine;
using System.Collections;

public class UIPopUp : MonoBehaviour {
	public float hideY, showY;
	public float triggerPos;
	public RectTransform pos;
	public bool hideAbove;
	private bool isHiding;
	// Update is called once per frame
	void Update () {
		if (((Screen.height - Input.mousePosition.y) / Screen.height < triggerPos) ^ hideAbove) {
			pos.localPosition = new Vector3 (pos.localPosition.x, Mathf.Lerp (pos.localPosition.y, hideY, 0.1f), pos.localPosition.z);
			isHiding = false;
		} else {
			pos.localPosition = new Vector3 (pos.localPosition.x, Mathf.Lerp (pos.localPosition.y, showY, 0.1f), pos.localPosition.z);
			isHiding = true;
		}
	}

	public bool IsShowing(){
		return !isHiding;
	}
}
