using UnityEngine;
using System.Collections;
using Thalmic;

/**
 * 
 * A cursor script for controlling the mouse within Unity
 * Other scripts can use this script to create cursor object by:
 * 1. Using the normalized x,y to set a cursor position directly
 * 2. Using the delta x, y values to move a cursor object by an arbritrary scale
 * 
 * */
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MyoCursor : PointerInputModule   {

	public GameObject cursorObject =  null;
	
	private PointerEventData pointer;

	public EventSystem eventSystem = null;
	public float defaultAcceleration = 0.3f;
	public float defaultSensitivity = 0.5f;

	//smoothing time in ms
	public float smoothTime = 10.0f;
	
	//the current delta values at this frame
	public float dx;
	public float dy;

	//the cursor position in a range of [-1,1]
	private float x;
	private float y;
	public Vector2 pos;
	public Vector2 velocity;
	private int pointerId = 0;
	public enum Type
	{
		DEFAULT,
		FIXED_ORIENTATION,
	}

	public Type type = Type.FIXED_ORIENTATION;


	protected override void Start(){
		base.Start ();
		eventSystem = FindObjectOfType<EventSystem> ();
		pointer = new PointerEventData (eventSystem);
		pointer.pointerId = pointerId;
	}


	protected override void OnEnable(){
		base.OnEnable ();
		MyoInteractibles.OnGestureStart += OnGestureStart;
		MyoInteractibles.OnGestureEnd += OnGestureEnd;
	}
	
	protected override void OnDisable(){
		base.OnDisable ();
		MyoInteractibles.OnGestureStart -= OnGestureStart;
		MyoInteractibles.OnGestureEnd -= OnGestureEnd;
	}

	bool makingFist = false;

	void OnGestureStart(GestureEvent ge){

		if (ge.mGesture == MyoInteractibles.Gesture.POSE_FIST) {

			makingFist = true;
			pointer.clickCount = 1;
		}
	}

	void OnGestureEnd(GestureEvent ge){

		if (ge.mGesture == MyoInteractibles.Gesture.POSE_FIST) {
			
			makingFist = false;
			pointer.clickCount = 0;
		}
	}
	
	public override void Process(){

		List<RaycastResult> rayResults = new List<RaycastResult>();

		eventSystem.RaycastAll (pointer, this.m_RaycastResultCache);

		int removeIndex = -1;
		if (cursorObject != null) {

			//remove the result equal to the mouse object
			for (int i = 0; i < this.m_RaycastResultCache.Count; i++) {
				if (m_RaycastResultCache [i].gameObject == cursorObject) {
					removeIndex = i;
					Debug.Log("Found first raycast");
				}
			}
	
			if (removeIndex >= 0)
				m_RaycastResultCache.RemoveAt (removeIndex);
		}
		RaycastResult raycastResult = FindFirstRaycast ( this.m_RaycastResultCache );
		pointer.pointerCurrentRaycast = raycastResult;
		this.ProcessMove (pointer);

		GetPointerData (0, out pointer, true);

		if (makingFist) {
			makingFist = false;
			pointer.pressPosition = ScreenPos;
			pointer.clickTime = Time.unscaledTime;
			pointer.pointerPressRaycast = raycastResult;
		
			pointer.eligibleForClick = true;

			if( this.m_RaycastResultCache.Count > 0 )
			{
				//if (pointer.selectedObject == raycastResult.gameObject)
				pointer.rawPointerPress = raycastResult.gameObject;

				pointer.selectedObject = raycastResult.gameObject;
				pointer.pointerPress = ExecuteEvents.ExecuteHierarchy ( raycastResult.gameObject, pointer, ExecuteEvents.submitHandler );
			}
			else
			{
				pointer.rawPointerPress = null;
			}
		}
		else
		{
			//pointer.clickCount = 0;
			pointer.eligibleForClick = false;
			pointer.pointerPress = null;
			pointer.rawPointerPress = null;
		}
		
	}

	void UpdateFixedOrientation(){

		//set the position to be dictated by the orientation of the myo
		Vector3 eulerAngles = MyoInteractibles.PrimaryMyo.Orientation.eulerAngles;
		x = -Mathf.Sin (Mathf.Deg2Rad*eulerAngles.z) * 1.0f;
		x = Mathf.Clamp (x, -1, 1);

		y = -Mathf.Sin (Mathf.Deg2Rad*eulerAngles.x) * 1.0f;
		y = Mathf.Clamp (y, -1, 1);

		pos = Vector2.SmoothDamp (pos, new Vector2(x, y), ref velocity, smoothTime*Time.deltaTime);
	}

	public float X
	{
		get { return pos.x;}
	}

	public float Y
	{
		get { return pos.y;}
	}

	void UpdateDefault(){

		Vector3 angularVelocity = MyoInteractibles.PrimaryMyo.AngularVelocity;
		float scale = 0.01f;

		dx = Mathf.Sin (Mathf.Deg2Rad*angularVelocity.y)*scale;
		dy = Mathf.Sin (Mathf.Deg2Rad * angularVelocity.x)*scale;

		x += dx;
		y += dy;

		x = Mathf.Clamp (x, -1, 1);
		y = Mathf.Clamp (y, -1, 1);
	}


	public Vector2 ScreenPos 
	{
		get {
			float posX = pos.x * Screen.width + Screen.width * 0.5f;
			float posY = pos.y * Screen.height + Screen.height * 0.5f;
			
			Vector2 screenPos = new Vector2 (posX, posY);
			pointer.position = screenPos;
			return screenPos;
		}
	}

	public void Update() {

		if (cursorObject != null) {
			cursorObject.transform.position = ScreenPos;
		}

		if (MyoInteractibles.PrimaryMyo != null) {

			if (type == Type.FIXED_ORIENTATION)
			{
				UpdateFixedOrientation();
			}
			else
			{
				UpdateDefault();
			}
		}
	}

}
