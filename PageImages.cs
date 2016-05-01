using UnityEngine;
using System.Collections;
using System.IO;
using AssemblyCSharp;

public class PageImages : MonoBehaviour {
	public Renderer[] pages;
	public IIIFImageGet iiifImage;
	public string manifestURL;

	public Annotation[] annotation;
	public AnnotationDrawer[] drawers;

	private ArrayList annotations;
	private IIIFGetManifest data;
	private int curr;

	// Use this for initialization
	void Start () {
		annotations = new ArrayList ();
		data = new IIIFGetManifest ();
		data.download(manifestURL);
		StartCoroutine(init ());
	}

	private IEnumerator init()
	{
		for (int i = 3; i < 6; i++)
			yield return StartCoroutine(InitPage (i, i - 3));
		curr = 0;
		annotation [0].UpdateWebAddress (iiifImage.removeTail(data.getPage(curr*2)));
	}
	public IEnumerator TurnPageLeft(){
		for (int i = 0; i < 4; i++) {
			pages[i].material.mainTexture = pages[i + 2].material.mainTexture;
		}
		curr++;
		annotation [0].UpdateWebAddress (iiifImage.removeTail(data.getPage(curr*2)));
		annotation [1].UpdateWebAddress (iiifImage.removeTail(data.getPage(curr*2 + 1)));
		yield return StartCoroutine(InitPage (4, curr*6 + 1));
		yield return StartCoroutine(InitPage (5, curr*6 + 2));
	}

	public IEnumerator TurnPageRight(){
		for (int i = 5; i > 1; i--) {
			pages[i].material.mainTexture = pages[i - 2].material.mainTexture;
		}
		curr--;
		yield return StartCoroutine(InitPage (0, curr*6 - 2));
		yield return StartCoroutine(InitPage (1, curr*6 - 1));
	}
	public void UpdateAnnotations(){
		if (File.Exists(annotation[0].LocalAnnotationFile()))
			annotations = annotation[0].GetAnnotations (File.ReadAllText(annotation[0].LocalAnnotationFile()), iiifImage.webAddress);
		for (int i = 0; i < drawers.Length; i++)
			drawers[i].UpdatesAnnotations (GetAnnotations (iiifImage.removeTail(data.getPage(curr + i))));
	}

	public Annotation.AnnotationBox[] GetAnnotations(string weburl){
		if (annotations == null)
			annotations = new ArrayList ();
		Annotation.AnnotationBox[] output = new Annotation.AnnotationBox[annotations.Count];
		for (int i = 0; i < output.Length; i++)
			output [i] = (Annotation.AnnotationBox)annotations [i];
		return output;
	}

	private IEnumerator InitPage(int page, int pageNum)
	{
		iiifImage.changeAddress(data.getPage (pageNum));
		yield return StartCoroutine(iiifImage.UpdateImage ());
		pages[page].material.mainTexture = iiifImage.texture;
	}
}
