using UnityEngine;
using System.Collections;

public class PlayerDrown : MonoBehaviour 
{
	public GameObject boat;

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			other.gameObject.transform.parent = boat.transform;
			other.gameObject.transform.position = boat.transform.position;
		}
	}

}
