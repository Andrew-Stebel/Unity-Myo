using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Thalmic;
using System;

/**
 * The Myo Interactibles class manages gestures from the ThalmicMyo objects.  This class will find active myos within the application 
 * then assign them to specific "Myo Roles" (e.g. dominant hand, non dominant hand) so that they can be used within interactibles.
 * It will then use the gyroscope and accelerometer in the myo device in order to create its own gestures.  These gestures can be combined with
 * the built in Myo Pose gestures to create complex gestures.
 * */

//these are more complex gestures than those provided by the myo firmware and may include a combination of myos as well as mixing pose + IMU data


public class MyoInteractibles : MonoBehaviour {
	
	public static float RECIPE_EXPIRTY_TIME = 2.0f;//the max time since the last item in an event could have happened...
	public static float EVENT_EXPIRY_TIME = 5.0f;//time in seconds than an event takes to be removed from the log
	
	private List<GestureEvent> frameEventList = new List<GestureEvent>();//gesture events within this frame
	private List<GestureRecipeInstance> activeRecipeInstances = new List<GestureRecipeInstance>();//recipes that are still valid and active
	
	public static MyoInteractibles Instance;

	//Currently supported myo roles
	//In the future we will ideally support legs and possibly other body parts
	public enum MyoRole
	{
		PRIMARY,

		LEFT_FOREARM,
		RIGHT_FOREARM,

		LEFT_UPPERARM,
		RIGHT_UPPERARM,
	}
	    
    //based on the settings and configuratino of the myo interactibles object there may be different myos configured to different roles...
	//this can be defined by the myo settings asset
	public Dictionary<MyoRole, MyoGestureController> controllerDictionary = new Dictionary<MyoRole, MyoGestureController>();
		
	#region Callbacks

	public static Action<GestureEvent> OnGesture;
	public static Action<GestureEvent> OnGestureEnd;
	public static Action<GestureEvent> OnGestureStart;
	
	public static Action<GestureRecipeInstance> OnGestureRecipeFail;
	public static Action<GestureRecipeInstance> OnGestureRecipeComplete;
	public static Action<GestureRecipeInstance> OnGestureRecipeItemComplete;

	public static Action<GestureRecipeInstance, string> OnGestureRecipeEvent;
	
	#endregion
	
	[HideInInspector]
	public bool leftHanded = false;

	public List<MyoGestureController> gestureControllers;
	
	private List<GestureRecipe> gestureRecipeList;//default recipes will trigger built in events

	public enum Gesture
	{
		//per myo device rotations...could be used as swipe controls
		UNKNOWN,
		
		//These are the basic myo pose gestures
		POSE_REST,
		POSE_FIST,
		POSE_WAVE_IN,
		POSE_WAVE_OUT,
		POSE_FINGERS_SPREAD,
		POSE_DOUBLE_TAP,
		
		//These are single custom myo gesture events.  Currently mainly based in acceleration impulses or deviations from resting orientation
		
		//orientation based events
		SPIN_ROLL_NEGATIVE,//spin along the x axis (naturally like spinning to the left / right
		SPIN_ROLL_POSITIVE,
		SPIN_YAW_POSITIVE,//spin along the z axis
		SPIN_YAW_NEGATIVE,
		SPIN_PITCH_POSITIVE,//spin along the y axis
		SPIN_PITCH_NEGATIVE,
		SPIN_EVENT, //spin event along no axis in particular (orientation greater than arbitrary threshold
		
		VELOCITY_ROLL_POSITIVE,
		VELOCITY_ROLL_NEGATIVE,
		VELOCITY_YAW_POSITIVE,
		VELOCITY_YAW_NEGATIVE,
		VELOCITY_PITCH_POSITIVE,
		VELOCITY_PITCH_NEGATIVE,
		
		ANGULAR_VELOCITY_EVENT,
		
		//Simple acceleration events
		ACCELERATION_ROLL_NEGATIVE,
		ACCELERATION_ROLL_POSITIVE,
		ACCELERATION_YAW_NEGATIVE,
		ACCELERATION_YAW_POSITIVE,
		ACCELERATION_PITCH_NEGATIVE,
		ACCELERATION_PITCH_POSITIVE,
		
		ANGULAR_ACCELERATION_EVENT,
		
