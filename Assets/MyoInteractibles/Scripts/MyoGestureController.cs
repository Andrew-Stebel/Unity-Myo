using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Thalmic;
using UnityEngine.UI;
using Gesture = MyoInteractibles.Gesture;
using GestureState = MyoInteractibles.GestureState;

/**
 * This class monitors a myo device and will trigger interactible gesture events when a myo gesture is detected
 * */

[Serializable]
public class MyoGestureController
{
	public ThalmicMyo myo;

	private List<MyoInteractibles.Gesture> activeGestureList = new List<MyoInteractibles.Gesture>();

	public MyoInteractibles.Gesture lastGesture;
	public Vector3 smoothedAccelerometer;//resting accelerometer data for this myo (damped with spring function)
	
	//impulse minus (damped accelerometer
	public Vector3 acceleration;
	public Vector3 jerk;//derivative of accelerometer value
	
	#region Thresholds

	public Vector3 angularVelocityUpperThreshold = new Vector3(0.1f, 0.1f, 0.1f);
	public Vector3 angularVelocityLowerThreshold = new Vector3(-0.1f -0.1f, -0.1f);
	
	public Vector3 angularAccelerationUpperThreshold = new Vector3(10f, 10f, 10f);
	public Vector3 angularAcclerationLowerThreshold = new Vector3(-10f, -10f, -10f);
	
	//Orientation Event Thresholds
	public float spinYawThreshold = 15;
	public float spinRollThreshold = 15;
	public float spinPitchThreshold = 15;

	public float spinAbritraryThreshold = 0.65f;

	//Accelerometer Event Thresholds (smoothed from accelerometer)
	public float upAccelerationThreshold = 0.5f;
	public float downAccelerationThreshold = 0.5f;
	public float rightAccelerationThreshold = 0.5f;
	public float leftAccelerationThreshold = 0.5f;
	public float forwardAccelerationThreshold = 0.5f;
	public float backwardAccelerationThreshold = 0.5f;
	
	//Jerk (acceleration / time)
	public float upJerkThreshold = 0.5f;//triggered when we go over a certain threshold
	public float downJerkThreshold = 0.5f;
	public float leftJerkThreshold = -0.5f;
	public float rightJerkThreshold = 0.5f;
	public float forwardJerkThreshold = 0.5f;
	public float backwardJerkThreshold = -0.5f;
	public float arbitraryJerkThreshold = 0.75f;

	#endregion

	#region Damping Variables

	private Vector3 correctedAccelerometerVelocity;//damped velocity
	private Vector3 smoothedAccelerometerVelocity;//damped velocity 
	private Vector3 dampedCorrectedVelocity;//velocity with damping
	private Vector3 dampedCorrectedAcceleration;

	private float dampTime = 2.0f;
	
	private float correctedDampTime = 50.0f;//impulse damping of corrected values
	
	#endregion

	#region Counters and Delays
	
	private float gestureDelayTime = 0.8f;
	private float yawDelayCounter = 0.0f;
	private float rollDelayCounter = 0.0f;
	private float pitchDelayCounter = 0.0f;

	#endregion

	
	#region Actions
	public Action<MyoGestureController, EventContainer> OnGesture;	
	public Action<MyoGestureController, EventContainer> OnGestureStarted;
	public Action<MyoGestureController, EventContainer> OnGestureEnded;
	#endregion



	//The estimated offset of this myo armband position in space.
	//This position is dependendant on the role of the myo.  E.g. if it were on the left or right arm
	//This vector should be relative to the center of the torso fo the player
	//Calculating this may vary on the application
	//The initial value of this is reset when the reference position is calculated
	public Vector3 estimatedOffset;

	//This is the acceleration at zero taking gravity into effect

	public static Vector3 Gravity = new Vector3(0,-0.98f, 0.0f);

	public Vector3 referenceAcceleration = Gravity;

