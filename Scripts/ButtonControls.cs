using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;

public class ButtonControls : MonoBehaviour {
	public Button[] buttons;
	public Image[] images;
	public PopUpBox popup;
	public Move bookCam;
	public CameraSwitch switcher;
	public PageImages presenter;
	public GameObject tweetBox;
	public Text tweetText;

	private int selected;
	private string popupText;
	private Regex tweetRegex;

	public bool isSpotlight = false;
	public MoveSpotlight spotlight;

	public GameObject twitterBirdObj;
	public Image twitterBird;
	public Sprite twitterBirdClosed;
	public Sprite twitterBirdOpen;
	public AudioSource twitterSound;
	public DialogBox dialog;

	public static ButtonControls current;
	public const int LIGHT_TOOL = 0;
	public const int ANNOTATION_TOOL = 1;
	public const int HAND_TOOL = 2;
	public const int DIRECTORY_TOOL = 3;
	public const int READER_TOOL = 4;
	public const int SELECTION_TOOL = 6;
	public const int zoom_TOOL = 5;
	public const int LENS_TOOL = 8;
	public const int TWITTER_TOOL = 9;
	// Use this for initialization
	void Start () {
		selected = -1;
		for (int i = 0; i < buttons.Length; i++)
			buttons [i].image.color = Color.cyan;
		for (int i = 0; i < images.Length; i++)
			images [i].color = new Color (0.3f,0.3f,0.3f,1);
		current = this;
		tweetRegex = new Regex ("<div class=\"js-tweet-text-container\">\\s*?<p class=\"TweetTextSize TweetTextSize--16px js-tweet-text tweet-text\" lang=\"en\" data-aria-label-part=\"0\">(.*?)<\\/p>\\s*?<\\/div>");
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
		if (newSelected == TWITTER_TOOL) {
			twitterBirdObj.SetActive (!twitterBirdObj.activeInHierarchy);
			if (twitterBirdObj.activeInHierarchy)
				images [newSelected].color = new Color (1, 1, 1, 1);
			else
				images [newSelected].color = new Color (0.3f, 0.3f, 0.3f, 1);
		} 
		else if (newSelected == LIGHT_TOOL) {
			isSpotlight = !isSpotlight;
			spotlight.hideProperties (!isSpotlight);
			if (isSpotlight)
				images [newSelected].color = new Color (1, 1, 1, 1);
			else
				images [newSelected].color = new Color (0.3f, 0.3f, 0.3f, 1);
		}
		else if (newSelected != 3 && newSelected != 5 && newSelected != 7) {
			clearLast ();
			if (selected != newSelected) {
				selected = newSelected;
				if (newSelected == READER_TOOL)
					presenter.ShowAnnotations (true);
				buttons [selected].image.color = Color.green;
				images [selected].color = new Color (1, 1, 1, 1);
			} else {
				selected = -1;
			}
		} else {
			switch (newSelected) {
			case 3://Directory
				dialog.Show("Directory to be added soon!");
				break;

			case 5://Zoom
				dialog.Show("Zoom to be added soon!");
				break;

			case 7://Help
				dialog.Show("Controls:" +
					"Left/Right Arrow Keys to move Left/Right.\n" +
					"Up/Down Arrow Keys to move Up/Down.\n" +
					"W/S to zoom In/Out.\n" +
					"\n" +
					"Buttons:\n" +
					"Hand: Grab the pages with the cursor to turn to the next/previous page.\n" +
					"Magnify Glass: See a transcription of the text.\n" +
					"+: Select a region to add an annotation to the local annotation file.\n" +
					"Note with \"A\": View local annotations.\n" +
					"Bird: Clicking on it shows/hides a bird. Click the bird to get the latest Tweet from our account.\n" +
					"?: Show this help dialog.");
				break;
			}
		}
	}

	public void clearSelected(){
		clearLast ();
		selected = -1;
	}

	public void ShowLatestTweet(){
		WebClient client = new WebClient ();
		string data = client.DownloadString ("https://twitter.com/TabulaFamae");
		MatchCollection tweets = tweetRegex.Matches (data);
		twitterBird.sprite = twitterBirdOpen;
		tweetText.text = tweets[0].Groups[1].Value;
		tweetBox.SetActive (true);
		twitterSound.Play ();
		StartCoroutine (HideTweetBox ());
	}

	public IEnumerator HideTweetBox(){
		yield return new WaitForSeconds (2);
		twitterBird.sprite = twitterBirdClosed;
		tweetBox.SetActive (false);
	}

	private void clearLast()
	{
		if (selected == READER_TOOL)
			presenter.ShowAnnotations (false);
		if (selected != -1) {
			buttons [selected].image.color = Color.cyan;
			images [selected].color = new Color (0.3f,0.3f,0.3f,1);
		}
	}
}
