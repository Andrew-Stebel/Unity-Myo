using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoSwing : MyoObjectListener 
{
	//myo object properties
	private MyoObjectListener triggeredObject;
	//gesture to trigger actions
	public MyoInteractibles.Gesture triggerGesture = MyoInteractibles.Gesture.POSE_FIST;

	void Awake()
	{
		triggeredObject = GetComponent<MyoObjectListener> ();
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
		
		if (!triggeredObject.Target)
			return;
		
		if (gestureEvent.mGesture == triggerGesture)
		{
			print ("GU");
			PlayForwardAnimation_();
		}
	}

	//called when gesture ends (no fist)
	void OnGestureEnd(GestureEvent gestureEvent)
	{
		if (Target == null)
			return;
		
		if (gestureEvent.mGesture == triggerGesture) 
		{

		}
	}

	void PlayForwardAnimation_()
	{
		GameObject.FindWithTag("Player").transform.GetComponent<Animation> () ["Swing"].speed = 1;
		GameObject.FindWithTag("Player").transform.GetComponent<Animation> () ["Swing"].time = 0;
		GameObject.FindWithTag("Player").transform.GetComponent<Animation> ().Play ("Swing");
	}

	void PlayReverseAnimation_()
	{
		GameObject.FindWithTag("Player").transform.GetComponent<Animation> () ["Swing"].speed = -1;
		GameObject.FindWithTag("Player").transform.GetComponent<Animation> () ["Swing"].time = GameObject.FindWithTag("Player").transform.GetComponent<Animation> () ["Swing"].length;
		GameObject.FindWithTag("Player").transform.GetComponent<Animation> ().Play ("Swing");
	}


}
