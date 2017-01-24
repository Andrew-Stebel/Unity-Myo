using UnityEngine;
using System.Collections;

public class EnableFence : MonoBehaviour 
{
	public GameObject button1;
	public GameObject button2;
	public GameObject button3;


	void Update()
	{
		if (button1.GetComponent<ButtonPuzzle> ().buttonDown
			&& button2.GetComponent<ButtonPuzzle> ().buttonDown
			&& button3.GetComponent<ButtonPuzzle> ().buttonDown) 
		{
			this.gameObject.SetActive(false);
		}
	}
}
