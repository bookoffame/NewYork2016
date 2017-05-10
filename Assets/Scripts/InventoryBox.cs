using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryBox : MonoBehaviour
{
	public static InventoryBox current;

	public LetterInInventory[] items;
	public Text text;

	private char[] itemValues;
	private int itemCount, selected;

	void  Start (){
		if (current == null)
			current = this;
		itemCount = 0;
		itemValues = new char[items.Length];
		OnClose ();
	}
	/// <summary>
	/// Makes the InventoryBox appear.
	/// </summary>
	public void Show(){
		gameObject.SetActive (true);
	}

	/// <summary>
	/// Action to perform when closed is clicked.
	/// </summary>
	public void OnClose ()
	{
		text.text = "Hmm? You wish to consult your captured letters?";
		if (selected != -1)
		    items [selected].selected = false;
		selected = -1;
		gameObject.SetActive (false);
	}

	public void Add(char item){
		items [itemCount].img.sprite = getLetterSprite (item);
		itemValues [itemCount] = item;
		itemCount++;

		if (itemCount == 1) {
			text.text = "Nice catch! We'll just fit this little guy in right here for now. And because I'm such a nice bird, I'm going to keep track of all the letters you collect in your inventory. Click on the knapsack below at any tim to see what you’ve found. But hey -- I'm not putting these in their correct order or anything. That'll be your job to order the letters once you collect them all. If you click and drag a letter, you can move it to any of the ten spaces. Keep this in mind as you collect more and more letters! Now then, get to it! Time flies in the blink of an eye, as they say!";
			Show ();
		} else if (itemCount == 10) {
			text.text = "Way to go! That's all of them. It's a good thing, too. Time's almost up! Now before we go back to see the Queen, we have to reform the word. Don't worry, time doesn't flow when the inventory menu is open -- consider it a perk of being a digitized bird. It's no fowl, you know what I mean? Okay, I'll stop jabbering and let you think this one through. When you're ready, switch back to the Lens of Auctoritee and knock on the Queen's corrupted door.";
			Show ();
		}
	}

	public string GetWord(){
		string output = "";
		for (int i = 0; i < itemValues.Length - 1; i++)
			output += itemValues[i].ToString().ToUpper() + '-';
		output += itemValues [itemValues.Length - 1].ToString ().ToUpper ();
		return output;
	}

	public void ClickedSpot(int i){
		if (i >= itemCount)
			return;
		if (selected == -1) {
			selected = i;
		} else {
			Sprite tempS = items[selected].img.sprite;
			char tempC = itemValues[selected];

			items [selected].img.sprite = items [i].img.sprite;
			itemValues [selected] = itemValues [i];

			items [i].img.sprite = tempS;
			itemValues [i] = tempC;

			items [selected].selected = false;
			items [i].selected = false;
			selected = -1;
		}
	}

	private Sprite getLetterSprite(char letter){
		return Resources.Load<Sprite> ("Letters/" + letter);
	}
}

