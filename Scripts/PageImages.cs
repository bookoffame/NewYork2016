using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using AssemblyCSharp;

public class PageImages : MonoBehaviour {
	public Renderer[] pages;
	public IIIFImageGet iiifImage;
	public string manifestURL;
	public Texture2D loadingTexture;

	public Annotation[] annotation;
	public AnnotationDrawer[] drawers;
	public TranscriptionTool leftTrans, rightTrans;
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

	public bool IsLoadingLeft(){return loadingLeft;}
	public bool IsLoadingRight(){return loadingRight;}

	public void ShowAnnotations(bool isShowing){
		foreach (AnnotationDrawer d in drawers) {
			d.ShowAnnotations (isShowing);
			d.enabled = isShowing;
		}
	}

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
