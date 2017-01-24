using UnityEngine;
using System.Collections;

public class WallPlateform : MonoBehaviour 
{
	public GameObject wall;

	bool playerOn = false;
	bool cubeOn = false;


	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			playerOn = true;
			
			if (!cubeOn)
			{
				playAnim_PlatformOn();
				wall.SetActive(false);
			}
		}
		if (other.tag == "Weight" && !other.isTrigger) 
		{
			cubeOn = true;
			if (!playerOn && other.gameObject.GetComponent<Rigidbody>().useGravity == true)
			{
				playAnim_PlatformOn();
				wall.SetActive(false);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player") 
		{
			playerOn = false;
		}
		if (other.tag == "Weight") 
		{
			cubeOn = false;
		}
		
		if (!playerOn && !cubeOn) 
		{
			playAnim_PlateformOff();
			wall.SetActive(true);
		}
	}
	
	void playAnim_PlatformOn()
	{
		this.GetComponent<Animation> () ["WeightedPlatform"].speed = 1;
		this.GetComponent<Animation> () ["WeightedPlatform"].time = 0;
		this.GetComponent<Animation> ().Play ("WeightedPlatform");
	}
	
	void playAnim_PlateformOff()
	{
		this.GetComponent<Animation> () ["WeightedPlatform"].speed = -1;
		this.GetComponent<Animation> () ["WeightedPlatform"].time = this.GetComponent<Animation> () ["WeightedPlatform"].length;
		this.GetComponent<Animation> ().Play ("WeightedPlatform");
	}
}
