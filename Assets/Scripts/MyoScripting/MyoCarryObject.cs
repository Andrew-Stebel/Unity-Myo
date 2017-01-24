using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MyoCarryObject : MyoObjectListener
{
	//used to trace movement of carried object
	GameObject mainCamera;

	//properties of the held object
	public float distance;
	public float smooth;
	public bool pickedUp = false;

	//gesture to listen for
	public MyoInteractibles.Gesture triggerGesture = MyoInteractibles.Gesture.POSE_FIST;

	void Start () 
	{
		mainCamera = GameObject.FindWithTag ("MainCamera");
	}
	
	void OnEnable()
	{
		MyoInteractibles.OnGestureStart += OnGestureStart;
		MyoInteractibles.OnGestureEnd += OnGestureEnd;
	}
	
	
	void OnDisable()
	{
		MyoInteractibles.OnGestureStart -= OnGestureStart;
		MyoInteractibles.OnGestureEnd -= OnGestureEnd;
	}
	
	//called when the gesture begins (fist)
	void OnGestureStart(GestureEvent gestureEvent)
	{
		if (!Target)
		{
			pickedUp = false;
			return;
		}

		//begin picked up state
		if (gestureEvent.mGesture == triggerGesture)
		{
			pickedUp = true;
		}
	}

	//called when gesture ends (no fist)
	void OnGestureEnd(GestureEvent gestureEvent)
	{
		if (Target == null)
		{
			pickedUp = false;
			return;
		}

		//end picked up state
		if (gestureEvent.mGesture == triggerGesture) 
		{
			pickedUp = false;
			this.gameObject.GetComponent<Rigidbody>().useGravity = true;
		}
	}

	//keep the object in player view if it is picked up
	void Update()
	{
		//drop item if out of range
		if (!pickedUp)
		{
			this.gameObject.GetComponent<Rigidbody>().useGravity = true;
		}

		//will drop if false
		if (pickedUp)
		{
			this.gameObject.GetComponent<Rigidbody>().useGravity = false;
			this.transform.position = Vector3.Lerp(this.transform.position, 
			                                     mainCamera.transform.position + mainCamera.transform.forward * distance, 
			                                     Time.deltaTime * smooth);
			this.transform.rotation = Quaternion.identity;
		}
	}
}
