using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoLightSwitch : MonoBehaviour 
{
	//switch properties
	public GameObject goLightSwitch;
	public AudioClip SwitchOn;
	private bool _switchOn = false;

	//other objects
	public Light Light;
	public GameObject goGate;
	private AudioSource _audioSource;


	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string lightOnMyoTag;
	public string lightOffMyoTag;

	void Awake()
	{
		_audioSource = gameObject.GetComponentInChildren<AudioSource> ();
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
			if (container.recipe.tag.Equals(lightOffMyoTag) && _switchOn)
			{
				LightSwitchOff();
			}
			else if(container.recipe.tag.Equals(lightOnMyoTag) && !_switchOn)
			{
				LightSwitchOn();
			}
		}
	}

	void LightSwitchOff()
	{
		goLightSwitch.GetComponent<Animation> () ["Toggle"].speed = -1;
		goLightSwitch.GetComponent<Animation> () ["Toggle"].time = goLightSwitch.GetComponent<Animation>()["Toggle"].length;
		goLightSwitch.GetComponent<Animation> ().Play ("Toggle");
		_switchOn = false;
		
		Light.enabled = false;
	}

	void LightSwitchOn()
	{
		goLightSwitch.GetComponent<Animation> () ["Toggle"].speed = 1;
		goLightSwitch.GetComponent<Animation> () ["Toggle"].time = 0;
		goLightSwitch.GetComponent<Animation> ().Play ("Toggle");
		_switchOn = true;
		
		
		if (goGate.activeSelf)
		{
			Light.enabled = true;
			_audioSource.clip = SwitchOn;
			_audioSource.Play ();
		}
	}

}
