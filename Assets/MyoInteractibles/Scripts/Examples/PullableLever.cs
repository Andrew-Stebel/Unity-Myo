using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoTriggeredObject))]
public class PullableLever : MonoBehaviour {
	
	public Transform leverTransform;

	public string pullDownTag;//the tag that will trigger this action
	public string pullUpTag;
	
	private MyoTriggeredObject triggeredObject;

	public float targetRotation;

	float currentRotation = 0.0f;
	float rotationVelocity = 0.0f;


	void Awake(){

		triggeredObject = GetComponent<MyoTriggeredObject> ();

	}
	
	void OnEnable(){

		MyoInteractibles.OnGestureRecipeEvent += OnGestureRecipeEvent;
		MyoInteractibles.OnGestureRecipeComplete += OnGestureRecipeComplete;

	}


	void OnDisable(){
	
		MyoInteractibles.OnGestureRecipeEvent -= OnGestureRecipeEvent;

		MyoInteractibles.OnGestureRecipeComplete -= OnGestureRecipeComplete;

	}


	void OnGestureRecipeComplete(GestureRecipeInstance container){
		
		if (triggeredObject.Target != null)
		{
			print (container.recipe.tag);
			if (container.recipe.tag.Equals(pullDownTag))
			{
				targetRotation = 180;
				Debug.Log("Pulled lever down");

			}
			else if (container.recipe.tag.Equals(pullUpTag))
			{
				targetRotation = 0;
				Debug.Log("Pulled lever up");
				rotationVelocity = 0.0f;
			}
		}
	}

	void OnGestureRecipeEvent(GestureRecipeInstance container, string tag){
		
		if (triggeredObject.Target == null)return;

	}



	// Use this for initialization
	void Start () {
	
	}



	// Update is called once per frame
	void Update () {
	
		currentRotation= Mathf.SmoothDamp (currentRotation, targetRotation, ref rotationVelocity, Time.deltaTime * 100.0f);

		leverTransform.localRotation = Quaternion.Euler (currentRotation, 90, 0);
	}
}
