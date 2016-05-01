using UnityEngine;
using System.Collections;
using System.IO;

public class IIIFImageGet : MonoBehaviour {
	public string webAddress;
	public int cropOffsetX, cropOffsetY, cropWidth, cropHeight = -1;
	public int targetWidth, targetHeight = -1;
	public bool mirrored = false;
	public int rotation = 0;
	public string quality = "default";
	public string format = ".jpg";
	public Texture2D texture;

	public IEnumerator UpdateImage () {
		string location = getAddress ();
		WWW iiifImage = new WWW (location);
		yield return new WaitUntil(() => iiifImage.isDone);
		texture = iiifImage.texture;
	}

	public string removeTail(string newAddress){
		int remaining = 4;
		int index = newAddress.Length - 1;
		while (remaining > 0) {
			if (newAddress [index] == '/')
				remaining--;
			index--;
		}
		return newAddress.Substring (0,index + 1);
	}

	public void changeAddress(string newAddress){
		webAddress = removeTail (newAddress);
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
		return location;
	}

}
