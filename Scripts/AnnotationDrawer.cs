using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnnotationDrawer : MonoBehaviour {
	
	public Transform topLeft, bottomRight;
	public Transform annoObj;
	public Transform canvas;

	private Annotation.AnnotationBox[] boxes;
	private RectTransform[] annotations;
	private Texture2D texture;
	private bool isShowing;

	void Start()
	{
		texture = new Texture2D(1,1);
		texture.SetPixel(1,1, new Color(0,0,1.0f,0.5f));
		texture.Apply();
		boxes = new Annotation.AnnotationBox[0];
		annotations = new RectTransform [0];
		isShowing = false;
	}

	public void UpdatesAnnotations(Annotation.AnnotationBox[] annos){
		for (int i = 0; i < annotations.Length; i++)
			Destroy (annotations [i].gameObject);
		boxes = annos;
		annotations = new RectTransform[annos.Length];
		for (int i = 0; i < annos.Length; i++) {
			Transform o = Instantiate (annoObj);
			o.SetParent (canvas, false);
			o.SetAsFirstSibling ();
			annotations[i] = o.GetComponent<RectTransform> ();
			o.GetComponentInChildren<Text>().text = annos[i].contents;
			o.gameObject.SetActive (isShowing);
		}
	}

	public void ShowAnnotations(bool isShowing){
		this.isShowing = isShowing;
		foreach (Transform o in annotations) {
			o.gameObject.SetActive (isShowing);
		}
	}

	void Update(){
		Vector3 myTopLeft = Camera.main.WorldToScreenPoint (topLeft.position);
		Vector3 myBottomRight = Camera.main.WorldToScreenPoint (bottomRight.position);
		myTopLeft.y = Screen.height - myTopLeft.y;
		myBottomRight.y = Screen.height - myBottomRight.y;

		float myWidth = myBottomRight.x - myTopLeft.x;
		float myHeight = myBottomRight.y - myTopLeft.y;

		for (int i = 0; i < boxes.Length; i++) {
			Rect pos = new Rect (
				myTopLeft.x + myWidth * boxes[i].x,
				myTopLeft.y + myHeight * boxes[i].y,
				myWidth * boxes[i].w,
				myHeight * boxes[i].h);

			Vector2 location = new Vector2 (pos.x / Screen.width, 1 - (pos.y / Screen.height));
			annotations [i].anchorMin = location;
			annotations [i].anchorMax = location;
			annotations [i].sizeDelta = new Vector2 (pos.width, pos.height);
		}
	}
}
