using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoGateLever : MonoBehaviour 
{
	//door and gate properties
	public GameObject goLever;
	public GameObject goGate;
	private bool _leverBack = false;

	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string frontMyoActivate;
	

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

	//determine if the desired gesture has been performed
	void OnGestureRecipeComplete(GestureRecipeInstance container)
	{
		if (triggeredObject.Target != null)
		{
			print (container.recipe.tag);
			//desired gesture made
			if (container.recipe.tag.Equals(frontMyoActivate) && !_leverBack)
			{
				goLever.GetComponent<Animation> () ["NewPull"].speed = 1;
				goLever.GetComponent<Animation> () ["NewPull"].time = 0;
				goLever.GetComponent<Animation> ().Play ("NewPull");
				_leverBack = true;
				
				goGate.GetComponent<Animation> () ["GateDown"].speed = 1;
				goGate.GetComponent<Animation> () ["GateDown"].time = 0;
				goGate.GetComponent<Animation> ().Play ("GateDown");
			}
		}
	}

}
