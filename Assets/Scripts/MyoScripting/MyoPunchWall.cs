using UnityEngine;
using System.Collections;

public class MyoPunchWall : MonoBehaviour
{
	//wall properties
	public GameObject wall;
	private bool punched = false;

	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string punchMyoTag;

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
			if (container.recipe.tag.Equals(punchMyoTag))
			{
				if (!punched)
				{
					wall.GetComponent<Rigidbody> ().isKinematic = false;
					wall.GetComponent<Rigidbody> ().useGravity = true;
					wall.GetComponent<Rigidbody> ().AddForce ( (transform.forward * 500) + transform.up * 500 );
					
					punched = true;
					
					StartCoroutine(DestroyWall());
					
				}
			}
		}
	}
	
	IEnumerator DestroyWall()
	{
		//wait 5 seconds before removing 
		yield return new WaitForSeconds(5);
		Destroy (wall);
	}
}
