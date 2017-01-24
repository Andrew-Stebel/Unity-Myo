using UnityEngine;
using System.Collections;

public class MyoDoor : MonoBehaviour 
{
	//door properties
	public GameObject door;
	private bool doorOpen = false;
	
	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string openDoorMyoTag;
	public string closeDoorMyoTag;
	
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
		if (triggeredObject.Target == null)
			return;
		
	}

	//determine if the desired gesture has been performed
	void OnGestureRecipeComplete(GestureRecipeInstance container)
	{
		if (triggeredObject.Target != null)
		{
			print (container.recipe.tag);
			//desired gestures made
			if (container.recipe.tag.Equals(openDoorMyoTag) && !doorOpen)
			{
				OpenDoorAnim();
			}
			else if(container.recipe.tag.Equals(closeDoorMyoTag) && doorOpen)
			{
				CloseDoorAnim();
			}
		}
	}

	void OpenDoorAnim()
	{
		door.GetComponent<Animation> () ["HingeDoor"].speed = 1;
		door.GetComponent<Animation> () ["HingeDoor"].time = 0;
		door.GetComponent<Animation> ().Play ("HingeDoor");
	}
	void CloseDoorAnim()
	{
		door.GetComponent<Animation> () ["HingeDoor"].speed = -1;
		door.GetComponent<Animation> () ["HingeDoor"].time = door.GetComponent<Animation> () ["HingeDoor"].length;
		door.GetComponent<Animation> ().Play ("HingeDoor");
	}
}
