using UnityEngine;
using System.Collections;

public class JumpReset : MonoBehaviour 
{
	public GameObject platform;

	void OnTriggerEnter(Collider other)
	{
		other.transform.parent = platform.transform;
		other.transform.position = new Vector3 (platform.transform.position.x,
		                                        platform.transform.position.y+1f,
		                                        platform.transform.position.z);
	}
}
