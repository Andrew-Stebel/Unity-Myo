using UnityEngine;
using System.Collections;

public class BoatMechanics : MonoBehaviour
{
	public static bool onBoat = false;
	public GameObject boat;
	bool rowed = false;

	public GameObject endPosit;
	public GameObject startPosit;

	bool origPosit = true;

	float timer;

	void OnTriggerStay(Collider other)
	{
		onBoat = true;

	}
	
	void OnTriggerExit(Collider other)
	{
		onBoat = false;
	}

	void Update()
	{
		if (onBoat)
		{
			BoatRowing();
		}
	}

	void BoatRowing()
	{
		if (Input.GetButton ("LeftArm") && Input.GetButton ("RightArm") 
		    && !rowed && DistanceCheck())
		{
			rowed = true;
			Vector3 startPosition = boat.transform.position;
			//going further
			if (origPosit)
			{
				Vector3 nextPosition = new Vector3(boat.transform.position.x,
				                                   boat.transform.position.y,
				                                   boat.transform.position.z+2f);
				boat.transform.position = Vector3.Lerp (startPosition, nextPosition, Time.time);
			}

			//going back
			else
			{
				Vector3 nextPosition = new Vector3(boat.transform.position.x,
				                                   boat.transform.position.y,
				                                   boat.transform.position.z-2f);
				boat.transform.position = Vector3.Lerp (startPosition, nextPosition, Time.time);
			}

		}
		if (Input.GetButtonUp("LeftArm") && Input.GetButtonUp("RightArm"))
		{
			rowed = false;
		}
	}

	public bool DistanceCheck()
	{
		if (origPosit)
		{
			if (Vector3.Distance (boat.transform.position, endPosit.transform.position) <= 4f)
			{
				origPosit = false;
				return false;
			}
		}
		else
		{
			if (Vector3.Distance (boat.transform.position, startPosit.transform.position) <= 4f)
			{
				origPosit = true;
				return false;
			}
		}
		return true;
	}

}
