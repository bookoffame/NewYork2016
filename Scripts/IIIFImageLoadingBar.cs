using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IIIFImageLoadingBar : MonoBehaviour {

	public IIIFImageGet image;
	public Image back;
	public Image progressBar;

	void Update(){
		if (image.GetProgress () < 0.9999) {
			progressBar.color = Color.green;
			back.color = Color.black;
		} else {
			progressBar.color = Color.clear;
			back.color = Color.clear;
		}
		progressBar.fillAmount = image.GetProgress();
	}
}
