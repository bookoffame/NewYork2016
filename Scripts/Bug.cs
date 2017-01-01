using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bug : MonoBehaviour
{
    public SpriteRenderer image;
	private bool isShowing;

	void Start()
	{
		isShowing = false;
	}
	// Update is called once per frame
	void Update ()
	{
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

	void OnMouseEnter()
	{
		Show ();
	}

	void OnMouseExit()
	{
		Hide ();
	}
		
}

