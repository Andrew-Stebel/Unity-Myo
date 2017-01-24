using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GestureRecipeViewWindow : EditorWindow {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GestureRecipe[] gestureRecipeList = new GestureRecipe[0];

	[MenuItem("Myo Interactibles/View Recipes")]
	public static void ShowWindow(){

		var window =EditorWindow.GetWindow(typeof(GestureRecipeViewWindow)) as GestureRecipeViewWindow;

		window.Refresh();
	}



	public void Refresh(){

	}

	string searchString = "";

	Vector2 scrollPosition = Vector2.zero;

	void OnEnable(){



	}

	void OnGUI(){

		gestureRecipeList = Resources.LoadAll<GestureRecipe>("");

		IEnumerable<GestureRecipe> recipes = gestureRecipeList.Where( e=>(e.name.ToLower().Contains(searchString.ToLower()) || searchString.Length == 0));

		GUILayout.BeginHorizontal(EditorStyles.toolbar);
	
		searchString = GUILayout.TextField(searchString, EditorStyles.toolbarTextField, GUILayout.MinWidth(100));
	

		GUILayout.Label(string.Format("{0} Results found.", recipes.Count()));

		GUILayout.EndHorizontal();

		GUILayout.BeginScrollView(scrollPosition);
	

		foreach(GestureRecipe gr in recipes)
        {
			GUILayout.BeginHorizontal();

			GUILayout.Label(gr.name);

			GUILayout.FlexibleSpace();


			if (GUILayout.Button("Select",GUILayout.Width(80))){

				Selection.activeObject = gr;
				Close();
			}

			GUILayout.EndHorizontal();

		}

		GUILayout.EndScrollView();





	}




}
