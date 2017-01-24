using UnityEngine;
using System.Collections;

using UnityEditor;
[CustomEditor(typeof(MyoDoorknob))]
public class MyoDoorknobEditor : Editor {

	public override void OnInspectorGUI()
	{

		GUILayout.Label ("This is an extension to the myo actor which will allow the character to control a sword with the myo");

		DrawDefaultInspector ();
		//MyoDoorknob t = target as MyoDoorknob;
	
	}
}
