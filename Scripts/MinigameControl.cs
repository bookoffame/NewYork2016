using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class MinigameControl : MonoBehaviour {

	public PageImages pageImages;
	public PopupTheatre theatre;
	public CutsceneDialog cutsceneDialog;
	public ButtonControls controls;
	public HandOnPage left, right;
	public Bug bug;
	public SpriteRenderer leftLock, rightLock;


	private int state, frameCount;
	private bool inDialog;

	// Use this for initialization
	void Start () {
		state = 0;
		frameCount = 0;
		inDialog = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!pageImages.OnPage (84))
		    bug.Hide ();
		switch (state) {
		case 0://Reached Page
			if (pageImages.OnPage (84)) {
		        StartCoroutine (ShowDialog ("OnPageEnter"));
				state = 1;
			}
			break;
		case 1://Used Lens
			if (controls.getSelected () == ButtonControls.MINI_GAME_LENS_TOOL) {
				StartCoroutine (ShowDialog ("OnUsedLens"));
				state = 2;
			}
			break;

		case 2://Found Bug
			if (bug.IsShowing ()) {
				frameCount++;
			} else {
				frameCount = 0;
			}
			if (frameCount >= 120 && !inDialog) {
				StartCoroutine (ShowDialog ("OnFoundBug"));
				state = 3;
			}
			break;

		case 3://Used Magic once
			if (bug.IsShowing () && Input.GetMouseButtonDown (0) && !inDialog) {
				StartCoroutine (ShowDialog ("OnFirstMagicUse"));
				state = 4;
			}
			break;

		case 4://Used Magic twice (minigame start)
			if (bug.IsShowing () && Input.GetMouseButtonDown (0) && !inDialog) {
				StartCoroutine (PlayCutscene ("MinigameStart"));
				state = 5;
			}
			break;

		case 5://Minigame start
			if (!inDialog) {
				StartCoroutine (ShowDialog ("OnMinigameStartCutsceneExit"));
				state = 6;
			}
			break;

		case 6://Used Flashlight
			if (pageImages.OnPage (84)) {
				left.enabled = false;
				right.enabled = false;
				leftLock.enabled = true;
				rightLock.enabled = true;
			} else {
				left.enabled = true;
				right.enabled = true;
				leftLock.enabled = false;
				rightLock.enabled = false;
			}
			if (!inDialog && controls.isSpotlight) {
				StartCoroutine (ShowDialog ("OnUsedFlashlight"));
				state = 7;
			}
			break;

		default:
			break;
			
		}
	}

	private IEnumerator PlayCutscene(string cutscene){
		int tool = controls.getSelected ();
		inDialog = true;
		controls.clearSelected ();
		controls.setLocked (true);
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
		controls.setLocked (false);
		controls.changeSelected (tool);
		inDialog = false;
	}

	private IEnumerator ShowDialog(string dialog){
		int tool = controls.getSelected ();
		inDialog = true;
		if (tool == ButtonControls.HAND_TOOL || tool == ButtonControls.SELECTION_TOOL)
			controls.clearSelected ();
		controls.setLocked (true);
		string data = Resources.Load<TextAsset> ("Dialog/" + dialog).text;
		string[] lines = data.Split(new string[]{"\n\n"},System.StringSplitOptions.None);

		for (int i = 0; i < lines.Length; i++) {
			string name = lines [i].Substring (0, lines [i].IndexOf (":"));
			string image = lines [i].Substring (lines [i].IndexOf (":")+1,lines [i].IndexOf("\n") - lines [i].IndexOf (":") - 1);
			string text = lines [i].Substring (lines [i].IndexOf ("\n") + 1);
			yield return new WaitForSeconds (0.1f);
			yield return StartCoroutine(cutsceneDialog.ShowDialog (Resources.Load<Sprite> ("Portraits/" + image),text));
			yield return new WaitForSeconds (0.4f);
		}
		controls.setLocked (false);
		if (tool == ButtonControls.HAND_TOOL || tool == ButtonControls.SELECTION_TOOL)
		    controls.changeSelected (tool);
		inDialog = false;
	}
}