	public enum ReferenceConfig{
		Auto,
		Manual
	}
	
	public ReferenceConfig referenceConfig = ReferenceConfig.Auto;

	public MyoGestureController(ThalmicMyo myo){

		this.myo = myo;
    }
	
	public class EventContainer
	{
		public Gesture gesture = MyoInteractibles.Gesture.UNKNOWN;
		public GestureState gestureState;
		public Quaternion orientation;//the orientation of the myo when this happened
		public Vector3 angularVelocity;
		public Vector3 angularAcceleration;

		public Vector3 acceleration;//the acceleration of the myo when this event happened (not taken from the accelerometer, but the corrected myo accelerometer values)
		public Vector3 jerk;//the impusle when this event happened (derivative of smoothed acceleration)

		public EventContainer(Gesture gesture, MyoGestureController controller, GestureState gestureState)
		{
			this.gesture = gesture;
			this.orientation = controller.Orientation;
			this.angularVelocity = controller.AngularVelocity;
			this.angularAcceleration = controller.angularAcceleration;
			this.acceleration = controller.Acceleration;
			this.jerk = controller.Jerk;
			this.gestureState= gestureState;
		}
	}
	

	private Vector3 smoothedOrientation;

	private float smoothedRoll;

	public float rollVelocity = 0.0f;
	public float smoothedRollVelocity = 0.0f;
	
	public float meanOrientationX;
	public float meanOrientationY;
	public float meanOrientationZ;

	Quaternion restingOrientation;//the average orientation of the myo over time
	Quaternion referenceOrientation;//the orientation that the myo has been calibrated with
	
	public float orientationSpeedX;
	public float orientationSpeedY;
	public float orientationSpeedZ;
		
	public Quaternion correctedOrientation;//takes into account roll
	private Vector3 angularVelocity;
	private Vector3 angularAcceleration;
	
	private Quaternion _antiYaw;

	float _referenceRoll = 0.0f;
	
	float restingDeviationTime = 0.0f;//the amount of time that the myo is deviating from the resting position

	// Compute the angle of rotation clockwise about the forward axis relative to the provided zero roll direction.
	// As the armband is rotated about the forward axis this value will change, regardless of which way the
	// forward vector of the Myo is pointing. The returned value will be between -180 and 180 degrees.
	float rollFromZero (Vector3 zeroRoll, Vector3 forward, Vector3 up)
	{
		// The cosine of the angle between the up vector and the zero roll vector. Since both are
		// orthogonal to the forward vector, this tells us how far the Myo has been turned around the
		// forward axis relative to the zero roll vector, but we need to determine separately whether the
		// Myo has been rolled clockwise or counterclockwise.
		float cosine = Vector3.Dot (up, zeroRoll);
		
		// To determine the sign of the roll, we take the cross product of the up vector and the zero
		// roll vector. This cross product will either be the same or opposite direction as the forward
		// vector depending on whether up is clockwise or counter-clockwise from zero roll.
		// Thus the sign of the dot product of forward and it yields the sign of our roll value.
		Vector3 cp = Vector3.Cross (up, zeroRoll);
		float directionCosine = Vector3.Dot (forward, cp);
		float sign = directionCosine < 0.0f ? 1.0f : -1.0f;
		
		// Return the angle of roll (in degrees) from the cosine and the sign.
		return sign * Mathf.Rad2Deg * Mathf.Acos (cosine);
	}



	public void SetReferenceManually()
	{
		ComputeReferenceOrientation(myo.transform.localRotation);

		ComputeReferenceGravity();

		referenceConfig = ReferenceConfig.Manual;
	}
	
	public void EnableAutoReference()
	{
		referenceConfig = ReferenceConfig.Auto;
	}


