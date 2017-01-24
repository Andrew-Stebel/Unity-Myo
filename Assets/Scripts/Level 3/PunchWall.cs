using UnityEngine;
using System.Collections;

public class PunchWall : MonoBehaviour 
{
	private GameObject _player;
	public GameObject wall;
	float _distToOpen = 3f;
	
	private bool punched = false;

	void Awake()
	{
		_player = GameObject.FindWithTag ("Player");
	}
	
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.E)
			&& Vector3.Distance (_player.transform.position, wall.transform.position) <= _distToOpen) 
		{
			if (!punched)
			{
				wall.GetComponent<Rigidbody> ().isKinematic = false;
				wall.GetComponent<Rigidbody> ().useGravity = true;
				wall.GetComponent<Rigidbody> ().AddForce ( (transform.forward * 500) + transform.up * 500 );

				punched = true;

				StartCoroutine(DestroyWall());

			}
		}
	}

	IEnumerator DestroyWall()
	{
		yield return new WaitForSeconds(5);
		Destroy (wall);
	}
}
