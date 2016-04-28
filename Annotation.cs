using UnityEngine;
using System.Collections;

public class Annotation : MonoBehaviour {
	public int pageWidth, pageHeight;
	public Collider page;
	public RectTransform selection;

	private int sx, sy;
	private int w, h;
	private Vector2 originalStart;
	private bool annotating;
	void Update () {
		RaycastHit hit;
		if (page.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000)) {
			if (ButtonControls.current.getSelected () == ButtonControls.ANNOTATION_TOOL) {
				if (Input.GetMouseButtonDown (0)) {
					annotating = true;
					sx = (int)(pageWidth * hit.textureCoord.x);
					sy = (int)(pageHeight * hit.textureCoord.y);
					originalStart = Camera.main.WorldToScreenPoint (hit.point);
					selection.anchoredPosition = originalStart;
				} else if (Input.GetMouseButton (0)) {
					w = (int)(pageWidth * hit.textureCoord.x) - sx;
					h = (int)(pageHeight * hit.textureCoord.y) - sy;
					Vector3 sizeDelta = Camera.main.WorldToScreenPoint (hit.point) - (Vector3)originalStart;
					Vector2 newStart = selection.anchoredPosition;
					if (sizeDelta.x < 0) {
						newStart.x = originalStart.x + sizeDelta.x;
						sizeDelta.x = -sizeDelta.x;
					}
					if (sizeDelta.y < 0) {
						newStart.y = originalStart.y + sizeDelta.y;
						sizeDelta.y = -sizeDelta.y;
					}
					selection.anchoredPosition = newStart;
					selection.sizeDelta = sizeDelta;
				} else if (Input.GetMouseButtonUp (0)) {
					if (annotating) {
						if (w < 0) {
							sx += w;
							w = -w;
						}
						if (h < 0) {
							sy += h;
							h = -h;
						}
						annotating = false;
						MakeAnnotation (pageWidth - sx, sy, w, h);
					}
				}
			}
		} else if (!Input.GetMouseButton (0))
			annotating = false;
	}

	private void MakeAnnotation(int x, int y, int w, int h){
		Debug.Log ("x: " + x.ToString ());
		Debug.Log ("y: " + y.ToString ());
		Debug.Log ("w: " + w.ToString ());
		Debug.Log ("h: " + h.ToString ());
	}
}