	public void ComputeReferenceOrientation(Quaternion reference)
	{
		// _antiYaw represents a rotation of the Myo armband about the Y axis (up) which aligns the forward
		// vector of the rotation with Z = 1 when the wearer's arm is pointing in the reference direction.

		Vector3 forward = reference * Vector3.forward;
		Vector3 up = reference * Vector3.up;

		_antiYaw = Quaternion.FromToRotation (
			new Vector3 (forward.x, 0, forward.z),
			new Vector3 (0, 0, 1)
			);
		
		// _referenceRoll represents how many degrees the Myo armband is rotated clockwise
		// about its forward axis (when looking down the wearer's arm towards their hand) from the reference zero
		// roll direction. This direction is calculated and explained below. When this reference is
		// taken, the joint will be rotated about its forward axis such that it faces upwards when
		// the roll value matches the reference.
		Vector3 referenceZeroRoll = computeZeroRollVector (forward);
		_referenceRoll = rollFromZero (referenceZeroRoll, forward, up);

		referenceOrientation = reference;

		//Reset the reference position while you are at it to account for drifts

		estimatedOffset = Vector3.zero;
	}

	public void ComputeReferenceGravity()
	{
		referenceAcceleration = myo.accelerometer;
	}

	
	// Compute a vector that points perpendicular to the forward direction,
	// minimizing angular distance from world up (positive Y axis).
	// This represents the direction of no rotation about its forward axis.
	Vector3 computeZeroRollVector (Vector3 forward)
	{
		Vector3 antigravity = Vector3.up;
		Vector3 m = Vector3.Cross (myo.transform.forward, antigravity);
		Vector3 roll = Vector3.Cross (m, myo.transform.forward);
		
		return roll.normalized;
	}
	
	// Adjust the provided angle to be within a -180 to 180.
	float normalizeAngle (float angle)
	{
		if (angle > 180.0f) {
			return angle - 360.0f;
		}
		if (angle < -180.0f) {
			return angle + 360.0f;
		}
		return angle;
	}

	Vector3 normalizeEulerAngle(Vector3 euler){

		Vector3 result = new Vector3(euler.x,euler.y,euler.z);
		result.x =normalizeAngle(euler.x);
		result.y = normalizeAngle(euler.y);
		result.z = normalizeAngle(euler.z);
		return result;
	}


	void UpdateCorrectedOrientation(float dt)
	{
		if (dt ==0)return;
		float dampingTime = 4.0f;

		meanOrientationX = Mathf.SmoothDampAngle(meanOrientationX, myo.transform.localEulerAngles.x, ref orientationSpeedX, dt*dampingTime);
		meanOrientationY = Mathf.SmoothDampAngle(meanOrientationY, myo.transform.localEulerAngles.y, ref orientationSpeedY, dt*dampingTime);
		meanOrientationZ = Mathf.SmoothDampAngle(meanOrientationZ, myo.transform.localEulerAngles.z, ref orientationSpeedZ, dt*dampingTime);

		//damped orientation over time
		restingOrientation = Quaternion.Euler(meanOrientationX, meanOrientationY, meanOrientationZ);

		//the reference orientation may either be automatically determined OR triggered by gesture
		if (referenceConfig == ReferenceConfig.Auto )
		{
			//only generate a reference position if our roll is not too great.
			//don't want to auto generate a reference position if one is holding a gesture.
			if (Mathf.Abs (meanOrientationZ ) < 10)
			{
				float restingAngleDifference = Quaternion.Angle(restingOrientation, referenceOrientation);
				
				if (Mathf.Abs(restingAngleDifference) > 10.0f){
					restingDeviationTime +=dt;
				}
				else{
					restingDeviationTime = 0.0f;
				}

				if(time < 3.0f || restingDeviationTime > 5.0f)
				{
					ComputeReferenceOrientation(restingOrientation);
				}
			}
		}
	
		// Current zero roll vector and roll value.
		Vector3 zeroRoll = computeZeroRollVector (myo.transform.forward);
		float roll = rollFromZero (zeroRoll, myo.transform.forward, myo.transform.up);
		
		// The relative roll is simply how much the current roll has changed relative to the reference roll.
		// adjustAngle simply keeps the resultant value within -180 to 180 degrees.
		float relativeRoll = normalizeAngle (roll - _referenceRoll);
		
		// antiRoll represents a rotation about the myo Armband's forward axis adjusting for reference roll.
		Quaternion antiRoll = Quaternion.AngleAxis (relativeRoll, myo.transform.forward);

		// Here the anti-roll and yaw rotations are applied to the myo Armband's forward direction to yield
		// the orientation of the joint.

		Quaternion newOrientation = _antiYaw * antiRoll * Quaternion.LookRotation (myo.transform.forward);

		Quaternion orientationDifference = newOrientation * Quaternion.Inverse( correctedOrientation);

		correctedOrientation = newOrientation;
		//compute the angular velocity and acceleration
		angularVelocity = orientationDifference.eulerAngles / dt;
		angularAcceleration = angularVelocity / dt;

		// The above calculations were done assuming the Myo armbands's +x direction, in its own coordinate system,
		// was facing toward the wearer's elbow. If the Myo armband is worn with its +x direction facing the other way,
		// the rotation needs to be updated to compensate.
		if (myo.xDirection == Thalmic.Myo.XDirection.TowardWrist) {
			// Mirror the rotation around the XZ plane in Unity's coordinate system (XY plane in Myo's coordinate
			// system). This makes the rotation reflect the arm's orientation, rather than that of the Myo armband.
			correctedOrientation= new Quaternion(correctedOrientation.x,
			                                      -correctedOrientation.y,
			                                      correctedOrientation.z,
			                                      -correctedOrientation.w);
		}
	}


