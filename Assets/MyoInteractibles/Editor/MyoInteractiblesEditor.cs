using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using Thalmic;

[CustomEditor(typeof(MyoInteractibles))]
public class MyoInteractiblesEditor :  Editor{


	public override void OnInspectorGUI(){
		GUILayout.Label ("This is the manager for the Myo Interactibles framework.");



		DrawDefaultInspector();


	
	}
}
