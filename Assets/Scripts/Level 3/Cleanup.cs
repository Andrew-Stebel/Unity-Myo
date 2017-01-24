using UnityEngine;
using System.Collections;

public class Cleanup : MonoBehaviour 
{

	void OnTriggerEnter(Collider other)
	{
		//destroy the remnants of the floor
		if (other.tag == "TrapFloor") 
		{
			Destroy(other.gameObject);
		}
	}
}
