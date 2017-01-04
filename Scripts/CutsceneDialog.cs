using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CutsceneDialog : MonoBehaviour {

	public Image portrait;
	public Image scroll;
	public Text text;

	public IEnumerator ShowDialog(Sprite character, string dialog){
		portrait.sprite = character;
		portrait.gameObject.SetActive (true);
		scroll.gameObject.SetActive (true);
		text.gameObject.SetActive (true);
		string[] lines = dialog.Split(new char[]{'\n'});
		for (int i = 0; i < (lines.Length + 2) / 3; i++) {
			text.text = GetStringIfExists (lines, i * 3) + "\n" +
			            GetStringIfExists (lines, i * 3 + 1) + "\n" +
			            GetStringIfExists (lines, i * 3 + 2);
			yield return new WaitUntil (() => Input.GetMouseButtonDown(0));
			yield return new WaitForSeconds (0.1f);
		}
		portrait.gameObject.SetActive (false);
		scroll.gameObject.SetActive (false);
		text.gameObject.SetActive (false);
	}

	private string GetStringIfExists(string[] arr, int index){
		if (index >= arr.Length)
			return "";
		else
			return arr [index];
	}
}
