using UnityEngine;
using System.Collections;

public class AnnotationDrawer : MonoBehaviour {
	Annotation.AnnotationBox[] annotations;
	public Transform topLeft, bottomRight;
	Texture2D texture;

	void Start()
	{
		texture = new Texture2D(1,1);
		texture.SetPixel(1,1, new Color(0,0,1.0f,0.5f));
		texture.Apply();
		annotations = new Annotation.AnnotationBox[0];
	}

	public void UpdatesAnnotations(Annotation.AnnotationBox[] annos){
		annotations = annos;
	}

	void OnGUI(){
		Vector3 myTopLeft = Camera.main.WorldToScreenPoint (topLeft.position);
		Vector3 myBottomRight = Camera.main.WorldToScreenPoint (bottomRight.position);
		myTopLeft.y = Screen.height - myTopLeft.y;
		myBottomRight.y = Screen.height - myBottomRight.y;
		float myWidth = myBottomRight.x - myTopLeft.x;
		float myHeight = myBottomRight.y - myTopLeft.y;
		foreach (Annotation.AnnotationBox a in annotations) {
			Rect pos = new Rect (
				           myTopLeft.x + myWidth * a.x,
				           myTopLeft.y + myHeight * a.y,
				           myWidth * a.w,
				           myHeight * a.h);

			GUI.DrawTexture (pos, texture);
			GUI.Label (pos, a.contents);
		}
	}
}
