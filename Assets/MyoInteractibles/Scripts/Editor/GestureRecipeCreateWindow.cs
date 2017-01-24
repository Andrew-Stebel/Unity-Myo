using UnityEngine;
using UnityEditor;

using System.Collections;
using System.IO;

public class GestureRecipeCreateWindow : EditorWindow {

	const string resourcesPath = "MyoInteractibles/Resources";
	const string gestureRecipePath = resourcesPath + "/GestureRecipes";

	[MenuItem("Myo Interactibles/ Create Recipe")]
	public static void ShowWindow(){

		EditorWindow.GetWindow(typeof(GestureRecipeCreateWindow));
	}
	

	void Create()
	{

	}

	void CreateRecipe()
	{

		string properPath = Path.Combine(Application.dataPath, gestureRecipePath);
		
		if (!Directory.Exists(properPath))
		{
			AssetDatabase.CreateFolder(resourcesPath, "GestureRecipes");
		}

		string fullFileName = fileName + ".asset";

		string fullPath = Path.Combine(Path.Combine("Assets", resourcesPath),
		                               fullFileName);

		if (!File.Exists(fullPath) || EditorUtility.DisplayDialog("Replace Existing File?",
		                                                       "Are you sure you want to overrite the existing recipe named: " + fileName  + "?", "Overwrite", "Cancel"))
		{
			GestureRecipe gr = ScriptableObject.CreateInstance<GestureRecipe>();
			gr.tag = tag;
			gr.name = fileName;
			AssetDatabase.CreateAsset(gr, fullPath);
			Selection.activeObject = gr;
			
			this.Close();
		}

	}

	string fileName = "";
	string tag = "";
	void OnGUI () {


		GUILayout.Label("Create a Gesture Recipe", EditorStyles.boldLabel);

		fileName = EditorGUILayout.TextField("File Name", fileName);

		GUILayout.Label("Tag for gesture recipe events");



		tag = EditorGUILayout.TextField("Tag", tag);

		if (GUILayout.Button("Create")){
			CreateRecipe();

		}




	}

}
