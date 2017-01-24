using UnityEngine;
using System.Collections;

public class MyoTurnableSwitch : MonoBehaviour {


	public Transform switchTransform;

	public string startTagLeft;
	public string startTagRight;

	public Material activeMaterial;
	public Material inactiveMaterial;

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

	private MyoTriggeredObject triggeredObject;

	void Awake(){
		
		switchTransform.gameObject.GetComponent<MeshRenderer>().material = inactiveMaterial;
		triggeredObject = this.gameObject.AddComponent<MyoTriggeredObject>();
		triggeredObject.triggerRadius = triggerRadius;
	}

	void OnEnable(){

		MyoInteractibles.OnGestureRecipeEvent += OnGestureRecipeEvent;

		MyoInteractibles.OnGestureRecipeFail += OnGestureRecipeFailed;

		MyoInteractibles.OnGestureRecipeComplete += OnGestureRecipeComplete;
	}

	void OnDisable(){


		MyoInteractibles.OnGestureRecipeComplete += OnGestureRecipeComplete;
		MyoInteractibles.OnGestureRecipeEvent -= OnGestureRecipeEvent;


		MyoInteractibles.OnGestureRecipeFail -= OnGestureRecipeFailed;
	}

	void OnGestureRecipeComplete(GestureRecipeInstance container){

		if (triggeredObject.Target != null)
		{
			if (container.recipe.tag.Equals(rightRecipe))
			{
				if (state == State.TURNING_RIGHT)
				{
					switchTransform.gameObject.GetComponent<MeshRenderer>().material = inactiveMaterial;
					state = State.DEFAULT;

					Debug.Log("TURNED RIGHT");
				}
			}
			else if (container.recipe.tag.Equals(leftRecipe))
			{
				if (state == State.TURNING_LEFT)
				{
					switchTransform.gameObject.GetComponent<MeshRenderer>().material = inactiveMaterial;
					state = State.DEFAULT;

					Debug.Log("TURNED LEFT");
				}
			}
		}
		//Debug.Log("Recipe complete" + container.recipe.tag);
	}

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

	void OnGestureRecipeEvent(GestureRecipeInstance container, string tag){

		if (triggeredObject.Target == null)return;

		if (tag.Equals (startTagLeft)) {

			state = State.TURNING_LEFT;
			currentTargetRotation-=90;
			switchTransform.gameObject.GetComponent<MeshRenderer> ().material = activeMaterial;
		} else if (tag.Equals (startTagRight)) {
			state = State.TURNING_RIGHT;
			currentTargetRotation+=90;
			switchTransform.gameObject.GetComponent<MeshRenderer> ().material = activeMaterial;
		}
	}


	// Use this for initialization
	void Start () {
	
	}

	
	// Update is called once per frame
	void Update () {
	
		if (!MyoInteractibles.IsRecipeActive(leftRecipe) && !MyoInteractibles.IsRecipeActive(rightRecipe))
		{
			state = State.DEFAULT;
			switchTransform.gameObject.GetComponent<MeshRenderer>().material = inactiveMaterial;
		}

		if (state == State.TURNING_LEFT || state == State.TURNING_RIGHT){

			currentRotation = Mathf.SmoothDamp(currentRotation, currentTargetRotation, ref currentRotationVelocity, Time.deltaTime * 50.0f);

			switchTransform.localRotation = Quaternion.Euler(currentRotation, 0, 0);
		}
		else if (state == State.DEFAULT){

			float maxRotation = 300.0f;
			switchTransform.localRotation = Quaternion.RotateTowards(switchTransform.localRotation, Quaternion.Euler(currentTargetRotation, 0,0), maxRotation*Time.deltaTime);
		}
	}
}
