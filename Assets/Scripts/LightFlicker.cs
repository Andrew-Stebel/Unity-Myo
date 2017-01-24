using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour 
{

	public Light Light;

	void FixedUpdate()
	{
		if (Random.value <= .2) 
		{
			Light.enabled = true;
		}
		else
			Light.enabled = false;
	}

}
