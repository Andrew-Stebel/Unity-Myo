using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoWallFanLever : MonoBehaviour 
{
	//lever properties
	public GameObject wallLever;
	private bool _leverDown = false;

	//wall fan object
	public GameObject wallFan;


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
			//desired gestures made
			if (container.recipe.tag.Equals(pullDownMyoTag) && !_leverDown)
			{
				wallLever.GetComponent<Animation> () ["NewPull"].speed = 1;
				wallLever.GetComponent<Animation> () ["NewPull"].time = 0;
				wallLever.GetComponent<Animation> ().Play ("NewPull");
				Debug.Log("Pulled lever down");

				
				_leverDown = true;
			}
			else if (container.recipe.tag.Equals(pullUpMyoTag) && _leverDown)
			{
				wallLever.GetComponent<Animation> () ["NewPull"].speed = -1;
				wallLever.GetComponent<Animation> () ["NewPull"].time = wallLever.GetComponent<Animation>()["NewPull"].length;
				wallLever.GetComponent<Animation> ().Play ("NewPull");
				Debug.Log("Pulled lever up");
				_leverDown = false;
			}
		}
	}

	void Update()
	{
		//spin the fan
		if (_leverDown) 
		{
			wallFan.transform.Rotate(0, 12, 0);
		}
	}
}