	public void UpdateOrientationEvents(float dt){

		if (pitchDelayCounter >= 0.0f)
			pitchDelayCounter-=dt;

		if (yawDelayCounter >= 0.0f)
			yawDelayCounter-=dt;

		if (rollDelayCounter >= 0.0f)
			rollDelayCounter-=dt;

		//decrease the delay counter
		Vector3 correctedOrientationEuler = normalizeEulerAngle(correctedOrientation.eulerAngles);

		RegisterGesture(Gesture.SPIN_EVENT, (correctedOrientationEuler.magnitude > spinAbritraryThreshold));

		RegisterGesture(Gesture.SPIN_ROLL_POSITIVE, (correctedOrientationEuler.z < -spinRollThreshold));
		RegisterGesture(Gesture.SPIN_ROLL_NEGATIVE, (correctedOrientationEuler.z > spinRollThreshold));

		RegisterGesture(Gesture.SPIN_PITCH_NEGATIVE, (correctedOrientationEuler.x < -spinPitchThreshold));
		RegisterGesture(Gesture.SPIN_PITCH_POSITIVE, (correctedOrientationEuler.x > spinPitchThreshold));

		RegisterGesture(Gesture.SPIN_YAW_NEGATIVE, (correctedOrientationEuler.y < -spinYawThreshold));
		RegisterGesture(Gesture.SPIN_YAW_POSITIVE, (correctedOrientationEuler.y > spinYawThreshold));

		/*//DREW EDIT
		RegisterGesture(Gesture.VELOCITY_ROLL_NEGATIVE, (angularVelocity.z < angularAcclerationLowerThreshold.z));
		RegisterGesture(Gesture.VELOCITY_ROLL_POSITIVE, (angularVelocity.z > angularVelocityUpperThreshold.z));
		RegisterGesture(Gesture.VELOCITY_YAW_NEGATIVE, (angularVelocity.y < angularAcclerationLowerThreshold.y));
		RegisterGesture(Gesture.VELOCITY_YAW_POSITIVE, (angularVelocity.y > angularVelocityUpperThreshold.y));
		RegisterGesture(Gesture.VELOCITY_PITCH_NEGATIVE, (angularVelocity.y < angularAcclerationLowerThreshold.y));
		RegisterGesture(Gesture.VELOCITY_PITCH_POSITIVE, (angularVelocity.y > angularVelocityUpperThreshold.y));

		RegisterGesture(Gesture.ACCELERATION_ROLL_NEGATIVE, (angularAcceleration.z < angularAcclerationLowerThreshold.z));
		RegisterGesture(Gesture.ACCELERATION_ROLL_POSITIVE, (angularAcceleration.z > angularAccelerationUpperThreshold.z));
		RegisterGesture(Gesture.ACCELERATION_YAW_NEGATIVE, (angularAcceleration.y < angularAcclerationLowerThreshold.y));
		RegisterGesture(Gesture.ACCELERATION_YAW_POSITIVE, (angularAcceleration.y > angularAccelerationUpperThreshold.y));
		RegisterGesture(Gesture.ACCELERATION_PITCH_NEGATIVE, (angularAcceleration.y < angularAcclerationLowerThreshold.y));
		*/
		RegisterGesture(Gesture.ACCELERATION_PITCH_POSITIVE, (angularAcceleration.y > angularAccelerationUpperThreshold.y));
	}
	
