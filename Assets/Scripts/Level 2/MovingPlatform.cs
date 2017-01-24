using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour 
{
	public GameObject plateform;

	private bool isMoving = false;

	void Update()
	{
		if (isMoving == true) 
		{
			plateform.GetComponent<Animation> ().Play ("PlatformMove");
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			isMoving = true;
		}
		other.transform.parent = plateform.transform;
	}

	void OnTriggerExit(Collider other)
	{
		other.transform.parent = null;
	}
}
