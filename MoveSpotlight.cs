using UnityEngine;
using System.Collections;

public class MoveSpotlight : MonoBehaviour {
	public float minX,minY,maxX,maxY;
	public Camera cam;
	public Light spotLight, worldLight;

	// Update is called once per frame
	void Update () {
		bool isSpotlight = ButtonControls.current.isSpotlight;
		spotLight.enabled = isSpotlight;
		worldLight.enabled = !isSpotlight;
		Vector3 pos = new Vector3 ();
		Vector3 mousePos = Input.mousePosition;
		pos.x = (maxX - minX) * (mousePos.x / Screen.width) + minX + cam.transform.position.x;
		pos.y = (maxY - minY)*(mousePos.y/Screen.height) + minY + cam.transform.position.y;
		pos.z = transform.position.z;
		transform.position = Vector3.MoveTowards(transform.position, pos, 1f);
	}
}