		//accelerometer based events
		ACCELERATE_UP,//y axis
		ACCELERATE_DOWN,
		ACCELERATE_LEFT,
		ACCELERATE_RIGHT,
		ACCELERATE_FORWARD,
		ACCELERATE_BACKWARD,
		ACCELERATE_EVENT,
		
		//quick changes in accelerometer values
		JERK_UP,
		JERK_DOWN,
		JERK_LEFT,
		JERK_RIGHT,
		JERK_FORWARD,
		JERK_BACKWARD,
		JERK_EVENT,
		
		//These are complex gestures that are based on a combination of all gestures in certain order or combination
		DOUBLE_RAISE_UP,
		DOUBLE_SPIN_LEFT,
		DOUBLE_SPIN_RIGHT,
		DOUBLE_SPIN_CONVERGE,
		DOUBLE_SPIN_DIVERGE
	}
	
	public enum GestureState
	{
		DEFAULT,//ongesture (also called ongesturestart)
		STARTING,//ongesturestart
		ENDING,//ongestureend
	}

	public IEnumerator Start(){

		yield return new WaitForSeconds (1.0f);
	
		RefreshMyos();
	}

	/// <summary>
	/// keeps track of the gesture event history in order to combine events
	/// </summary>
	public List<GestureEvent> gestureEvents = new List<GestureEvent>();
	
	
	//Find the unique combinations of single gesture events that make meaningful gesture events
	private void RegisterMultiGestures(MyoGestureController.EventContainer eventContainer, SingleGestureEvent sge, MyoGestureController controller, float timestamp, GestureState state){

		Gesture aGesture = eventContainer.gesture;//gesture we just received and are evaluating

		foreach(GestureEvent gestureEvent in gestureEvents)
		{

			if (gestureEvent is SingleGestureEvent){

				SingleGestureEvent ge = (SingleGestureEvent) gestureEvent;

			//check for an event with the same gesture but opposite arm.
			if (aGesture == ge.mGesture)
			{
				//make sure that the gesture did not happen with the same controller
				if (ge.mController != controller)
				{
					//make sure that they are different arms
					if (ge.mController.myo.arm != controller.myo.arm)
					{
						//Look for event combinations
						if (ge.mGesture == Gesture.SPIN_ROLL_NEGATIVE && aGesture == Gesture.SPIN_ROLL_NEGATIVE)
						{
							//check to see if they happened within a threshold for the double spin event
							float spinLeftThresholdTime = 0.5f;
							if (ge.mTimestamp - timestamp < spinLeftThresholdTime)
							{
								MultiGestureEvent mge = new MultiGestureEvent(Time.time, ge, sge, Gesture.DOUBLE_SPIN_LEFT);
								RegisterEvent(mge);
								if (OnGesture != null)
								{
									OnGesture(mge);
								}
							}
						}
						else if (ge.mGesture == Gesture.SPIN_ROLL_POSITIVE && sge.mGesture == Gesture.SPIN_ROLL_POSITIVE)
						{
							float threshold = 0.5f;
							if (ge.mTimestamp - sge.mTimestamp < threshold)
							{
								MultiGestureEvent mge = new MultiGestureEvent(Time.time, ge, sge, Gesture.DOUBLE_SPIN_RIGHT);
								RegisterEvent(mge);

								if (OnGesture != null)
								{
									OnGesture(mge);
								}
							}
						}
						else if ((ge.mGesture == Gesture.SPIN_ROLL_NEGATIVE && sge.mGesture == Gesture.SPIN_ROLL_POSITIVE) || 
						         (ge.mGesture == Gesture.SPIN_ROLL_POSITIVE && sge.mGesture == Gesture.SPIN_ROLL_NEGATIVE))
						{
							float threshold = 0.5f;
							if (ge.mTimestamp - sge.mTimestamp < threshold)
							{
								MultiGestureEvent mge = new MultiGestureEvent(Time.time, ge, sge, Gesture.DOUBLE_SPIN_RIGHT);
								RegisterEvent(mge);
				
								if (OnGesture != null)
								{
									OnGesture(mge);
								}
							}
						}
					}
				}
			}
			}
		}
	}

