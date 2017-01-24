using UnityEngine;
using System.Collections;
using Thalmic;


public class MyoInteractiblesDemoScene : MonoBehaviour {
		


	public GameObject settingsMenu;

	public void ConnectButtonPressed()
	{
#if UNITY_IOS
		MyoBindings.myo_ShowSettings ();
#endif
	}

	void Awake(){
		settingsMenu.SetActive (false);
	}

	public void MenuButtonPressed(){

		settingsMenu.SetActive (!settingsMenu.gameObject.activeSelf);
	}
}
