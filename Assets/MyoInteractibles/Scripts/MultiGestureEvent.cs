using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MultiGestureEvent : GestureEvent
{
	private List<SingleGestureEvent> mEvents; 
	
	public MultiGestureEvent(float timestamp, List<SingleGestureEvent> events, MyoInteractibles.Gesture gesture)
	{
		mEvents = events;
		mGesture = gesture;
		mTimestamp = timestamp;
	}
	
	public MultiGestureEvent(float timestamp, SingleGestureEvent event1, SingleGestureEvent event2, MyoInteractibles.Gesture gesture)
	{
		mTimestamp = timestamp;
		mGesture = gesture;
		mEvents = new List<SingleGestureEvent>();
		mEvents.Add(event1);
		mEvents.Add (event2);
	}
}