	public void OnControllerGestureEnd(MyoGestureController controller, MyoGestureController.EventContainer eventContainer)
	{
		SingleGestureEvent e = new SingleGestureEvent(controller, eventContainer, Time.time);

		e.gestureState = GestureState.ENDING;
        RegisterEvent(e);

		RegisterMultiGestures (eventContainer, e, controller, Time.time, GestureState.ENDING);

		if (OnGestureEnd != null)
			OnGestureEnd(new SingleGestureEvent(controller, eventContainer, Time.time));
	}

	public void OnControllerGestureStart(MyoGestureController controller, MyoGestureController.EventContainer eventContainer)
	{
		SingleGestureEvent e = new SingleGestureEvent(controller, eventContainer, Time.time);


		e.gestureState = GestureState.STARTING;
		RegisterEvent(e);

		RegisterMultiGestures (eventContainer, e, controller, Time.time, GestureState.STARTING);

		if (OnGestureStart != null)
			OnGestureStart(new SingleGestureEvent(controller, eventContainer, Time.time));
	}
	
	//This callback is triggered when one of the myo gesture controller objects triggers an events
	public void OnControllerGesture(MyoGestureController controller, MyoGestureController.EventContainer eventContainer)
	{
		SingleGestureEvent e = new SingleGestureEvent(controller, eventContainer, Time.time);
		RegisterEvent(e);

		RegisterMultiGestures (eventContainer, e, controller, Time.time, GestureState.DEFAULT);
		if (OnGesture != null)
			OnGesture(e);
	}

	public void RegisterEvent(GestureEvent evt)
	{
		frameEventList.Add(evt);
		gestureEvents.Add (evt);
	}
	
	/**
	 *  Clear events that are older than a certain time range
	 * */
	public void FlushEventLog(float timestamp)
	{
		gestureEvents.RemoveAll(e=>(timestamp - e.mTimestamp) > EVENT_EXPIRY_TIME);
		activeRecipeInstances.RemoveAll(e=>(timestamp - e.timestamp) > RECIPE_EXPIRTY_TIME);
	}
	
