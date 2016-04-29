using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpBox : MonoBehaviour {
	public Text enteredText;
	private string myText;
	private bool done;

	public IEnumerator PopUp(){
		done = false;
		myText = "";
		yield return new WaitUntil(() => done);
	}

	public string getText(){
		return myText;
	}

	public void Submit(){
		myText = enteredText.text;
		done = true;
	}

	public void Cancel(){
		myText = "";
		done = true;
	}

	public void reset(){
		done = false;
		enteredText.text = "";
	}


}
