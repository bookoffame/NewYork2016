using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using AssemblyCSharp;

/// <summary>
/// Presents the IIIF images from a manifest on 6 pages.
/// </summary>
public class PageImages : MonoBehaviour {
	/// <summary>
	/// The pages to present the IIIF images on.
	/// </summary>
	public Renderer[] pages;

	/// <summary>
	/// Used to obtain the images.
	/// </summary>
	public IIIFImageGet iiifImage;

	/// <summary>
	/// The manifest URL.
	/// </summary>
	public string manifestURL;

	/// <summary>
	/// The texture to display when loading images
	/// </summary>
	public Texture2D loadingTexture;

	/// <summary>
	/// Retrieve infomation about annotations on the pages.
	/// </summary>
	public Annotation[] annotation;

	/// <summary>
	/// The annotation drawers.
	/// </summary>
	public AnnotationDrawer[] drawers;

	/// <summary>
	/// The left transcription.
	/// </summary>
	public TranscriptionTool leftTrans;

	/// <summary>
	/// The right transcription.
	/// </summary>
	public TranscriptionTool rightTrans;

	/// <summary>
	/// The text that display the page number.
	/// </summary>
	public Text pageDisplay;

	private ArrayList annotations;
	private IIIFGetManifest data;
	private int curr;
	private bool loadingRight, loadingLeft;
	private string transcription;

	// Use this for initialization
	void Start () {
		annotations = new ArrayList ();
		data = new IIIFGetManifest ();
		data.download(manifestURL);
		loadingLeft = true;
		loadingRight = true;
		StartCoroutine(init ());
	}

	private IEnumerator init()
	{
		for (int i = 0; i < 6; i++) {
			pages [i].enabled = true;
			pages [i].material.mainTexture = loadingTexture;
		}
		curr = 73;
		for (int i = 0; i < 6; i++)
			yield return StartCoroutine(InitPage (i));
		annotation [0].UpdateWebAddress (iiifImage.removeTail(data.getPage(curr*2 - 1)));
		annotation [1].UpdateWebAddress (iiifImage.removeTail(data.getPage(curr*2)));
		transcription = Resources.Load<TextAsset> ("Transcriptions/anno").text;
		UpdateAnnotations ();
		UpdatePageDisplay ();
		loadingLeft = false;
		loadingRight = false;
		yield return new WaitUntil(()=>true);
	}

	/// <summary>
	/// Shifts page's textures to the left and loads the next two pages.
	/// </summary>
	public IEnumerator TurnPageLeft(){
		yield return new WaitWhile (() => loadingLeft);
		loadingLeft = true;
		for (int i = 0; i < 4; i++) {
			pages [i].enabled = true;
			pages[i].material.mainTexture = pages[i + 2].material.mainTexture;
		}
		curr++;
		annotation [0].UpdateWebAddress (iiifImage.removeTail(data.getPage(curr*2 - 1)));
		annotation [1].UpdateWebAddress (iiifImage.removeTail(data.getPage(curr*2)));
		UpdateAnnotations ();
		pages [4].material.mainTexture = loadingTexture;
		pages [5].material.mainTexture = loadingTexture;
		UpdatePageDisplay ();
		yield return StartCoroutine(InitPage (4));
		yield return StartCoroutine(InitPage (5));
		loadingLeft = false;
	}

	/// <summary>
	/// Shifts page's textures to the right and loads the previous two pages.
	/// </summary>
	public IEnumerator TurnPageRight(){
		yield return new WaitWhile (() => loadingRight);
		loadingRight = true;
		for (int i = 5; i > 1; i--) {
			pages [i].enabled = true;
			pages[i].material.mainTexture = pages[i - 2].material.mainTexture;
		}
		curr--;
		annotation [0].UpdateWebAddress (iiifImage.removeTail(data.getPage(curr*2 - 1)));
		annotation [1].UpdateWebAddress (iiifImage.removeTail(data.getPage(curr*2)));
		UpdateAnnotations ();
		pages [0].material.mainTexture = loadingTexture;
		pages [1].material.mainTexture = loadingTexture;
		UpdatePageDisplay ();
		yield return StartCoroutine(InitPage (0));
		yield return StartCoroutine(InitPage (1));
		loadingRight = false;
	}

