using UnityEngine;
using System.Collections;

//This script will turn on and off an object when the user uses a certain gesture.
public class GestureToggleObject : MonoBehaviour {

	//the gesture that will enable this object
	public MyoInteractibles.Gesture gesture;
	public Transform target;

	void OnEnable(){

		MyoInteractibles.OnGestureStart += OnGestureStart;
		MyoInteractibles.OnGestureEnd += OnGestureEnd;
		target.gameObject.SetActive (false);
	}

	void OnDisable(){

		MyoInteractibles.OnGestureStart -= OnGestureEnd;

	}
	
	void OnGestureEnd(GestureEvent gestureEvent)
	{
		if (gestureEvent.mGesture == gesture)
			target.gameObject.SetActive (false);
	}

	void OnGestureStart(GestureEvent gestureEvent)
	{
		if (gestureEvent.mGesture == gesture)
			target.gameObject.SetActive (true);
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
