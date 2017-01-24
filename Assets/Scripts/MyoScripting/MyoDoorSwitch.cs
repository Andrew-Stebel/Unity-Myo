using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoDoorSwitch : MonoBehaviour 
{
	//switch properties
	public GameObject goDoorSwitch;
	private bool _switchOn = false;
	private GameObject _upPosition;
	private GameObject _downPosition;

	//gate and light objects
	public GameObject gate;
	public Light Light;
	
	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string switchOnMyoTag;
	public string switchOffMyoTag;

	void Awake()
	{
		triggeredObject = GetComponent<MyoObjectListener> ();
	}
	
	void OnEnable(){
		
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

			//desired gestures made
			if (container.recipe.tag.Equals(switchOffMyoTag) && _switchOn)
			{
				ToggleSwitch();
			}
			else if(container.recipe.tag.Equals(switchOnMyoTag) && !_switchOn)
			{
				ToggleSwitch();
			}
		}
	}

	//flip lightswitch, check which events can occur
	void ToggleSwitch()
	{
		if (_switchOn) 
		{
			goDoorSwitch.GetComponent<Animation> () ["Toggle"].speed = -1;
			goDoorSwitch.GetComponent<Animation> () ["Toggle"].time = goDoorSwitch.GetComponent<Animation>()["Toggle"].length;
			goDoorSwitch.GetComponent<Animation> ().Play ("Toggle");
			
			_switchOn = false;
			
			
			//handle rest if no light is on
			if (!Light.enabled) {
				gate.SetActive (true);
			}
		} 
		else
		{
			goDoorSwitch.GetComponent<Animation> () ["Toggle"].speed = 1;
			goDoorSwitch.GetComponent<Animation> () ["Toggle"].time = 0;
			goDoorSwitch.GetComponent<Animation> ().Play ("Toggle");
			_switchOn = true;
			
			//handle rest if no light is on
			if (!Light.enabled) {
				gate.SetActive (false);
			}
		}
	}

}
