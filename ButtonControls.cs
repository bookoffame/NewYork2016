using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonControls : MonoBehaviour {
	public Button[] buttons;
	public Image[] images;
	public PopUpBox popup;
	public Move bookCam;
	public CameraSwitch switcher;

	private int selected;
	private string popupText;

	public static ButtonControls current;
	public const int ANNOTATION_TOOL = 1;
	public const int HAND_TOOL = 2;
	public const int READER_TOOL = 4;
	// Use this for initialization
	void Start () {
		selected = -1;
		for (int i = 0; i < buttons.Length; i++)
			buttons [i].image.color = Color.cyan;
		for (int i = 0; i < images.Length; i++)
			images [i].color = new Color (0.9f,0.9f,0.9f,1);
		current = this;
	}

	public IEnumerator PopUp(){
		int old = selected;
		clearSelected ();
		bookCam.setActivated (false);
		switcher.gameObject.SetActive (false);
		popup.gameObject.SetActive (true);
		popup.reset ();
		yield return popup.StartCoroutine ("PopUp");
		popupText =  popup.getText ();
		popup.gameObject.SetActive (false);
		bookCam.setActivated (true);
		switcher.gameObject.SetActive (true);
		changeSelected(old);
	}

	public string getPopupText(){
		return popupText;
	}
	public int getSelected(){
		return selected;
	}

	public void changeSelected(int newSelected){
		clearLast ();
		selected = newSelected;
		buttons [selected].image.color = Color.green;
		images [selected].color = new Color (1,1,1,1);
		buttons [selected].interactable = false;
	}

	public void clearSelected(){
		clearLast ();
		selected = -1;
	}

	private void clearLast()
	{
		if (selected != -1) {
			buttons [selected].image.color = Color.cyan;
			images [selected].color = new Color (0.9f,0.9f,0.9f,1);
			buttons [selected].interactable = true;
		}
	}
}
