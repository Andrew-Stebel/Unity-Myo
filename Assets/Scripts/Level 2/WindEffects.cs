using UnityEngine;
using System.Collections;

public class WindEffects : MonoBehaviour 
{
	private GameObject _player;
	void Awake()
	{
		_player = GameObject.Find ("Player");
	}
	
	public static float amplitude=1;
	
	public static bool floating = false;
	float newY;
	float timer;
	
	void OnTriggerEnter(Collider c)
	{
		floating = true;
		timer = 0.0f;
		newY = _player.transform.position.y;
	}
	void OnTriggerExit(Collider c)
	{
		floating = false;
	}

	void OnTriggerStay(Collider c)
	{
		floating = true;
	}
	void FixedUpdate () 
	{
		if (floating)
		{
			timer += Time.deltaTime;
			
			//Float up and down along the y axis, 
			_player.transform.position = new Vector3 (_player.transform.position.x,
			                                          newY + amplitude * Mathf.Sin (1 * timer),
			                                          _player.transform.position.z);
		}
		
	}
}