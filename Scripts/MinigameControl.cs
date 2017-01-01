using UnityEngine;
using System.Collections;
using System.IO;

public class MinigameControl : MonoBehaviour {

	public PageImages pageImages;
	public PopupTheatre theatre;
	public CutsceneDialog cutsceneDialog;
	public ButtonControls controls;

	private bool reachedPage;

	// Use this for initialization
	void Start () {
		reachedPage = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!reachedPage) {
			if (pageImages.OnPage (84)) {
				StartCoroutine(PlayCutscene("Test"));
				reachedPage = true;
			}
		}
	}

	private IEnumerator PlayCutscene(string cutscene){
		controls.clearSelected ();
		StartCoroutine(MusicControl.controller.ChangeSong(MusicControl.controller.COURT_MUSIC));
		yield return StartCoroutine(theatre.SetupPopupTheatre ());
		yield return new WaitForSeconds (5f);
		string data = Resources.Load<TextAsset> ("Cutscenes/" + cutscene).text;
		string[] lines = data.Split(new string[]{"\n\n"},System.StringSplitOptions.None);

		for (int i = 0; i < lines.Length; i++) {
			string name = lines [i].Substring (0, lines [i].IndexOf (":"));
			string image = lines [i].Substring (lines [i].IndexOf (":")+1,lines [i].IndexOf("\n") - lines [i].IndexOf (":") - 1);
			string text = lines [i].Substring (lines [i].IndexOf ("\n") + 1);
			yield return StartCoroutine(cutsceneDialog.ShowDialog (Resources.Load<Sprite> ("Portraits/" + image),text));
			yield return new WaitForSeconds (0.5f);
		}
		StartCoroutine(MusicControl.controller.ChangeSong(MusicControl.controller.MINIGAME_MUSIC));
		yield return StartCoroutine(theatre.LeavePopupTheatre());
	}
}