	public void RegisterGesture(Gesture gesture, bool active){

		//set the delay counters for the axis spin gestures
		if (active)
		{
			if (gesture == Gesture.SPIN_ROLL_NEGATIVE || gesture == Gesture.SPIN_ROLL_POSITIVE)
			{
				if (rollDelayCounter > 0 )
				{
					return;
				}
				else
				{
					rollDelayCounter = gestureDelayTime;
				}
			}

			if ((gesture == Gesture.SPIN_YAW_NEGATIVE || gesture == Gesture.SPIN_YAW_POSITIVE))
			{
				
				if (yawDelayCounter > 0)
				{
					return;
				}
				else
				{
					yawDelayCounter = gestureDelayTime;
				}
			}

			if ((gesture == Gesture.SPIN_PITCH_NEGATIVE || gesture == Gesture.SPIN_PITCH_POSITIVE) && pitchDelayCounter > 0)
			{
				if (pitchDelayCounter > 0)
				{
					return;
				}
				else
				{

					pitchDelayCounter = gestureDelayTime;
				}
	        }
		}

		frameGestureList.Add(gesture);
		bool alreadyActive = false;
		
		foreach(Gesture g in activeGestureList){
			if (g == gesture){
				alreadyActive = true;
			}
		}

        
        if (active){
			OnGesture (this, new EventContainer (gesture, this, GestureState.DEFAULT));
			lastGesture = gesture;
			if (!alreadyActive){
				activeGestureList.Add(gesture);
				if (OnGestureStarted != null)
				{
					EventContainer ec = new EventContainer (gesture, this, GestureState.STARTING);
					ec.gestureState = GestureState.STARTING;
					OnGestureStarted(this, ec);
				}
			}
		}
		else
		{
			if (alreadyActive){
				activeGestureList.Remove(gesture);
				if (OnGestureEnded != null)
				{
					EventContainer ec = new EventContainer (gesture, this, GestureState.ENDING);
					ec.gestureState = GestureState.ENDING;

					OnGestureEnded(this, ec);
				}
			}
		}
	}
	
