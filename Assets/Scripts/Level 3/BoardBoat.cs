using UnityEngine;
using System.Collections;

public class BoardBoat : MonoBehaviour
{
	private GameObject _player;

	public GameObject boat;
	public GameObject boatSeat;

	public GameObject boardingPoint;
	bool onBoard = false;

	void Awake ()
	{
		_player = GameObject.FindWithTag ("Player");
	}

	void OnTriggerEnter(Collider other)
	{
		if (!onBoard)
		{
			onBoard = true;
			_player.transform.parent = boatSeat.transform;
			_player.transform.position = new Vector3 (boatSeat.transform.position.x,
			                                          boatSeat.transform.position.y+2f,
			                                          boatSeat.transform.position.z);
		}
		else
		{
			onBoard = false;
			_player.transform.parent = null;
			_player.transform.position = boardingPoint.transform.position;
		}
	}
}
