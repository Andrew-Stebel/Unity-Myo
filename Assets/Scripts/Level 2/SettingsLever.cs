using UnityEngine;
using System.Collections;

public class SettingsLever : MonoBehaviour 
{	
	private GameObject _player;
	//fan lever
	public GameObject settingsLever;
	//fan
	public GameObject settingsFan;

	//other settings
	private float _spinLevel = 1;
	private int _settingLevel = 1;
	private float _distToOpen = 2f;

	void Awake()
	{
		_player = GameObject.FindWithTag("Player");
	}

	void Update()
	{
		settingsFan.transform.Rotate(0, _spinLevel*2, 0);
		WindEffects.amplitude = _spinLevel;

		if (Input.GetKeyDown (KeyCode.E)
			&& Vector3.Distance (_player.transform.position, settingsLever.transform.position) <= _distToOpen) 
		{
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

		if (Input.GetKeyDown (KeyCode.F)
			&& Vector3.Distance (_player.transform.position, settingsLever.transform.position) <= _distToOpen) 
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
