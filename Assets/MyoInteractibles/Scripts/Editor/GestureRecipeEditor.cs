using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

[CustomEditor(typeof(GestureRecipe))]
public class GestureRecipeEditor : Editor {

	private ReorderableList list;
	
	private const string ROOT_MENU = "MyoInteractibles";

	private void OnEnable() {

		GestureRecipe gr = (GestureRecipe) target;

		if (gr.itemList == null)
		{
			gr.itemList = new System.Collections.Generic.List<GestureRecipe.Item>();
		}

		list = new ReorderableList(serializedObject, 
		                           serializedObject.FindProperty("itemList"), 
		                           true, true, true, true);

		list.drawHeaderCallback = (Rect rect) => {

			EditorGUI.LabelField(rect, "Recipe Items");

		};
	
	
		list.elementHeight = EditorGUIUtility.singleLineHeight*11;
		list.drawElementCallback =  
		(Rect rect, int index, bool isActive, bool isFocused) => {

		

			int propertyIndex = 0;

			var element = list.serializedProperty.GetArrayElementAtIndex(index);

			GestureRecipe.Item itemElement = gr.itemList[index];

			rect.y += 2;

			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("myoRole"), new GUIContent("Role"));

			propertyIndex++;

			rect.y+=EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("gesture"), new GUIContent("Gesture"));

			propertyIndex++;

			rect.y+=EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("gestureState"), new GUIContent("State"));

			propertyIndex++;
			
			rect.y+=EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("eventTag"), new GUIContent("Event Tag"));

			propertyIndex++;
			rect.y+=EditorGUIUtility.singleLineHeight;
			if (index != 0)
			{
				//Gestures that are pose gestures are binary and can't have values
				if (itemElement.gesture > MyoInteractibles.Gesture.POSE_DOUBLE_TAP)
				{
					rect.y+=EditorGUIUtility.singleLineHeight;
					EditorGUI.PropertyField(
						new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),
						element.FindPropertyRelative("minValue"), new GUIContent("Minimum Value"));
					propertyIndex++;
				
					rect.y+=EditorGUIUtility.singleLineHeight;
					EditorGUI.PropertyField(
						new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),
						element.FindPropertyRelative("maxValue"), new GUIContent("Maximum Value"));
					propertyIndex++;
				}
				else
				{
					/*rect.y+=EditorGUIUtility.singleLineHeight;
					EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),"");
					rect.y+=EditorGUIUtility.singleLineHeight;
					EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),"");
				
					propertyIndex+=2;*/
				}

				rect.y+=EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("minDuration"), new GUIContent("Minimum Duration"));
				propertyIndex++;

				rect.y+=EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("maxDuration"), new GUIContent("Maximum Duration"));
				propertyIndex++;

				rect.y+=EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("delayFromLastMin"), new GUIContent("Minimum Time from Last Item"));
				propertyIndex++;

				rect.y+=EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("delayFromLastMax"), new GUIContent("Maximum Time from Last Item"));
				propertyIndex++;

			}
		};

	}
	    
    public override void OnInspectorGUI(){

		GestureRecipe gr = (GestureRecipe) target;
        
        serializedObject.Update();

		GUILayout.Label("Select a unique event tag for this recipe. \n This tag will be fired by MyoInteractibles manager when the recipe is complete.\n");

		gr.tag = EditorGUILayout.TextField("Event Tag", gr.tag);

		GUILayout.Label("Recipe Items");

		list.DoLayoutList();
		serializedObject.ApplyModifiedProperties();

		//DrawDefaultInspector ();
	}
}