using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoBoatMechanics : MonoBehaviour 
{
	//boat variables
	public GameObject boat;
	public static bool onBoat = false;
	float origBoatY;

	//start/end positions
	public GameObject endPosit;
	public GameObject startPosit;
	float timer;

	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string rowingTag;
	Vector3 nextPosition;
	
	bool thereYet = false;

	void Awake()
	{
		origBoatY = boat.transform.position.y;
		triggeredObject = GetComponent<MyoObjectListener> ();
		nextPosition = boat.transform.position;
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
			if (!thereYet && container.recipe.tag.Equals(rowingTag))
			{
				nextPosition = new Vector3(boat.transform.position.x,
				                                   boat.transform.position.y,
				                                   boat.transform.position.z+2);
			}
		}
	}

	bool DistanceCheck()
	{
		if (Vector3.Distance (boat.transform.position, endPosit.transform.position) <= 5.3f)
		{
			return true;
		}
		return false;
	}

	void Update()
	{
		timer += Time.deltaTime;
		boat.transform.position = new Vector3 (boat.transform.position.x,
		                                       origBoatY + 0.2f * Mathf.Sin (1 * timer),
		                                       boat.transform.position.z);

		boat.transform.position = Vector3.MoveTowards(boat.transform.position, nextPosition, Time.deltaTime*2);

		if (DistanceCheck())
			thereYet = true;
	}
}
