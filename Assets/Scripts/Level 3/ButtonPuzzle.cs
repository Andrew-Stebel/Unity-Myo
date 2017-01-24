using UnityEngine;
using System.Collections;

public class ButtonPuzzle : MonoBehaviour 
{
	bool playerOn = false;
	bool cubeOn = false;

	public GameObject laserFence;
	public bool buttonDown = false;

	void OnTriggerEnter(Collider other)
	{
		if (this.transform.parent.tag == "Light")
		{
			LightEnter(other);
		}
		if (this.transform.parent.tag == "Medium")
		{
			MediumEnter(other);
		}

		if (this.transform.parent.tag == "Heavy")
		{
			HeavyEnter(other);
		}
	}

	void LightEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			playerOn = true;
			
			if (!cubeOn)
			{
				playAnim_PlatformOn();
				buttonDown = true;
			}
		}
		if (other.tag == "Weight" && other.GetComponent<MeshCollider>().Equals(other))
		{
			cubeOn = true;
			if (!playerOn)
			{
				playAnim_PlatformOn();
				buttonDown = true;
			}
		}

		if (other.tag == "Block" && other.GetComponent<BoxCollider>().Equals(other)) 
		{
			playAnim_PlatformOn();
			buttonDown = true;
		}
	}

	void MediumEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			playAnim_PlatformOn();
			buttonDown = true;
		}
	}

	void HeavyEnter(Collider other)
	{
		if (other.tag == "Block" && other.GetComponent<BoxCollider>().Equals(other)) 
		{
			playAnim_PlatformOn();
			buttonDown = true;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (this.transform.parent.tag == "Light") {
			LightExit (other);
		}
		if (this.transform.parent.tag == "Medium") {
			MediumExit (other);
		}
		if (this.transform.parent.tag == "Heavy") {
			HeavyExit (other);
		}
	}

	void LightExit(Collider other)
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
			buttonDown = false;
		}
	}

	void MediumExit(Collider other)
	{
		if (other.tag == "Player") 
		{
			playerOn = false;
			playAnim_PlateformOff();
			buttonDown = false;
		}
	}

	void HeavyExit(Collider other)
	{
		if (other.tag == "Block") 
		{
			playAnim_PlateformOff();
			buttonDown = false;
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
