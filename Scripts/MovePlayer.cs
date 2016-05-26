using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {

	public MonoBehaviour moveScript;
	public bool inControl;

	void Start()
	{
		moveScript.enabled = inControl;
	}

	public void ChangeControl(bool newControl){
		moveScript.enabled = newControl;
		inControl = newControl;
	}
}
