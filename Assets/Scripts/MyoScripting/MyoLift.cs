using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoLift : MonoBehaviour 
{
	//handle properties
	public GameObject goHandle;
	private bool _switchOut = false;

	//lift object
	public GameObject goLift;


	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string pullOutMyoTag;
	public string pushInMyoTag;

	void Awake()
	{
		triggeredObject = GetComponent<MyoObjectListener> ();
	}

	void OnEnable()
	{
		MyoInteractibles.OnGestureRecipeEvent += OnGestureRecipeEvent;
		MyoInteractibles.OnGestureRecipeComplete += OnGestureRecipeComplete;
		
	}
	
	void OnDisable()
	{
		MyoInteractibles.OnGestureRecipeEvent -= OnGestureRecipeEvent;
		MyoInteractibles.OnGestureRecipeComplete -= OnGestureRecipeComplete;
	}
	
	void OnGestureRecipeEvent(GestureRecipeInstance container, string tag)
	{
		if (triggeredObject.Target == null)
			return;
	}

	//determine if the desired gesture (recipe) has been performed
	void OnGestureRecipeComplete(GestureRecipeInstance container)
	{
		if (triggeredObject.Target != null)
		{
			print (container.recipe.tag);
			//desired gestures made
			if (container.recipe.tag.Equals(pullOutMyoTag) && !_switchOut)
			{
				goHandle.GetComponent<Animation> () ["HandlePull"].speed = 1;
				goHandle.GetComponent<Animation> () ["HandlePull"].time = 0;
				goHandle.GetComponent<Animation> ().Play ("HandlePull");
				
				_switchOut = true;
			}
			else if (container.recipe.tag.Equals(pushInMyoTag) && _switchOut)
			{
				goHandle.GetComponent<Animation> () ["HandlePull"].speed = -1;
				goHandle.GetComponent<Animation> () ["HandlePull"].time = goHandle.GetComponent<Animation> ()["HandlePull"].length;
				goHandle.GetComponent<Animation> ().Play ("HandlePull");
				_switchOut = false;
			}
		}
	}
	
	void Update()
	{
		//constantly move(play) the lift
		if (_switchOut == true) 
		{
			goLift.GetComponent<Animation>().Play ("Lift");
		}
	}

}
