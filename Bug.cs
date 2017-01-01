using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bug : MonoBehaviour {

	public Image image;
	public Transform topLeft, bottomRight;
	public Vector2 location;

	private bool isShowing;
	private const float CAM_SCALE = 0.4f;
	private const float CAM_MIN = 2f;

	void Start()
	{
		isShowing = true;
	}

	// Update is called once per frame
	void Update () {
		Vector3 OFFSET = new Vector3 (-0.5f*Screen.width,-0.4f*Screen.height,0);
		Vector3 tl = Camera.main.WorldToViewportPoint (topLeft.position);
		Vector3 br = Camera.main.WorldToViewportPoint (bottomRight.position);
		float width = tl.x - br.x;
		float height = tl.y - br.y;

		transform.localPosition = new Vector3 ((tl.x - location.x*width*0.8f)*Screen.width,
			(br.y + location.y*height*0.8f)*Screen.height,
			0) + OFFSET;
			
		transform.localScale = new Vector3 (CAM_SCALE*Camera.main.transform.localPosition.z + CAM_MIN,
			CAM_SCALE*Camera.main.transform.localPosition.z + CAM_MIN,
			CAM_SCALE*Camera.main.transform.localPosition.z + CAM_MIN);
		
		if (isShowing && image.color.a <= 0.99f) {
			image.color = new Color (image.color.r, image.color.g, image.color.b, image.color.a + 0.01f);
		} else if (!isShowing && image.color.a >= 0.01f) {
			image.color = new Color (image.color.r, image.color.g, image.color.b, image.color.a - 0.01f);
		}
	}

	public void Show(){
		isShowing = true;
	}

	public void Hide(){
		isShowing = false;
	}

	public bool IsShowing(){
		return isShowing;
	}
}
