using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CutsceneDialog : MonoBehaviour {

	public SetPortrait portrait;
	public Image scroll;
	public Text text;
	private const int MAX_DIALOG_LENGTH = 250;

	public IEnumerator ShowDialog(string character, string dialog){
		portrait.gameObject.SetActive (true);
		portrait.SetImage(character);
		scroll.gameObject.SetActive (true);
		text.gameObject.SetActive (true);
		dialog = dialog.Replace ('\n', ' ');

		for (int i = 0, end = GetEnd(dialog, i); i < dialog.Length; i = end, end = GetEnd(dialog, i)) {
			text.text = dialog.Substring (i, end - i);
			yield return new WaitUntil (() => Input.GetMouseButtonDown(0));
			yield return new WaitForSeconds (0.1f);
		}
		portrait.gameObject.SetActive (false);
		scroll.gameObject.SetActive (false);
		text.gameObject.SetActive (false);
	}

	private int GetEnd(string dialog, int index){
		int lastWordEnd = index;
		int lastLastWordEnd = lastWordEnd;

		while (lastWordEnd - index < MAX_DIALOG_LENGTH && lastWordEnd < dialog.Length) {
			lastLastWordEnd = lastWordEnd;
			lastWordEnd = dialog.IndexOf (' ', lastWordEnd + 1);
			if (lastWordEnd == -1) {
				return dialog.Length;
			}
		}
		return lastLastWordEnd;
	}
}
