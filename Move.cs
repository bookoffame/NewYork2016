using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class Move : MonoBehaviour {
	public Transform myTransform;
	public float speed;
	public IIIFImageGet image;
	public string manifestURL;
	private IIIFGetManifest data;
	private bool on;
	// Update is called once per frame

	void Start()
	{
		data = new IIIFGetManifest ();
		data.download(manifestURL);
		image.changeAddress(data.getPage (0));
		image.UpdateImage ();
		on = false;
	}

	void Update () {
		if (on) {
			myTransform.Translate (speed * Input.GetAxis ("Horizontal"), speed * Input.GetAxis ("Vertical"), speed * Input.GetAxis ("Forward"));
			if (Input.GetKeyDown (KeyCode.M)) {
				image.changeAddress (data.getNextPage ());
				image.UpdateImage ();
			} else if (Input.GetKeyDown (KeyCode.N)) {
				image.changeAddress (data.getPrevPage ());
				image.UpdateImage ();
			}
		}
	}

	public void setActivated(bool isActivated){
		on = isActivated;
	}
}