	public void UpdateAccelerometerEvents(float dt){
		smoothedAccelerometer = Vector3.SmoothDamp(smoothedAccelerometer, myo.accelerometer, ref smoothedAccelerometerVelocity, dampTime * dt);

		Vector3 newAccelerometer= Vector3.SmoothDamp (acceleration, (myo.accelerometer - smoothedAccelerometer), ref correctedAccelerometerVelocity, Time.deltaTime * correctedDampTime);

		jerk = (newAccelerometer - acceleration) / dt;
	
		//calculate the smoothed accelerometer values
		acceleration = newAccelerometer;
	
		//acceleration
		/*RegisterGesture(Gesture.ACCELERATE_UP, (acceleration.y > upAccelerationThreshold));
		RegisterGesture(Gesture.ACCELERATE_DOWN, (acceleration.y < -downAccelerationThreshold));

		RegisterGesture(Gesture.ACCELERATE_RIGHT, (acceleration.x < rightAccelerationThreshold));
		RegisterGesture(Gesture.ACCELERATE_LEFT, (acceleration.x < leftAccelerationThreshold));

		RegisterGesture(Gesture.ACCELERATE_FORWARD, (acceleration.z > forwardAccelerationThreshold));
		RegisterGesture(Gesture.ACCELERATE_BACKWARD, (acceleration.z < backwardAccelerationThreshold));
*/
		//JERK
		RegisterGesture(Gesture.JERK_UP, (jerk.y > upJerkThreshold));
		RegisterGesture(Gesture.JERK_DOWN, (jerk.y < -downJerkThreshold));
		
		RegisterGesture(Gesture.JERK_RIGHT, (jerk.x < rightJerkThreshold));
		RegisterGesture(Gesture.JERK_LEFT, (jerk.x < leftJerkThreshold));
		
		RegisterGesture(Gesture.JERK_FORWARD, (jerk.z > forwardJerkThreshold));
		RegisterGesture(Gesture.JERK_BACKWARD, (jerk.z < backwardJerkThreshold));
	}

	//gestures that are currently present in this frame
	private List<Gesture> frameGestureList = new List<Gesture>();
	
	public void UpdatePoseEvents(float dt){

		RegisterGesture(Gesture.POSE_DOUBLE_TAP, (myo.pose == Thalmic.Myo.Pose.DoubleTap));
		RegisterGesture (Gesture.POSE_FIST, (myo.pose == Thalmic.Myo.Pose.Fist));
		RegisterGesture (Gesture.POSE_REST, (myo.pose == Thalmic.Myo.Pose.Rest));
		RegisterGesture (Gesture.POSE_WAVE_IN, (myo.pose == Thalmic.Myo.Pose.WaveIn));
		RegisterGesture (Gesture.POSE_WAVE_OUT, (myo.pose == Thalmic.Myo.Pose.WaveOut));
		RegisterGesture (Gesture.POSE_FINGERS_SPREAD, (myo.pose == Thalmic.Myo.Pose.FingersSpread));
	}

	public Vector3 currentAcceleration;

	public void UpdateEstimatedPosition(float dt){

		//distance = 1/2 a (t)^2

		//correct the myo orientation using the calculated reference acceleration
		Vector3 correctedAccelerometer = myo.accelerometer - referenceAcceleration;
		Vector3 inc = correctedAccelerometer*(dt*dt)*0.5f;
		estimatedOffset += inc;
	}
	
	public Vector3 Offset{
		//TODO: implement the offset
		get { 
			return this.estimatedOffset;
		}
	}

	public Vector3 Acceleration
	{
		get {return smoothedAccelerometer;}
	}

	public Vector3 Jerk
	{
		get {return jerk;}
	}

	
	public Quaternion Orientation
	{
		get {return correctedOrientation;}
	}

	public Vector3 AngularVelocity
	{
		get {return angularVelocity;}
	}

	public Vector3 AngularAcceleration
	{
		get {return angularAcceleration;}
	}


	float time =0.0f;
	public void Update(float dt)
	{
		if (myo == null)return;

		time+=dt;

		//reset all of the frame gestures
		frameGestureList.Clear();

		UpdateCorrectedOrientation(dt);
	
		UpdateEstimatedPosition(dt);

		
		if (time > 2.0f)
		{
			//check for both orientation and impulse events occuring on this single myo
			//these events are then registered and processed by the myo actor
			UpdateOrientationEvents (dt);
			UpdateAccelerometerEvents (dt);
			UpdatePoseEvents (dt);
		}
	}
}