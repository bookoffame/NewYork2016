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

	public Renderer backLeftPage, backRightPage;
	public Renderer leftPage, rightPage;

	public ImageBufferer buffer;

	private IIIFImageGet iiifImage;
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

	/// <summary>
	/// The rotation required for each page.
	/// </summary>
	public int[] rotations;

	private ArrayList annotations;
	private int curr;
	private bool loadingRight, loadingLeft;
	private string transcription;

	// Use this for initialization
	void Start () {
		annotations = new ArrayList ();
		iiifImage = ScriptableObject.CreateInstance<IIIFImageGet>();
		StartCoroutine(init ());
	}

	private IEnumerator init()
	{
		for (int i = 0; i < 6; i++) {
			pages [i].enabled = true;
			pages [i].material.mainTexture = buffer.GetImage(i);
		}
		curr = 73;
		annotation [0].UpdateWebAddress (iiifImage.removeTail(buffer.GetURL(curr*2 - 1)));
		annotation [1].UpdateWebAddress (iiifImage.removeTail(buffer.GetURL(curr*2)));
		transcription = Resources.Load<TextAsset> ("Transcriptions/anno").text;
		UpdateAnnotations ();
		UpdatePageDisplay ();
		loadingLeft = false;
		loadingRight = false;
		yield return new WaitUntil(()=>true);
	}

	void Update(){
		/*for (int i = 0; i < 6; i++) {
			pages [i].material.mainTexture = buffer.GetImage(i);
		}*/
		backLeftPage.material.mainTexture = buffer.GetDualTexture(curr*2 - 3,curr*2 - 3);
		backRightPage.material.mainTexture = buffer.GetDualTexture(curr*2 + 2,curr*2 + 2);
		leftPage.material.mainTexture = buffer.GetDualTexture (curr*2 - 1,curr*2 - 2);
		rightPage.material.mainTexture = buffer.GetDualTexture (curr*2,curr*2 + 1);
	}

	/// <summary>
	/// Shifts page's textures to the left and loads the next two pages.
	/// </summary>
	public IEnumerator TurnPageLeft(){
		buffer.TurnPageLeft ();
		curr++;
		annotation [0].UpdateWebAddress (iiifImage.removeTail(buffer.GetURL(curr*2 - 1)));
		annotation [1].UpdateWebAddress (iiifImage.removeTail(buffer.GetURL(curr*2)));
		UpdateAnnotations ();
		UpdatePageDisplay ();
		yield return new WaitUntil (() => true);
	}

	/// <summary>
	/// Shifts page's textures to the right and loads the previous two pages.
	/// </summary>
	public IEnumerator TurnPageRight(){
		buffer.TurnPageRight ();
		curr--;
		annotation [0].UpdateWebAddress (iiifImage.removeTail(buffer.GetURL(curr*2 - 1)));
		annotation [1].UpdateWebAddress (iiifImage.removeTail(buffer.GetURL(curr*2)));
		UpdateAnnotations ();
		UpdatePageDisplay ();
		yield return new WaitUntil (() => true);
	}

	/// <summary>
	/// Determines whether this instance is loading left pages.
	/// </summary>
	/// <returns><c>true</c> if this instance is loading left pages; otherwise, <c>false</c>.</returns>
	public bool IsLoadingLeft(){return false;}

	/// <summary>
	/// Determines whether this instance is loading right pages.
	/// </summary>
	/// <returns><c>true</c> if this instance is loading right pages; otherwise, <c>false</c>.</returns>
	public bool IsLoadingRight(){return false;}

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
		leftTrans.UpdatesTranscriptions (GetAnnotationsBoxArray());
		
		annotations = annotation[1].GetAnnotations (transcription, annotation [1].webAddress);
		rightTrans.UpdatesTranscriptions (GetAnnotationsBoxArray());
	}

	public void AddNewAnnotation(int page, Annotation.AnnotationBox anno){
		ButtonControls.current.changeSelected (ButtonControls.READER_TOOL);
		drawers [page].AddNewAnnotation (anno);
	}

	/// <summary>
	/// Gets the annotations for a specified page.
	/// </summary>
	/// <returns>The annotations for the page.</returns>
	/// <param name="which">Which page to get the annotations for (0 for left, 1 for right).</param>
	public Annotation.AnnotationBox[] GetAnnotations(int which){
		if (File.Exists(annotation[which].LocalAnnotationFile()))
			annotations = annotation[which].GetAnnotations (File.ReadAllText(annotation[which].LocalAnnotationFile()), annotation[which].webAddress);
		return GetAnnotationsBoxArray ();
	}

	private Annotation.AnnotationBox[] GetAnnotationsBoxArray(){
		Annotation.AnnotationBox[] output = new Annotation.AnnotationBox[annotations.Count];
		for (int i = 0; i < output.Length; i++)
			output [i] = (Annotation.AnnotationBox)annotations [i];
		return output;
	}

	private void UpdatePageDisplay(){
		pageDisplay.text = (curr - 3).ToString () + "v : " + (curr - 2).ToString () + "r";
	}
}
