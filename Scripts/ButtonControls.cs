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

	public Image twitterBird;
	public Sprite twitterBirdClosed;
	public Sprite twitterBirdOpen;
	public AudioSource twitterSound;

	public static ButtonControls current;
	public const int ANNOTATION_TOOL = 1;
	public const int HAND_TOOL = 2;
	public const int READER_TOOL = 4;
	public const int SELECTION_TOOL = 6;
	// Use this for initialization
	void Start () {
		selected = -1;
		for (int i = 0; i < buttons.Length; i++)
			buttons [i].image.color = Color.cyan;
		for (int i = 0; i < images.Length; i++)
			images [i].color = new Color (0.9f,0.9f,0.9f,1);
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
		clearLast ();
		selected = newSelected;
		if (newSelected == READER_TOOL)
			presenter.ShowAnnotations (true);
		buttons [selected].image.color = Color.green;
		images [selected].color = new Color (1,1,1,1);
		buttons [selected].interactable = false;
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
			images [selected].color = new Color (0.9f,0.9f,0.9f,1);
			buttons [selected].interactable = true;
		}
	}
}
