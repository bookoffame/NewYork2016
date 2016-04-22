using UnityEngine;
using System.Collections;

public class IIIFImageGet : MonoBehaviour {
	public Transform location;
	public string webAddress;
	public int cropOffsetX, cropOffsetY, cropWidth, cropHeight = -1;
	public int targetWidth, targetHeight = -1;
	public bool mirrored = false;
	public int rotation = 0;
	public string quality = "default";
	public string format = ".jpg";
	//public string myName;
	public Renderer myRenderer;

	// Use this for initialization
	public void UpdateImage () {
		string location = getAddress ();
		WWW iiifImage = new WWW (location);
		while (!iiifImage.isDone);
		/*
		GameObject newSprite = AddSprite(iiifImage.texture);
		newSprite.transform.position = location.position;*/
		myRenderer.material.mainTexture = iiifImage.texture;

	}

	public void changeAddress(string newAddress){
		int remaining = 4;
		int index = newAddress.Length - 1;
		while (remaining > 0) {
			if (newAddress [index] == '/')
				remaining--;
			index--;
		}
		webAddress = newAddress.Substring (0,index + 1);
	}
	public string getAddress(){
		string location = webAddress;
		location = location.Insert (location.Length,"/");
		if (cropOffsetX == -1)
			location = location.Insert (location.Length,"full/");
		else
			location = location.Insert (location.Length, cropOffsetX.ToString () + ","
				+ cropOffsetY.ToString () + "," + cropWidth.ToString () + "," + cropHeight.ToString () + "/");
		if (targetWidth == -1 && targetHeight == -1)
			location = location.Insert (location.Length, "full/");
		else {
			if (targetWidth != -1)
				location = location.Insert (location.Length,targetWidth.ToString());
			location = location.Insert (location.Length,",");
			if (targetHeight != -1)
				location = location.Insert (location.Length,targetHeight.ToString());
			location = location.Insert (location.Length,"/");
		}
		if (mirrored)
			location = location.Insert (location.Length,"!");
		location = location.Insert (location.Length,rotation.ToString() + "/");
		location = location.Insert (location.Length, quality + format);
		Debug.Log (location);
		return location;
	}

	/*
	public GameObject AddSprite (Texture2D tex) {
		Texture2D _texture = tex;
		Sprite newSprite = Sprite.Create(_texture, new Rect(0f, 0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f),128f);
		GameObject sprGameObj = new GameObject();
		sprGameObj.name = myName;
		sprGameObj.AddComponent<SpriteRenderer>();
		SpriteRenderer sprRenderer = sprGameObj.GetComponent<SpriteRenderer>();
		sprRenderer.sprite = newSprite;
		sprRenderer.
		return sprGameObj;
	}*/


}
