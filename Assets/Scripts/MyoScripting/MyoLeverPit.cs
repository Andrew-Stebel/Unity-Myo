using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoLeverPit : MonoBehaviour 
{	
	//lever and trap objects
	public GameObject trapLever;
	public GameObject[] trapFloors;

	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string pullDownMyoTag;
	
	void Awake()
	{
		triggeredObject = GetComponent<MyoObjectListener> ();
	}

	void OnEnable(){
		
		MyoInteractibles.OnGestureRecipeEvent += OnGestureRecipeEvent;
		MyoInteractibles.OnGestureRecipeComplete += OnGestureRecipeComplete;
		
	}

	void OnDisable(){
		
		MyoInteractibles.OnGestureRecipeEvent -= OnGestureRecipeEvent;
		
		MyoInteractibles.OnGestureRecipeComplete -= OnGestureRecipeComplete;
		
	}
	
	void OnGestureRecipeEvent(GestureRecipeInstance container, string tag)
	{
		if (triggeredObject.Target == null)return;
		
	}
	
	//determine if the desired gesture (recipe) has been performed
	void OnGestureRecipeComplete(GestureRecipeInstance container)
	{
		if (triggeredObject.Target != null)
		{
			print (container.recipe.tag);
			//desired gestures made
			if (container.recipe.tag.Equals(pullDownMyoTag))
			{
				StartCoroutine(ActivateTrap());
			}
		}
	}

	//event of pulling lever is post-poned slightly
	IEnumerator ActivateTrap()
	{
		trapLever.GetComponent<Animation> ().Play ("NewPull");
		//wait until lever is complete
		yield return new WaitForSeconds(trapLever.GetComponent<Animation>()["NewPull"].length);

		//change properties of each piece of floor
		foreach (GameObject fl in trapFloors)
		{
			fl.GetComponent<Rigidbody> ().isKinematic = false;
			fl.GetComponent<Rigidbody> ().useGravity = true;
		}
	}
}
