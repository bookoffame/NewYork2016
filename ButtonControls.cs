using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonControls : MonoBehaviour {
	public Button[] buttons;
	public Image[] images;

	private int selected;

	// Use this for initialization
	void Start () {
		selected = -1;
		for (int i = 0; i < buttons.Length; i++)
			buttons [i].image.color = Color.cyan;
		for (int i = 0; i < images.Length; i++)
			images [i].color = new Color (0.9f,0.9f,0.9f,1);
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
