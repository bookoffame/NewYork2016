using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogBox : MonoBehaviour {
	public Text myText;

	public void Show(string text){
		myText.text = text;
		gameObject.SetActive (true);
	}

	public void OnOkay(){
		gameObject.SetActive (false);
	}
}
