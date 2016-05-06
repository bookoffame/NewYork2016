using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {

	public Rigidbody player;
	bool inControl = true;
	public float speed;

	// Update is called once per frame
	void Update () {
		if (inControl)
		    player.velocity = new Vector3 (speed*Input.GetAxis("Horizontal"), player.velocity.y, speed*Input.GetAxis("Vertical"));
	}

	public void ChangeControl(bool newControl){
		inControl = newControl;
	}
}
