using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MyoPushBlock : MyoObjectListener 
{
	bool leftSpread = false;
	bool rightSpread = false;

	//myo object properties
	//gesture to listen for
	public MyoInteractibles.Gesture triggerGesture = MyoInteractibles.Gesture.POSE_FIST;

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
			return;
		
		//begin picked up state
		if (gestureEvent.mGesture == triggerGesture)
		{
			if (MyoInteractibles.LeftMyo.lastGesture == triggerGesture)
				leftSpread = true;
			if (MyoInteractibles.RightMyo.lastGesture == triggerGesture)
				rightSpread = true;
		}
	}

	//called when gesture ends (no fist)
	void OnGestureEnd(GestureEvent gestureEvent)
	{
		if (Target == null)
		{
			leftSpread = false;
			rightSpread = false;
			return;
		}
		
		//end picked up state
		if (gestureEvent.mGesture == triggerGesture) 
		{
			if (MyoInteractibles.LeftMyo.lastGesture == triggerGesture)
				leftSpread = false;
			if (MyoInteractibles.RightMyo.lastGesture == triggerGesture)
				rightSpread = false;
		}
	}

	void Update()
	{
		if (leftSpread && rightSpread)
		{
			this.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * 2);
			this.gameObject.GetComponent<Rigidbody>().AddForce(transform.right * 10);
		}
	}
	
}
