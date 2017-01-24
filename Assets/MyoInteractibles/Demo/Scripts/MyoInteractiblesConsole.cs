using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MyoInteractiblesConsole : MonoBehaviour {


	public Text gestureText;
	public Text recipeText;

	public Transform correctedTransform;

	void OnEnable(){
		MyoInteractibles.OnGestureEnd += OnGestureEnd;
		MyoInteractibles.OnGestureStart += OnGestureStart;

		MyoInteractibles.OnGestureRecipeComplete += OnGestureRecipeComplete;
		MyoInteractibles.OnGestureRecipeEvent += OnGestureRecipeEvent;
		MyoInteractibles.OnGestureRecipeFail += OnGestureRecipeFailed;

	}
	
	void OnDisable(){
		MyoInteractibles.OnGestureEnd -= OnGestureEnd;
		MyoInteractibles.OnGestureStart -= OnGestureStart;
		MyoInteractibles.OnGestureRecipeComplete -= OnGestureRecipeComplete;
		MyoInteractibles.OnGestureRecipeComplete -= OnGestureRecipeComplete;
		MyoInteractibles.OnGestureRecipeFail -= OnGestureRecipeFailed;
	}

	void WriteLine(string text, Text textField, string color)
	{
		//Debug.Log("Writing text" + text);
		textField.text =  string.Format("<color={0}>{1}</color>\n",color, text) + textField.text;
	}

	void OnGestureRecipeEvent(GestureRecipeInstance container, string eventTag){

		WriteLine(string.Format("OnGestureRecipeEvent: {0} - {1}", container.recipe.name, eventTag), recipeText, "darkblue");
	}

	void OnGestureRecipeComplete(GestureRecipeInstance container){

		WriteLine(string.Format("OnGestureRecipeCompleted: {0}", container.recipe.name), recipeText, "green");
	}

	void OnGestureRecipeFailed(GestureRecipeInstance container){

		WriteLine(string.Format("OnGestureRecipeFailed: {0}", container.recipe.name), recipeText, "red");
	}
	
	void OnGestureEnd(GestureEvent ge)
	{
		WriteLine(string.Format( "OnGestureEnd: {0} ",ge.mGesture.ToString()), gestureText, "orange");
	}

	void OnGestureStart(GestureEvent ge){

		WriteLine(string.Format( "OnGestureStart: {0} ",ge.mGesture.ToString()),gestureText, "green"); 
	}
	
	void Awake(){
		Clear ();
	}


	// Use this for initialization
	void Start () {

	}

	public void ResetButtonPressed(){

		ThalmicHub.instance.ResetHub();

	}

	void Clear()
	{
		gestureText.text = "";
		recipeText.text = "";
	}

	// Update is called once per frame
	void Update () {
	


		if (Input.GetKeyDown(KeyCode.R))
		{
			//Update all of the reference positions
			MyoInteractibles.UpdateAllReferences();
			
		}

		if (MyoInteractibles.PrimaryMyo != null)
			correctedTransform.localRotation = MyoInteractibles.PrimaryMyo.Orientation;

		if (Input.GetKeyUp(KeyCode.C))
		{
			Clear();
		}

	}

}
