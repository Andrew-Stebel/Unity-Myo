using UnityEngine;
using System.Collections;

public class WallWind : MonoBehaviour 
{

	private GameObject _player;
	void Awake()
	{
		_player = GameObject.Find ("Player");
	}

	private bool grounded = true;
	
	void OnTriggerEnter(Collider c) 
	{
		if (c.tag == "Player") 
		{
			grounded = false;
		}
	}

	void OnTriggerExit(Collider c)
	{
		WindEffects.floating = false;
		grounded = true;
	}

	void FixedUpdate()
	{
		if (!grounded)
		{
			_player.transform.position = new Vector3 (_player.transform.position.x,
			                                          _player.transform.position.y,
			                                          _player.transform.position.z+0.1f);
		}
	}

}
