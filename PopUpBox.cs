using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpBox : MonoBehaviour {
	public InputField input;
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
		myText = input.text;
		done = true;
	}

	public void Cancel(){
		myText = "";
		done = true;
	}

	public void reset(){
		done = false;
		input.text = "";
	}


}
