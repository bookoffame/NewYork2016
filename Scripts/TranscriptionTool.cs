using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TranscriptionTool : MonoBehaviour {
	public Transform topLeft, bottomRight;
	public RectTransform annotations;
	public Transform canvas;

	private Annotation.AnnotationBox boxes;
	private Texture2D texture;
	private Text text;

	void Start()
	{
		texture = new Texture2D(1,1);
		texture.SetPixel(1,1, new Color(0,0,1.0f,0.5f));
		texture.Apply();
	}

	public void UpdatesTranscriptions(Annotation.AnnotationBox annos){
		boxes = annos;
		text = annotations.gameObject.GetComponentInChildren<Text> ();
		text.text = annos.contents;
	}
		
	void Update(){
		Vector3 myTopLeft = Camera.main.WorldToScreenPoint (topLeft.position);
		Vector3 myBottomRight = Camera.main.WorldToScreenPoint (bottomRight.position);
		myTopLeft.y = Screen.height - myTopLeft.y;
		myBottomRight.y = Screen.height - myBottomRight.y;

		float myWidth = myBottomRight.x - myTopLeft.x;
		float myHeight = myBottomRight.y - myTopLeft.y;

		Rect pos = new Rect (
			myTopLeft.x + myWidth * boxes.x,
			myTopLeft.y + myHeight * boxes.y,
			myWidth * boxes.w,
			myHeight * boxes.h);

		Vector3 location = new Vector3 (pos.x*0.875f - Screen.width*0.4535f, Screen.height*0.395f - pos.y*0.875f);

		annotations.localPosition = location - annotations.parent.parent.localPosition;
		Vector2 size = new Vector2 (pos.width*1.35f, pos.height*1.35f);
		annotations.sizeDelta = size;
	}
}