	/// <summary>
	/// Determines whether this instance is loading left pages.
	/// </summary>
	/// <returns><c>true</c> if this instance is loading left pages; otherwise, <c>false</c>.</returns>
	public bool IsLoadingLeft(){return loadingLeft;}

	/// <summary>
	/// Determines whether this instance is loading right pages.
	/// </summary>
	/// <returns><c>true</c> if this instance is loading right pages; otherwise, <c>false</c>.</returns>
	public bool IsLoadingRight(){return loadingRight;}

	/// <summary>
	/// Shows/Hides the annotations.
	/// </summary>
	/// <param name="isShowing">If set to <c>true</c> shows annotations. Otherwise, hides annotations.</param>
	public void ShowAnnotations(bool isShowing){
		foreach (AnnotationDrawer d in drawers) {
			d.ShowAnnotations (isShowing);
			d.enabled = isShowing;
		}
	}

	/// <summary>
	/// Updates the annotations that are being drawn.
	/// </summary>
	public void UpdateAnnotations(){
		if (curr != 0)
		    for (int i = 0; i < drawers.Length; i++) {
			    drawers [i].UpdatesAnnotations (GetAnnotations (i));
		    }
		else
			drawers [0].UpdatesAnnotations (GetAnnotations (0));

		Annotation.AnnotationBox empty;

		empty.contents = "";
		empty.x = 0;
		empty.y = 0; 
		empty.w = 0;
		empty.h = 0;

		annotations = annotation[0].GetAnnotations (transcription, annotation [0].webAddress);
		if (annotations.Count > 0)
			leftTrans.UpdatesTranscriptions ((Annotation.AnnotationBox)annotations [0]);
		else
			leftTrans.UpdatesTranscriptions (empty);
		
		annotations = annotation[1].GetAnnotations (transcription, annotation [1].webAddress);
		if (annotations.Count > 0) 
			rightTrans.UpdatesTranscriptions ((Annotation.AnnotationBox)annotations [0]);
		else
			rightTrans.UpdatesTranscriptions (empty);
	}

	/// <summary>
	/// Gets the annotations for a specified page.
	/// </summary>
	/// <returns>The annotations for the page.</returns>
	/// <param name="which">Which page to get the annotations for (0 for left, 1 for right).</param>
	public Annotation.AnnotationBox[] GetAnnotations(int which){
		if (File.Exists(annotation[which].LocalAnnotationFile()))
			annotations = annotation[which].GetAnnotations (File.ReadAllText(annotation[which].LocalAnnotationFile()), annotation[which].webAddress);
		Annotation.AnnotationBox[] output = new Annotation.AnnotationBox[annotations.Count];
		for (int i = 0; i < output.Length; i++)
			output [i] = (Annotation.AnnotationBox)annotations [i];
		return output;
	}

	private IEnumerator InitPage(int page)
	{
		int pageNum = curr * 2 - 3 + page;
		if (pageNum > 0 && pageNum < data.getNumOfPages ()) {
			pages [page].enabled = true;
			if (pageNum % 2 == 1) {
				iiifImage.cropOffsetX = 175;
			} else {
				iiifImage.cropOffsetX = 60;
			}
			iiifImage.changeAddress (data.getPage (pageNum));
			yield return StartCoroutine (iiifImage.UpdateImage ());
			pages [page].material.mainTexture = iiifImage.texture;
		} else {
			pages [page].enabled = false;
		}
	}

	private void UpdatePageDisplay(){
		pageDisplay.text = (curr - 3).ToString () + "v : " + (curr - 2).ToString () + "r";
	}
}
