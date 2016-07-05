using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using AssemblyCSharp;

/// <summary>
/// Presents the IIIF images from a manifest on 6 pages.
/// </summary>
public class ImageBufferer : MonoBehaviour {
	/// <summary>
	/// The images for the buffered pages
	/// </summary>
	private Texture2D[] pageImages;

	/// <summary>
	/// Used to obtain the images.
	/// </summary>
	private IIIFImageGet[] downloaders;

	/// <summary>
	/// The manifest URL.
	/// </summary>
	public string manifestURL;

	/// <summary>
	/// The texture to display when loading images
	/// </summary>
	public Texture2D loadingTexture;

	private IIIFGetManifest data;

	private const int OFFSET = 2;

	private Hashtable pageToImage;

	private int curr;

	// Use this for initialization
	void Start () {
		pageImages = new Texture2D[12];
		data = new IIIFGetManifest ();
		pageToImage = new Hashtable();
		downloaders = new IIIFImageGet[pageImages.Length];
		for (int i = 0; i < downloaders.Length; i++) {
			pageImages [i] = loadingTexture;
			downloaders [i] = gameObject.AddComponent<IIIFImageGet>();
			downloaders [i].cropOffsetX = 60;
			downloaders [i].cropOffsetY = 210;
			downloaders [i].cropWidth = 2900;
			downloaders [i].cropHeight = 4000;
			downloaders [i].targetWidth = 2900;
			downloaders [i].targetHeight = 4000;
			downloaders [i].rotation = 0;
			downloaders [i].mirrored = false;
			downloaders [i].quality = "default";
			downloaders [i].format = ".jpg";
		}
		curr = 72;
		data.download(manifestURL);
		for (int i = 0; i < pageImages.Length; i++) {
			StartCoroutine (LoadPage (i));
		}
	}

	/// <summary>
	/// Shifts page's textures to the left and loads the next two pages.
	/// </summary>
	public void TurnPageLeft(){
		
		pageToImage.Remove (curr * 2 - 3);
		pageToImage.Remove (curr * 2 - 2);

		for (int i = 2; i < pageImages.Length; i++){
			if (pageToImage.ContainsKey (curr * 2 - 3 + i))
				pageToImage[curr * 2 - 3 + i] = i - 2;
		}

		curr++;

		for (int i = 0; i < pageImages.Length - 2; i++){
			pageImages [i] = pageImages [i + 2];
		}
		
		StartCoroutine (LoadPage (pageImages.Length - 2));
		StartCoroutine (LoadPage (pageImages.Length - 1));
	}

	/// <summary>
	/// Shifts page's textures to the right and loads the previous two pages.
	/// </summary>
	public void TurnPageRight(){
		
		pageToImage.Remove (curr * 2 - 4 + pageImages.Length);
		pageToImage.Remove (curr * 2 - 5 + pageImages.Length);

		for (int i = 0; i < pageImages.Length - 2; i++) {
			if (pageToImage.ContainsKey (curr * 2 - 3 + i))
				pageToImage [curr * 2 - 3 + i] = i + 2;
		}

		curr--;

		for (int i = pageImages.Length - 1; i > 1; i--){
			pageImages [i] = pageImages [i - 2];
	    }
		StartCoroutine (LoadPage (0));
		StartCoroutine (LoadPage (1));
	}

	public Texture2D GetImage(int page){
		return pageImages [page + OFFSET];
	}

	public string GetURL(int pageNum){
		return data.getPage (pageNum);
	}

	private IEnumerator LoadPage(int image)
	{
		int pageNum = curr * 2 - 3 + image;
		pageToImage [pageNum] = image;
		if (pageNum > 0 && pageNum < data.getNumOfPages ()) {
			downloaders [image].StopAllCoroutines ();
			pageImages [image] = loadingTexture;
			if (pageNum % 2 == 1) {
				downloaders[image].cropOffsetX = 175;
			} else {
				downloaders[image].cropOffsetX = 60;
			}
			downloaders[image].changeAddress (data.getPage (pageNum));
			downloaders[image].targetWidth = downloaders[image].cropWidth/2;
			downloaders[image].targetHeight = downloaders[image].cropHeight/2;
			yield return StartCoroutine (downloaders[image].UpdateImage ());
			if (!pageToImage.Contains(pageNum)) {
				yield break;
			}
			pageImages [(int)pageToImage[pageNum]] = downloaders [image].texture;
			downloaders[image].targetWidth = downloaders[image].cropWidth;
			downloaders[image].targetHeight = downloaders[image].cropHeight;
			yield return StartCoroutine (downloaders[image].UpdateImage ());
			if (!pageToImage.Contains(pageNum)) {
				yield break;
			}
			pageImages [(int)pageToImage[pageNum]] = downloaders [image].texture;
		} else {
			pageImages [image] = loadingTexture;
		}
		pageToImage.Remove (pageNum);
	}

	void OnGUI(){
		string output = "";
		ArrayList list = new ArrayList ();
		foreach (int key in pageToImage.Keys)
			list.Add (key);
		list.Sort ();
		foreach (int key in list)
			if (key % 2 == 0)
				output += "Loading page " + ((key - 3)/2).ToString () + "v to " + pageToImage [key].ToString () + ".\n";
		    else
				output += "Loading page " + ((key - 3)/2).ToString () + "r to " + pageToImage [key].ToString () + ".\n";
		GUI.Box (new Rect (0,0,200,20*pageToImage.Count), output);
	}

}
