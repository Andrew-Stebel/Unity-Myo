using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoValve : MonoBehaviour 
{
	//valve properties
	public Transform goValve;
	bool turning = false;

	//water properties
	public GameObject waterLevel;
	float pressure = 0;
	bool tapOn = false;

	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string startTagLeft;
	public string startTagRight;

	//code provided from demo script, MyoTurnableSwitch
		public float currentTargetRotation = 0.0f;
		float currentRotation = 0.0f;
		float currentRotationVelocity = 0.0f;
		public float triggerRadius = 5.0f;

		public string leftRecipe;
		public string rightRecipe;

		public enum State
		{
			DEFAULT,
			STARTING,
			TURNING_LEFT,//activated at the start of one of the gesture recipes
			TURNING_RIGHT,
		}
		public State state;
	


	void Awake()
	{
		triggeredObject = GetComponent<MyoObjectListener> ();
	}
	
	void OnEnable(){
		
		MyoInteractibles.OnGestureRecipeEvent += OnGestureRecipeEvent;
		
		//MyoInteractibles.OnGestureRecipeFail += OnGestureRecipeFailed;
		
		MyoInteractibles.OnGestureRecipeComplete += OnGestureRecipeComplete;
	}
	
	void OnDisable(){
		
		
		MyoInteractibles.OnGestureRecipeComplete += OnGestureRecipeComplete;
		MyoInteractibles.OnGestureRecipeEvent -= OnGestureRecipeEvent;
		
		
//		MyoInteractibles.OnGestureRecipeFail -= OnGestureRecipeFailed;
	}
	
	//determine if the desired gesture (recipe) has been performed
	void OnGestureRecipeComplete(GestureRecipeInstance container)
	{
		if (triggeredObject.Target != null)
		{
			print (container.recipe.tag);
			//desired gestures made
			if (container.recipe.tag.Equals(rightRecipe))
			{
				turning = true;
				turnRight();
				currentTargetRotation-=90;

				Debug.Log("TURNED RIGHT");
			}
			else if (container.recipe.tag.Equals(leftRecipe))
			{
				if (tapOn)
				{
					turnLeft();
					currentTargetRotation+=90;
					Debug.Log("TURNED LEFT");
				}

			}
		}
		//Debug.Log("Recipe complete" + container.recipe.tag);
	}

	//UNSURE OF FAILED RECIPE RESULTS
	void OnGestureRecipeFailed(GestureRecipeInstance container){
		
		if (container.recipe.tag.Equals(leftRecipe))
		{
			if (state == State.TURNING_LEFT)
				state = State.DEFAULT;
		}
		
		if (container.recipe.tag.Equals(rightRecipe))
		{
			if (state == State.TURNING_RIGHT)
				state = State.DEFAULT;
		}
	}


	void OnGestureRecipeEvent(GestureRecipeInstance container, string tag)
	{
		if (triggeredObject.Target == null)
			return;
	}

	void Update () 
	{
		//determine direction to move valve
		if (turning)
		{
			currentRotation = Mathf.SmoothDamp(currentRotation, currentTargetRotation, ref currentRotationVelocity, Time.deltaTime * 50.0f);
			goValve.localRotation = Quaternion.Euler(0, currentRotation, 0);
		}

		//water level inspection
		if (tapOn && pressure > 0.0f && waterLevel.transform.position.y  <= -2.3f ) 
		{
			//fill water/raise water
			waterLevel.transform.position = new Vector3(waterLevel.transform.position.x,
			                                            waterLevel.transform.position.y + pressure,
			                                            waterLevel.transform.position.z);
		}
	}

	void turnRight()
	{
		tapOn = true;
		if (pressure < 0.15f)
			pressure+=0.00050f;
	}
	
	void turnLeft()
	{
		if (tapOn)
		{
			if (pressure <= 0)
			{
				tapOn = false;
				pressure = 0;
				turning = false;
			}
			else
				pressure-= 0.00050f;
		}
	}
}
