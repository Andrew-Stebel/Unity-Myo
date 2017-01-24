using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyoObjectListener))]
public class MyoSettingsLever : MonoBehaviour 
{
	//lever properties
	public GameObject settingsLever;
	private int _settingLevel = 1;

	//fan properties
	public GameObject settingsFan;
	private float _spinLevel = 1;



	//myo object properties
	private MyoObjectListener triggeredObject;
	//tags to trigger actions
	public string pullDownMyoTag;
	public string pullUpMyoTag;
	
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
				//determine next actions
				if (_settingLevel == 1) 
				{
					settingsLever.GetComponent<Animation> () ["LeverSetting-2"].speed = 1;
					settingsLever.GetComponent<Animation> () ["LeverSetting-2"].time = 0;
					settingsLever.GetComponent<Animation> ().Play ("LeverSetting-2");
					_settingLevel = 2;
					_spinLevel = 1.5f;
				} 
				else if (_settingLevel == 2) 
				{
					settingsLever.GetComponent<Animation> () ["LeverSetting-3"].speed = 1;
					settingsLever.GetComponent<Animation> () ["LeverSetting-3"].time = 0;
					settingsLever.GetComponent<Animation> ().Play ("LeverSetting-3");
					_settingLevel = 3;
					_spinLevel = 2;
				}
			}
			else if (container.recipe.tag.Equals(pullUpMyoTag))
			{
				if (_settingLevel == 3)
				{
					settingsLever.GetComponent<Animation>()["LeverSetting-3"].speed = -1;
					settingsLever.GetComponent<Animation>()["LeverSetting-3"].time = settingsLever.GetComponent<Animation>()["LeverSetting-3"].length;
					settingsLever.GetComponent<Animation>().Play ("LeverSetting-3");
					_settingLevel = 2;
					_spinLevel = 1.5f;
				}
				else if (_settingLevel == 2)
				{
					settingsLever.GetComponent<Animation>()["LeverSetting-2"].speed = -1;
					settingsLever.GetComponent<Animation>()["LeverSetting-2"].time = settingsLever.GetComponent<Animation>()["LeverSetting-2"].length;
					settingsLever.GetComponent<Animation>().Play ("LeverSetting-2");
					_settingLevel = 1;
					_spinLevel = 1;
				}
			}
		}
	}

	void Update()
	{
		//adjust fan settings
		settingsFan.transform.Rotate(0, _spinLevel*2, 0);
		WindEffects.amplitude = _spinLevel;
	}

}
