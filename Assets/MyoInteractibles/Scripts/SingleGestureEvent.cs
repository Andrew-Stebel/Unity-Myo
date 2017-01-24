using UnityEngine;
using System.Collections;

//Records an instanceo of a gesture event
public class SingleGestureEvent : GestureEvent
{
	public MyoGestureController mController;
	//single gesture events have information from the event container
	public MyoGestureController.EventContainer mEventContainer;
	
	public SingleGestureEvent(MyoGestureController controller, MyoGestureController.EventContainer eventContainer, float timestamp)
	{
		mController = controller;
		mEventContainer = eventContainer;
		mGesture = mEventContainer.gesture;
		mTimestamp = timestamp;
	}
}
