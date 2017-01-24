using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MyoPickupObject : MyoTriggeredObject {

	public MyoInteractibles.Gesture triggerGesture = MyoInteractibles.Gesture.POSE_FIST;
	
	void OnEnable(){
		MyoInteractibles.OnGestureStart += OnGestureStart;
		MyoInteractibles.OnGestureEnd += OnGestureEnd;
	}


	void OnDisable(){
		MyoInteractibles.OnGestureStart -= OnGestureStart;
		MyoInteractibles.OnGestureEnd -= OnGestureEnd;
	}
	
	public bool pickedUp = false;

	void PickUp()
	{
		this.transform.parent = Target.transform;
		this.GetComponent<Rigidbody>().isKinematic = true;
		pickedUp = true;
	}
	
	void Drop()
	{
		this.transform.parent = null;
		this.GetComponent<Rigidbody>().isKinematic = false;
		pickedUp = false;
	}

	void OnGestureStart(GestureEvent gestureEvent){

		if (!Target)
			return;

		if (gestureEvent.mGesture == triggerGesture){
			PickUp();
		}
	}

	void OnGestureEnd(GestureEvent gestureEvent){

		if (Target == null)
			return;

		if (gestureEvent.mGesture == triggerGesture) {
			if (transform.parent == Target.transform) {
				Drop ();
			}
		}
	}

}
