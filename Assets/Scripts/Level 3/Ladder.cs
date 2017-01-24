using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour 
{
	private GameObject _player;
	bool canClimb = false;
	public float heightFactor = 3.2f;

	void Awake () 
	{
		_player = GameObject.FindWithTag ("Player");
	}
	
	void OnTriggerEnter (Collider coll)
	{
		if(coll.gameObject == _player)
		{
			canClimb = true;
		}
	}
	
	void OnTriggerExit (Collider coll)
	{
		if(coll.gameObject == _player)
		{
			canClimb = false;
		}
	}

	void FixedUpdate () 
	{
		if(canClimb)
		{
			if(Input.GetKey(KeyCode.W))
			{
				_player.transform.position += (Vector3.up);
			}
		}
	}

}
