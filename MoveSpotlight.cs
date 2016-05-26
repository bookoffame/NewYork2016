using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveSpotlight : MonoBehaviour {
	public float minX,minY,maxX,maxY;
	public Camera cam;
	public Light spotLight, worldLight, ceilingLight;
	public Image preview;
	public Toggle frozenToggle, hideToggle;
	public bool frozen = false;
	public bool hidingProperties = false;
	public Transform properties;

	// Update is called once per frame
	void Update () {
		bool isSpotlight = ButtonControls.current.isSpotlight;
		spotLight.enabled = isSpotlight;
		worldLight.enabled = !isSpotlight && !ceilingLight.enabled;
		if (Input.GetKeyDown (KeyCode.F))
			freezePosition (!frozen);
		if (Input.GetKeyDown (KeyCode.H))
			hideProperties (!hidingProperties);
		if (Input.GetKeyDown (KeyCode.C))
			ceilingLight.enabled = !ceilingLight.enabled;
		if (!frozen) {
			Vector3 pos = new Vector3 ();
			Vector3 mousePos = Input.mousePosition;
			pos.x = (maxX - minX) * (mousePos.x / Screen.width) + minX + cam.transform.position.x;
			pos.y = (maxY - minY) * (mousePos.y / Screen.height) + minY + cam.transform.position.y;
			pos.z = transform.position.z;
			transform.position = Vector3.MoveTowards (transform.position, pos, 1f);
		}
	}

	public void updateHue(float hue){
		float h,s,v;
		Color.RGBToHSV (spotLight.color, out h, out s, out v);
		Color newColor = Color.HSVToRGB (hue, s, v);
		spotLight.color = newColor;
		preview.color = newColor;
	}

	public void updateSat(float sat){
		float h,s,v;
		Color.RGBToHSV (spotLight.color, out h, out s, out v);
		Color newColor = Color.HSVToRGB (h, sat, v);
		spotLight.color = newColor;
		preview.color = newColor;
	}

	public void updateValue(float value){
		float h,s,v;
		Color.RGBToHSV (spotLight.color, out h, out s, out v);
		Color newColor = Color.HSVToRGB (h, s, value);
		spotLight.color = newColor;
		preview.color = newColor;
	}

	public void updateSize(float newSize){
		spotLight.spotAngle = newSize * 180;
	}

	public void updateBrightness(float brightness){
		spotLight.intensity = brightness * 8;
	}

	public void freezePosition(bool isFreeze){
		frozen = isFreeze;
		frozenToggle.isOn = isFreeze;
	}

	public void hideProperties(bool isHiding){
		hidingProperties = isHiding;
		hideToggle.isOn = isHiding;
		properties.gameObject.SetActive(!isHiding);
	}
}
