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
	public SpriteRenderer leftLock, rightLock,bugImg;
	public YesNoDialog yesNoDialog;
	public GameObject letters;
	public ParticleSystem letterEffect;

	private int state, frameCount;
	private bool inDialog;
	private SpriteRenderer bugRenderer;
	private bool done, result;
	private int newI;

	// Use this for initialization
	void Start () {
		state = 0;
		frameCount = 0;
		inDialog = false;
		bugRenderer = bug.gameObject.GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		bug.enabled = pageImages.OnPage (84);
		if (!bug.enabled) {
			bug.Hide ();
			bugRenderer.color = new Color (bugRenderer.color.r,bugRenderer.color.g,bugRenderer.color.b,0f);
		}
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
				bugImg.enabled = true;
				StartCoroutine (ShowDialog ("OnFirstMagicUse"));
				state = 4;
			}
			break;

		case 4://Used Magic twice (minigame start)
			if (!inDialog)
				bugImg.enabled = false;
				
			if (bug.IsShowing () && Input.GetMouseButtonDown (0) && !inDialog) {
				bugImg.enabled = true;
				StartCoroutine (ShowDialog ("OnSecondMagicUse"));
				state = 5;
			}
			break;

		case 5://Used Magic twice (minigame start)
			if (!inDialog) {
				bugImg.enabled = false;
				StartCoroutine (PlayCutscene ("MinigameStart"));
				state = 6;
			}
			break;

		case 6://Minigame start
			if (!inDialog) {
				StartCoroutine (ShowDialog ("OnMinigameStartCutsceneExit"));
				state = 7;
			}
			break;

		case 7://Used Flashlight
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
				state = 8;
			}
			break;

		case 8://Finding letters
			if (!inDialog && letters.transform.childCount == 0 && bug.IsShowing () && Input.GetMouseButtonDown (0)) {
				StartCoroutine (PlayCutscene ("MinigameEnd"));
				state = 9;
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

			done = false;
			result = false;
			StartCoroutine (HandleDialogEvent(name,image,text,i));
			yield return new WaitUntil (() => done);

			if (!result) {
				yield return new WaitForSeconds (0.1f);
				yield return StartCoroutine (cutsceneDialog.ShowDialog (Resources.Load<Sprite> ("Portraits/" + image), text));
				yield return new WaitForSeconds (0.4f);
			} else {
				i = newI;
				yield return new WaitForSeconds (0.1f);
			}
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

			done = false;
			result = false;
			StartCoroutine (HandleDialogEvent(name,image,text,i));
			yield return new WaitUntil (() => done);

			if (!result) {
				yield return new WaitForSeconds (0.1f);
				yield return StartCoroutine (cutsceneDialog.ShowDialog (Resources.Load<Sprite> ("Portraits/" + image), text));
				yield return new WaitForSeconds (0.4f);
			} else {
				i = newI;
				yield return new WaitForSeconds (0.1f);
			}
		}
		controls.setLocked (false);
		if (tool == ButtonControls.HAND_TOOL || tool == ButtonControls.SELECTION_TOOL)
		    controls.changeSelected (tool);
		inDialog = false;
	}

	private IEnumerator HandleDialogEvent(string name, string args, string dialog, int i)
	{
		newI = i;
		if (name.Equals ("Lock Book")) {
			bool lockBook = System.Boolean.Parse (args.Trim ());
			left.enabled = !lockBook;
			right.enabled = !lockBook;
			leftLock.enabled = lockBook;
			rightLock.enabled = lockBook;
			yield return new WaitForSeconds (0.1f);
			result = true;
			done = true;
			yield return true;
		} else if (name.Equals ("Show Letters")) {
			
			yield return new WaitForSeconds (0.1f);
			letterEffect.Play ();
			letters.SetActive (true);
			result = true;
			done = true;
			yield return true;
		} else if (name.Equals ("Choice")) {
			string[] choices = args.Split (new char[] { ',' });
			int yesChoice = System.Int32.Parse (choices [0].Trim ()) - 1;
			int noChoice = System.Int32.Parse (choices [1].Trim ()) - 1;
			YesNoDialog.dialog = yesNoDialog;
			dialog = dialog.Replace ("$i", InventoryBox.current.GetWord ());
			yield return StartCoroutine (YesNoDialog.ShowDialog (dialog));

			if (YesNoDialog.Choice ()) {
				newI = yesChoice;
			} else {
				newI = noChoice;
			}
			yield return new WaitForSeconds (0.1f);
			result = true;
			done = true;
			yield return true;
		} else if (name.Equals ("Check")) {
			string[] choices = args.Split (new char[] { ',' });
			int yesChoice = System.Int32.Parse (choices [0].Trim ()) - 1;
			int noChoice = System.Int32.Parse (choices [1].Trim ()) - 1;
			string answer = choices [2];

			if (answer.Equals(InventoryBox.current.GetWord())) {
				newI = yesChoice;
			} else {
				newI = noChoice;
			}

			yield return new WaitForSeconds (0.1f);
			result = true;
			done = true;
			yield return true;
		}
		else if (name.Equals ("Inventory")) {
			InventoryBox.current.text.text = dialog;
			InventoryBox.current.Show();
			yield return new WaitWhile (() => InventoryBox.current.gameObject.activeInHierarchy);
			yield return new WaitForSeconds (0.1f);
			result = true;
			done = true;
			yield return true;
		}
			
		result = false;
		done = true;
		yield return false;

	}
}
