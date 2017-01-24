using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoLever : MonoBehaviour 
{	
	//lever properties
	public GameObject goLever;
	private bool _leverDown = false;
	//cube
	public GameObject holder;

	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string pullDownMyoTag;
	public string pullUpMyoTag;

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
			//desired gestures made & correct lever position
			if (container.recipe.tag.Equals(pullDownMyoTag) && !_leverDown)
			{
				goLever.GetComponent<Animation> () ["NewPull"].speed = 1;
				goLever.GetComponent<Animation> () ["NewPull"].time = 0;
				goLever.GetComponent<Animation> ().Play ("NewPull");

				//drop down the cube
				Destroy(holder);

				_leverDown = true;
			}
			else if (container.recipe.tag.Equals(pullUpMyoTag) && _leverDown)
			{
				goLever.GetComponent<Animation> () ["NewPull"].speed = -1;
				goLever.GetComponent<Animation> () ["NewPull"].time = goLever.GetComponent<Animation>()["NewPull"].length;
				goLever.GetComponent<Animation> ().Play ("NewPull");

				_leverDown = false;
			}
		}
	}
}
