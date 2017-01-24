using UnityEngine;
using System.Collections;

public class SystemCode : MonoBehaviour 
{
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (this.tag == "Respawn")
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		if (this.tag == "Finish") 
		{
			if (Application.loadedLevelName == "Level1")
			{
				Application.LoadLevel("Level2");
			}

			if (Application.loadedLevelName == "Level2")
			{
				Application.LoadLevel("Level3");
			}
			if (Application.loadedLevelName == "Level3")
			{
				Application.LoadLevel("Level1");
			}
		}
		if (this.tag == "Lift") 
		{
			other.gameObject.transform.parent = this.transform;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (this.tag == "Lift") {
			other.gameObject.transform.parent = null;
		}
	}
}