	void Awake(){

		if (Instance != null){
			Destroy (this.gameObject);
			return;
		}

		gestureRecipeList = new List<GestureRecipe>( Resources.LoadAll<GestureRecipe>(""));

		Instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	void OnDestroy(){
		if (this == Instance)Instance = null;

		ClearControllers();
	}

	void ClearControllers(){

		foreach(MyoGestureController gc in gestureControllers)
		{
			gc.OnGesture-=OnControllerGesture;
			gc.OnGestureStarted-=OnControllerGestureStart;
			gc.OnGestureEnded-=OnControllerGestureEnd;
		}
		gestureControllers.Clear();
	}


	//This method automatically refreshes the myo and will set the myos to the default roles depending on how the myo has been synced.
	//e.g. if the myo is on the right arm put it as the right arm role
	//NOTE: right arm is by default the primary myo, but if only left arm is synched it takes over this role
	public void RefreshMyos(){

		if (ThalmicHub.instance == null)return;

		ClearControllers();
		controllerDictionary.Clear();

		foreach(ThalmicMyo m in ThalmicHub.instance._myos)
		{
			MyoGestureController gc = new MyoGestureController(m);

			gc.OnGesture+=OnControllerGesture;
			gc.OnGestureStarted += OnControllerGestureStart;
			gc.OnGestureEnded += OnControllerGestureEnd;

			gestureControllers.Add(gc);
		}

		foreach(MyoGestureController gc in gestureControllers)
		{
			if (!controllerDictionary.ContainsKey(MyoRole.LEFT_FOREARM))
			{
				if (gc.myo.arm == Thalmic.Myo.Arm.Left)
				{
					controllerDictionary[MyoRole.LEFT_FOREARM] = gc;
				}
			}

			if (!controllerDictionary.ContainsKey(MyoRole.RIGHT_FOREARM)){

				if (gc.myo.arm == Thalmic.Myo.Arm.Right){

					controllerDictionary[MyoRole.RIGHT_FOREARM] = gc;
				}
			}
		}

		if (controllerDictionary.ContainsKey (MyoRole.RIGHT_FOREARM)) {
			controllerDictionary [MyoRole.PRIMARY] = controllerDictionary [MyoRole.RIGHT_FOREARM];
		} else if (controllerDictionary.ContainsKey (MyoRole.LEFT_FOREARM)) {
			controllerDictionary [MyoRole.PRIMARY] = controllerDictionary [MyoRole.LEFT_FOREARM];
		}

	}

	public void UpdateRecipeInstances(){

		List<GestureRecipeInstance> completedInstances = new List<GestureRecipeInstance>();
		List<GestureRecipeInstance> rejectedInstances = new List<GestureRecipeInstance>();

		//based on the currently active gesture events in the log, see if we can initiate any gestures!
		foreach(GestureEvent ge in frameEventList)
		{
			foreach(GestureRecipe gr in gestureRecipeList)
			{
				GestureRecipe.Item rootItem = gr.Root;

				//make sure that we don't currently have an active gesture at the same step and recipe.
				bool contained = false;
				foreach(GestureRecipeInstance gri in activeRecipeInstances)
				{
					if (gri.recipe == gr && gri.stepIndex == 1)
					{
						contained = true;
					}
				}

				//we already have a recipe that is at this index
				if (contained)
					break;

				if (rootItem != null){

					if (GestureRecipeInstance.ItemSatisfied(rootItem, ge, 0.0f, true))
					{
						GestureRecipeInstance gri = new GestureRecipeInstance(gr, Time.time);
						gri.GoNext(Time.time);


						if (rootItem.eventTag != null && rootItem.eventTag.Length > 0)
						{
							if (OnGestureRecipeEvent != null)
								OnGestureRecipeEvent(gri, rootItem.eventTag);
						}

						if (gri.Completed)//in case this recipe is only one step...
						{
							completedInstances.Add(gri);
                        }
						else
						{
							activeRecipeInstances.Add(gri);
						}
					}
				}
			}
		}

		//based on the current active gesture recipes look for recipes that have their next item satisfied
		foreach(GestureRecipeInstance gri in activeRecipeInstances){

			foreach(GestureEvent ge in frameEventList)
			{

				if (gri.Rejected(ge))
				{
					rejectedInstances.Add (gri);
					//we no longer want to look for valid events...because we have gone too far!
					break;
				}
				else
				{
					if (gri.Satisfied(ge))
					{
						GestureRecipe.Item currentItem = gri.CurrentItem;
						if (currentItem != null)
						{

							if (currentItem.eventTag != null && currentItem.eventTag.Length > 0)
							{
								if (OnGestureRecipeEvent != null)
									OnGestureRecipeEvent(gri, currentItem.eventTag);
	                        }
						}

                        gri.GoNext(Time.time);
                        
                        
                        
                        if (gri.Completed)
						{
							completedInstances.Add(gri);
							if (OnGestureRecipeItemComplete != null)
								OnGestureRecipeItemComplete(gri);

							break;
						}

					}
				}

			}
		}

		foreach(GestureRecipeInstance gre in completedInstances){
			activeRecipeInstances.Remove(gre);
			if (OnGestureRecipeComplete != null)
				OnGestureRecipeComplete(gre);
		}

		foreach(GestureRecipeInstance gre in rejectedInstances){
			if (OnGestureRecipeFail != null)
				OnGestureRecipeFail(gre);
			activeRecipeInstances.Remove(gre);
		}
	}
	
	public static int NumActiveRecipes
	{
		get {
			if (Instance == null)return 0;

			return Instance.activeRecipeInstances.Count;
		}
	}

	public static bool IsRecipeActive(string recipeTag)
	{
		if (Instance == null)return false;

		foreach(GestureRecipeInstance gre in Instance.activeRecipeInstances)
		{
			if (gre.recipe.tag.Equals(recipeTag))
			{
				return true;
			}
		}
		return false;
	}

	public void Update() {

		//Clear all of the gesture events in the current frame
		frameEventList.Clear();

		//Update all of the gesture controllers to look for active gestures in this frame
		foreach(MyoGestureController gc in gestureControllers){
			gc.Update(Time.deltaTime);
		}

		//update the current gesture recipe states
		UpdateRecipeInstances();

		//Clean up event log and remove gestures older than a certain range
		FlushEventLog(Time.time);
#if UNITY_EDITOR
		//For toggling the debug console for the gesture controllers
		if (Input.GetKeyDown(KeyCode.X))
		{
			debug=!debug;
		}
#endif
	}


#if UNITY_EDITOR

	bool debug = false;

	void OnGUI(){
		if (!debug)return;

		GUILayout.BeginArea(new Rect(0,0,Screen.width,Screen.height));
		{
			GUILayout.Label("Primary");

			GUILayout.BeginVertical();
			{
				GUILayout.Label("Primary");

				GUILayout.Label("Mean Accelerometer");
				float range = 0.1f;

				GUILayout.Label("Mean Accelerometer Low Pass BW");
                
				MyoGestureController controller = PrimaryMyo;
				if (controller != null)
				{
					GUILayout.Label("Mean Accelerometer");
	                
					GUILayout.HorizontalSlider(controller.AngularVelocity.x, -range, range);
					GUILayout.HorizontalSlider(controller.AngularVelocity.y, -range, range);
					GUILayout.HorizontalSlider(controller.AngularVelocity.z, -range, range);

					GUILayout.Label("Corrected Accelerometer");

					GUILayout.HorizontalSlider(controller.Acceleration.x, -range, range);
					GUILayout.HorizontalSlider(controller.Acceleration.y, -range, range);
					GUILayout.HorizontalSlider(controller.Acceleration.z, -range, range);
	                
					GUILayout.Label("Corrected Impulse");
				}
			}
			GUILayout.EndHorizontal();
		}

		GUILayout.EndArea();
	}

#endif

	//Updates all of the references rotations for the gesture controller to be the current orientation
	public static void UpdateAllReferences()
	{
		foreach(MyoGestureController gc in Instance.gestureControllers)
		{
			gc.SetReferenceManually();
		}
	}

	//Update the reference orientation for a particular myo controller
	public static void UpdateReference(MyoGestureController gc)
	{
		gc.SetReferenceManually();
	}
	
	public static void SetPrimaryMyo(MyoGestureController myoController){

		if (Instance == null || myoController == null)
			return;

		Instance.controllerDictionary [MyoRole.PRIMARY] = myoController;
	}

	public static void SetMyoRole(MyoGestureController myoController, MyoRole role){
		if (Instance == null || myoController == null)
			return;

		//Clear the previous myo roles of this myo
		foreach (MyoRole m in Instance.controllerDictionary.Keys) {

			if (m != MyoRole.PRIMARY)
			{
				if (role == m)
					Instance.controllerDictionary.Remove(m);
			}
		}

		Instance.controllerDictionary [role] = myoController;
	}

	public static MyoGestureController LeftMyo
	{
		get 
		{ 
			if (Instance != null && Instance.controllerDictionary.ContainsKey(MyoRole.LEFT_FOREARM))
			{
				
				return Instance.controllerDictionary[MyoRole.LEFT_FOREARM];
			}
			return null;
		}
	}
	
	public static MyoGestureController LeftUpperMyo
	{
		get 
		{ 
			if (Instance != null && Instance.controllerDictionary.ContainsKey(MyoRole.LEFT_FOREARM))
			{
				
				return Instance.controllerDictionary[MyoRole.LEFT_FOREARM];
			}
			return null;
		}
	}

	public static MyoGestureController RightMyo
	{
		get 
		{ 
			if (Instance != null && Instance.controllerDictionary.ContainsKey(MyoRole.RIGHT_FOREARM))
			{
				
				return Instance.controllerDictionary[MyoRole.RIGHT_FOREARM];
			}
			return null;
		}
	}


	public static MyoGestureController RightUpperMyo
	{
		get 
		{ 
			if (Instance != null && Instance.controllerDictionary.ContainsKey(MyoRole.RIGHT_UPPERARM))
			{
				
				return Instance.controllerDictionary[MyoRole.RIGHT_FOREARM];
			}
			return null;
		}
	}


	public static MyoGestureController PrimaryMyo {
		get {
			if (Instance != null && Instance.controllerDictionary.ContainsKey (MyoRole.PRIMARY)) {

				return Instance.controllerDictionary [MyoRole.PRIMARY];
			}

			//Check for a myo in the thalmic hub to match the primary myo role...
			if (Instance)
				Instance.RefreshMyos ();

			return null;
		}
	}
	}

