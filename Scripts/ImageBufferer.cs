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
		for (int i = 0; i < pageImages.Length; i++) {
			pageImages [i] = loadingTexture;
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
			IIIFImageGet downloader = ScriptableObject.CreateInstance<IIIFImageGet>();
			downloader.cropOffsetY = 210;
			downloader.cropWidth = 2900;
			downloader.cropHeight = 4000;
			downloader.targetWidth = 2900;
			downloader.targetHeight = 4000;
			downloader.rotation = 0;
			downloader.mirrored = false;
			downloader.quality = "default";
			downloader.format = ".jpg";
			pageImages [image] = loadingTexture;
			if (pageNum % 2 == 1) {
				downloader.cropOffsetX = 175;
			} else {
				downloader.cropOffsetX = 60;
			}
			downloader.changeAddress (data.getPage (pageNum));
			downloader.targetWidth = downloader.cropWidth/2;
			downloader.targetHeight = downloader.cropHeight/2;
			yield return StartCoroutine (downloader.UpdateImage ());
			if (!pageToImage.Contains(pageNum)) {
				yield break;
			}
			pageImages [(int)pageToImage[pageNum]] = downloader.texture;
			downloader.targetWidth = downloader.cropWidth;
			downloader.targetHeight = downloader.cropHeight;
			yield return StartCoroutine (downloader.UpdateImage ());
			if (!pageToImage.Contains(pageNum)) {
				yield break;
			}
			pageImages [(int)pageToImage[pageNum]] = downloader.texture;
		} 
		else {
			pageImages [image] = loadingTexture;
		}
		pageToImage.Remove (pageNum);
	}

	/*void OnGUI(){
		string output = "";
		ArrayList list = new ArrayList ();
		foreach (int key in pageToImage.Keys)
			list.Add (key);
		list.Sort ();
		foreach (int key in list)
		    output += "Loading page " + (key - curr * 2 - 3) + " to " + pageToImage [key].ToString () + ".\n";
		GUI.Box (new Rect (0,0,200,20*pageToImage.Count), output);
	}*/

}
