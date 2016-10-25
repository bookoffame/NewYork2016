using UnityEngine;
using System.Collections;

public class InventoryBox : MonoBehaviour
{
	/// <summary>
	/// Makes the InventoryBox appear.
	/// </summary>
	public void Show(){
		gameObject.SetActive (true);
	}

	/// <summary>
	/// Action to perform when closed is clicked.
	/// </summary>
	public void OnClose ()
	{
		gameObject.SetActive (false);
	}
}

