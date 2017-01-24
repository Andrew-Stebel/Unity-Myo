using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoPulley : MyoObjectListener 
{
	bool leftHold = false;
	bool rightHold = false;
	
	//myo object properties
	//gesture to listen for
	public MyoInteractibles.Gesture triggerGesture = MyoInteractibles.Gesture.POSE_FIST;

	//pulley properties
	public GameObject lift;
	Vector3 originalPosition;
	Vector3 targetPosition;
	bool alternate = false;
	bool liftAtBase = true;
		
	string latestPull;

	void Awake()
	{
		originalPosition = lift.transform.position;
		targetPosition = originalPosition;
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
			return;
		
		//begin picked up state
		if (gestureEvent.mGesture == triggerGesture)
		{
			if (!leftHold)
			{
				if (MyoInteractibles.LeftMyo.lastGesture != null && MyoInteractibles.LeftMyo.lastGesture == triggerGesture)
				{
					print ("left on");
					leftHold = true;
					latestPull = "LEFTPULL";
				}
			}
			if (!rightHold)
			{
				if (MyoInteractibles.RightMyo.lastGesture != null && MyoInteractibles.RightMyo.lastGesture == triggerGesture)
				{
					print ("right on");
					rightHold = true;
					latestPull = "RIGHTPULL";
				}
			}
			StartLiftChange();
		}
	}

	//called when gesture ends (no fist)
	void OnGestureEnd(GestureEvent gestureEvent)
	{
		if (Target == null)
		{
			leftHold = false;
			rightHold = false;
			return;
		}
		
		//end picked up state
		if (gestureEvent.mGesture == triggerGesture) 
		{
			if (leftHold)
			{
				if (MyoInteractibles.LeftMyo.lastGesture == MyoInteractibles.Gesture.JERK_RIGHT)
				{
					print ("Left let go");
					leftHold = false;
				}
			}
			if (rightHold)
			{
				if (MyoInteractibles.RightMyo.lastGesture == MyoInteractibles.Gesture.JERK_RIGHT)
				{
					print ("Right let go");
					rightHold = false;
				}
			}

			ArmOffLift();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
			other.transform.parent = this.transform;
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
			other.transform.parent = null;
	}

	void StartLiftChange()
	{
		if (liftAtBase)
		{
			DetermineFirstArm();
			targetPosition = new Vector3 (lift.transform.position.x,
			                              lift.transform.position.y+2,
			                              lift.transform.position.z);
			liftAtBase = false;
		}
		//in the middle of the lift
		else
		{
			if (latestPull == "RIGHTPULL" && !alternate) 
			{
				print("right pull");
				alternate = true;
				targetPosition = new Vector3 (lift.transform.position.x,
				                              lift.transform.position.y+2,
				                              lift.transform.position.z);
			}
			else if (latestPull == "LEFTPULL" && alternate) 
			{
				print ("left pull");
				alternate = false;
				targetPosition = new Vector3 (lift.transform.position.x,
				                              lift.transform.position.y+2,
				                              lift.transform.position.z);
			}
		}
	}

	void ArmOffLift()
	{
		if (!leftHold && !rightHold)
			targetPosition = originalPosition;
	}

	void DetermineFirstArm()
	{
		if (rightHold)
		{
			alternate = true;
		}
		if (leftHold)
		{
			alternate = false;
		}
		liftAtBase = false;
	}

	void Update()
	{
		if (lift.transform.position != new Vector3 (lift.transform.position.x,
		                                            19,
		                                            lift.transform.position.z))
		{
			lift.transform.position = Vector3.MoveTowards(lift.transform.position, targetPosition, Time.deltaTime*2);
		}
		if (lift.transform.position == originalPosition)
		{
			liftAtBase = true;
		}
	}
}
