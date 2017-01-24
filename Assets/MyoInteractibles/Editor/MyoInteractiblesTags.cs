using UnityEngine;
using System.Collections;
using UnityEditor;


[InitializeOnLoad]
public static class MyoInteractiblesTags{

	public static int InteractiblesLayer = 31;
	
	//STARTUP
	static MyoInteractiblesTags()
	{
		CreateLayer();
	}
	
	//creates a new layer
	static void CreateLayer(){
		SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		
		SerializedProperty it = tagManager.GetIterator();
		bool showChildren = true;
		while (it.NextVisible(showChildren))
		{
			//set your tags here
			if (it.name == "User Layer 30")
			{
				it.stringValue = "Myo Interactibles";
			}
		}
		tagManager.ApplyModifiedProperties();
	}
}