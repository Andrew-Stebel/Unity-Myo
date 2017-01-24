using UnityEngine;
using System.Collections;

public class Effects : MonoBehaviour 
{
	public GameObject gate;
	float x,y,z;

	void Awake()
	{
		x = Random.value;
		y = Random.value;
		z = Random.value;
	}
	void Update()
	{
		gate.transform.Rotate(x,y,z);
	}
}
