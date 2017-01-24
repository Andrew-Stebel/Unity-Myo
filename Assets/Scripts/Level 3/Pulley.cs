using UnityEngine;
using System.Collections;

public class Pulley : MonoBehaviour 
{
	private GameObject _player;
	Vector3 originalPosition;
	void Awake()
	{
		_player = GameObject.FindWithTag("Player");
		originalPosition = lift.transform.position;
	}


	public GameObject rope;
	public GameObject lift;
	private float _interactDist = 3f;

	bool alternate = false;
	bool liftAtBase = true;

	void Update()
	{
		//are they close to the rope?
		if (Vector3.Distance (_player.transform.position, lift.transform.position) <= _interactDist) 
		{
			//is it the first pull
			if (liftAtBase)
			{
				DetermineFirstArm();
			}
			//in the middle of the lift
			else
			{
				AlternateArms();
			}

			//have they let go of the rope?
			LetGo();

		} 
		//noone nearby, lift is going down
		else 
		{
			lift.transform.position = Vector3.Lerp (lift.transform.position, originalPosition, Time.deltaTime * 0.3f);
		}

		//verify lift is back at base
		if (lift.transform.position == originalPosition)
			liftAtBase = true;

	}

	void PositionChanging()
	{
		//only increment if it's not at the top
		if (Vector3.Distance(lift.transform.position, new Vector3(lift.transform.position.x, 18.8f, lift.transform.position.z)) > 1.0f)
		{
			Vector3 startPosition = lift.transform.position;
			Vector3 newPosition = new Vector3 (lift.transform.position.x, lift.transform.position.y + 2, lift.transform.position.z);
			lift.transform.position = Vector3.Lerp (startPosition, newPosition, 1.0f);
		}
	}



	void DetermineFirstArm()
	{
		if (Input.GetButtonDown ("RightArm"))
		{
			alternate = true;
			liftAtBase = false;
			PositionChanging ();
		}
		if (Input.GetButtonDown ("LeftArm"))
		{
			alternate = false;
			liftAtBase = false;
			PositionChanging ();
		}
	}

	void AlternateArms()
	{
		if (Input.GetButtonDown ("RightArm") && !alternate) 
		{
			PositionChanging ();
			alternate = true;
		}
		if (Input.GetButtonDown ("LeftArm") && alternate) 
		{
			PositionChanging ();
			alternate = false;
		}
	}

	void LetGo()
	{
		if (!Input.GetButton ("RightArm") && !Input.GetButton ("LeftArm"))
		{
			Vector3 startPosition = lift.transform.position;
			lift.transform.position = Vector3.MoveTowards(startPosition, originalPosition, Time.deltaTime*5);
			alternate = false;
		}
	}
}
